using System;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AndroidXml;
using Models;
using Nuke.Common;
using Nuke.Common.IO;
using Schemas;
using Serilog;

public partial class Build
{
    [Parameter("CRM base url"), Secret] 
    readonly string CrmBaseUrl = string.Empty;

    [Parameter("CRM username"), Secret] 
    readonly string CrmUsername = string.Empty;

    [Parameter("CRM password"), Secret] 
    readonly string CrmPassword = string.Empty;

    Target Deploy => target => target
        .After(BuildAndroidUnity)
        .Executes(async () =>
        {
            if (string.IsNullOrEmpty(CrmBaseUrl))
            {
                Log.Warning("CRM base url not set");
                return;
            }
            
            if (string.IsNullOrEmpty(CrmUsername))
            {
                Log.Warning("CRM username not set");
                return;
            }
            
            if (string.IsNullOrEmpty(CrmPassword))
            {
                Log.Warning("CRM password not set");
                return;
            }
            
            var accessToken = await Authorization(CancellationToken.None);
            var authenticationHeader = new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);
            Log.Information("Authorization successful");

            var buildReportPath = GetBuildReportFileOrDefault();
            if (buildReportPath is null)
            {
                Log.Error("Build report file not found");
                return;
            }

            var buildReport = await GetBuildReportAsync(buildReportPath, CancellationToken.None);
            var buildOptions = buildReport.BuildOptions;
            AbsolutePath buildPath = buildOptions.LocationPathName;
            if (buildPath is null)
            {
                Log.Error("Build file not found");
                return;
            }

            var assignedFile = await AssignBuildFileToUrlAsync(buildPath.Name, authenticationHeader, CancellationToken.None);
            Log.Information("Assigned successful. Key: {Key};", assignedFile.Key);

            await UploadBuildAsync(new Uri(assignedFile.Url), buildPath, CancellationToken.None);
            Log.Information("Build uploaded successful. Key: {Key};", assignedFile.Key);
            
            var build = await AddBuildAsync(assignedFile.Key, buildReport, authenticationHeader, CancellationToken.None);
            Log.Information("Build added successful. Key: {Key}; Url: {Url};", assignedFile.Key, build.Url);
            
            await SetActiveBuildAsync(build.Id, authenticationHeader, CancellationToken.None);
            Log.Information("Set active build successful. Key: {Key}; Id: {Id};", assignedFile.Key, build.Id);
        });

    private async ValueTask<AccessTokenSchema> Authorization(CancellationToken cancellationToken)
    {
        var urlBuilder = new UriBuilder(CrmBaseUrl);
        urlBuilder.WithPathSegment("crm/auth/sign_in");

        var request = new AuthenticationSchema(CrmUsername, CrmPassword);
        var requestJson = JsonSerializer.Serialize(request);

        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        var response = await httpClient.PostAsync(urlBuilder.Uri, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<AccessTokenSchema>(responseJson);
    }

    private async ValueTask<AssignBuildFileSchema> AssignBuildFileToUrlAsync
    (
        string filename,
        AuthenticationHeaderValue authenticationHeader,
        CancellationToken cancellationToken
    )
    {
        var urlBuilder = new UriBuilder(CrmBaseUrl);
        urlBuilder.WithPathSegment("crm/launcher-builds/presigned-url")
            .WithParameter(nameof(filename), filename);

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

        var response = await httpClient.GetAsync(urlBuilder.Uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<AssignBuildFileSchema>(responseJson);
    }

    private static async ValueTask UploadBuildAsync(Uri url, AbsolutePath buildPath, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(30);

        await using var fileStream = File.OpenRead(buildPath);
        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.android.package-archive");

        using var response = await httpClient.PutAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async ValueTask<BuildSchema> AddBuildAsync
    (
        string key,
        BuildReport buildReport,
        AuthenticationHeaderValue authenticationHeader,
        CancellationToken cancellationToken
    )
    {
        var urlBuilder = new UriBuilder(CrmBaseUrl);
        urlBuilder.WithPathSegment("crm/launcher-build")
            .WithParameter(nameof(key), key);

        var buildOptions = buildReport.BuildOptions;
        var version = buildReport.ApplicationVersion;
        var versionCode = GetVersionCode(buildOptions.LocationPathName);
        var checksum = await GetChecksum(buildOptions.LocationPathName, cancellationToken);

        var currentTag = GetCurrentTagOrEmpty();
        var changelog = string.IsNullOrEmpty(currentTag) == false 
            ? GetChangelogBetweenTags(currentTag)
            : "Build";
        
        changelog = changelog.Length > 2048 
            ? changelog[..2048]
            : changelog;

        var request = new AddBuildSchema(changelog, version, versionCode, checksum, false);
        var requestJson = JsonSerializer.Serialize(request);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
        
        var response = await httpClient.PostAsync(urlBuilder.Uri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<BuildSchema>(responseJson);
    }

    private static long GetVersionCode(AbsolutePath buildPath)
    {
        using var zip = ZipFile.OpenRead(buildPath);
        var manifestEntry = zip.GetEntry("AndroidManifest.xml");
        if (manifestEntry == null)
            throw new InvalidOperationException("AndroidManifest.xml not found");
        
        using var stream = manifestEntry.Open();
        using var reader = new AndroidXmlReader(stream);
        var document = XDocument.Load(reader);
        
        var manifest = document.Root;
        if (manifest == null || manifest.Name.LocalName != "manifest")
            throw new InvalidOperationException("Invalid AndroidManifest.xml");
        
        XNamespace androidNs = "http://schemas.android.com/apk/res/android";
        var versionCodeAttribute = manifest.Attribute(androidNs + "versionCode");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if(versionCodeAttribute is null)
            throw new InvalidOperationException("versionCode attribute not found");
        
        return Convert.ToInt64(versionCodeAttribute.Value);
    }

    private static async ValueTask<string> GetChecksum(AbsolutePath buildPath, CancellationToken cancellationToken)
    {
        await using var buildFileStream = new FileStream(buildPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var crc32 = new Crc32();
        await crc32.AppendAsync(buildFileStream, cancellationToken);
        var buildFileChecksum = BitConverter.ToInt32(crc32.GetCurrentHash());
        return buildFileChecksum.ToString("x");
    }

    private async ValueTask SetActiveBuildAsync(long id, AuthenticationHeaderValue authenticationHeader, CancellationToken cancellationToken)
    {
        var urlBuilder = new UriBuilder(CrmBaseUrl);
        urlBuilder.WithPathSegment("crm/launcher-builds")
            .WithPathSegment(id.ToString())
            .WithPathSegment("set-active");
        
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
        
        var response = await httpClient.PatchAsync(urlBuilder.Uri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}