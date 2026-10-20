using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Nuke.Common.IO;

public partial class Build
{
    private static AbsolutePath FindXcodeExportDirectory()
    {
        var pbxprojFile = ArtifactsDirectory
            .GlobFiles("**/*.xcodeproj/project.pbxproj")
            .OrderByDescending(GetLastWriteTimeUtc)
            .FirstOrDefault();

        return pbxprojFile == null ? 
            throw new FileNotFoundException($"Xcode project was not found in artifacts directory: {ArtifactsDirectory}") 
            : pbxprojFile.Parent!.Parent;
    }
    
    private static DateTime GetLastWriteTimeUtc(AbsolutePath path)
    {
        return File.GetLastWriteTimeUtc(path.ToString());
    }

    private static void PatchInfoPlist(AbsolutePath infoPlistPath)
    {
        if (File.Exists(infoPlistPath) == false)
            throw new FileNotFoundException($"Info.plist was not found: {infoPlistPath}");

        var document = XDocument.Load(infoPlistPath, LoadOptions.PreserveWhitespace);
        var dict = GetPlistDict(document);

        var backgroundModesArray = GetOrCreateArrayValue(dict, "UIBackgroundModes");
        var hasRemoteNotification = backgroundModesArray
            .Elements("string")
            .Any(element => element.Value == "remote-notification");

        if (!hasRemoteNotification)
            backgroundModesArray.Add(new XElement("string", "remote-notification"));

        SetBooleanValue(dict, "FirebaseMessagingAutoInitEnabled", false);

        SavePlist(document, infoPlistPath);
    }

    private void PatchEntitlements(AbsolutePath entitlementsPath)
    {
        var document = File.Exists(entitlementsPath)
            ? XDocument.Load(entitlementsPath, LoadOptions.PreserveWhitespace)
            : CreateEmptyPlistDocument();

        var dict = GetPlistDict(document);

        SetStringValue(dict, "aps-environment", IOSApsEnvironment);

        SavePlist(document, entitlementsPath);
    }

    private static XDocument CreateEmptyPlistDocument()
    {
        return new XDocument(
            new XElement("plist",
                new XAttribute("version", "1.0"),
                new XElement("dict")));
    }

    private static XElement GetPlistDict(XDocument document)
    {
        var dict = document.Root?.Element("dict");

        return dict ?? throw new InvalidOperationException("Invalid plist format: root dict element was not found.");
    }

    private static XElement GetOrCreateArrayValue(XElement dict, string key)
    {
        var keyElement = FindKeyElement(dict, key);

        if (keyElement == null)
        {
            var array = new XElement("array");

            dict.Add(new XElement("key", key));
            dict.Add(array);

            return array;
        }

        var valueElement = GetValueElement(keyElement);

        if (valueElement?.Name == "array")
            return valueElement;

        valueElement?.Remove();

        var newArray = new XElement("array");
        keyElement.AddAfterSelf(newArray);

        return newArray;
    }

    private static void SetStringValue(XElement dict, string key, string value)
    {
        var keyElement = FindKeyElement(dict, key);

        if (keyElement == null)
        {
            dict.Add(new XElement("key", key));
            dict.Add(new XElement("string", value));
            return;
        }

        var valueElement = GetValueElement(keyElement);

        if (valueElement == null)
        {
            keyElement.AddAfterSelf(new XElement("string", value));
            return;
        }

        if (valueElement.Name == "string")
        {
            valueElement.Value = value;
            return;
        }

        valueElement.ReplaceWith(new XElement("string", value));
    }

    private static void SetBooleanValue(XElement dict, string key, bool value)
    {
        var keyElement = FindKeyElement(dict, key);
        var elementName = value ? "true" : "false";

        if (keyElement == null)
        {
            dict.Add(new XElement("key", key));
            dict.Add(new XElement(elementName));
            return;
        }

        var valueElement = GetValueElement(keyElement);

        if (valueElement == null)
        {
            keyElement.AddAfterSelf(new XElement(elementName));
            return;
        }

        if (valueElement.Name == elementName)
            return;

        valueElement.ReplaceWith(new XElement(elementName));
    }

    private static XElement FindKeyElement(XElement dict, string key)
    {
        return dict
            .Elements("key")
            .FirstOrDefault(element => element.Value == key);
    }

    private static XElement GetValueElement(XElement keyElement)
    {
        return keyElement
            .ElementsAfterSelf()
            .FirstOrDefault(element => element.Name != "key");
    }

    private static void SavePlist(XDocument document, AbsolutePath path)
    {
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));

        writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        writer.WriteLine("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");

        using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true,
            Encoding = new UTF8Encoding(false),
        });

        document.Root.Save(xmlWriter);
        writer.WriteLine();
    }
}