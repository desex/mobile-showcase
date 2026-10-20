using System;
using System.Collections.Generic;
using System.Text;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;

namespace Sotbify.MobileApp.Server.HttpClient.Core
{
    public readonly struct HttpRequest
    {
        public HttpRequest
        (
            HttpMethod method,
            Uri url,
            string path,
            IEnumerable<KeyValuePair<string, string>> parameters,
            IEnumerable<KeyValuePair<string, string>> headers,
            string body
        )
        {
            Method = method;
            Url = url;
            Path = path ?? string.Empty;
            Parameters = new List<KeyValuePair<string,string>>(parameters);
            Headers = new Dictionary<string, string>(headers);
            Body = body ?? string.Empty;
        }

        public HttpMethod Method { get; }
        public Uri Url { get; }
        public string Path { get; }
        public IReadOnlyList<KeyValuePair<string, string>> Parameters { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public string Body { get; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder
                .Append("Method: " ).Append(Method).AppendLine()
                .Append("URL: " ).Append(Url).AppendLine()
                .Append("Path: " ).Append(Path).AppendLine();
            
            if (Parameters.Count > 0)
            {
                stringBuilder.Append("Parameters: ").AppendLine();
                foreach (var (key, value) in Parameters) 
                    stringBuilder.Append('\t').Append(key).Append(" = ").Append(value).AppendLine();
            }
            
            if (Headers.Count > 0)
            {
                stringBuilder.Append("Headers: ").AppendLine();
                foreach (var (key, value) in Headers) 
                    stringBuilder.Append('\t').Append(key).Append(" = ").Append(value).AppendLine();
            }
            
            stringBuilder.Append("Body: ").Append(Body);
                
            return stringBuilder.ToString();
        }
    }
}