using System;
using System.Collections.Generic;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Server.HttpClient.Core
{
    public sealed class HttpRequestBuilder
    {
        private readonly Dictionary<string, string> _headers = new();
        private readonly List<KeyValuePair<string, string>> _parameters = new();

        private HttpMethod _method = HttpMethod.Get;
        private Uri _url;
        private string _path = string.Empty;
        private string _body = string.Empty;

        public HttpRequest Create()
        {
            return new HttpRequest
            (
                _method,
                _url,
                _path,
                _parameters,
                _headers,
                _body
            );
        }

        public HttpRequestBuilder CopyFrom(HttpRequest request)
        {
            _url = request.Url;
            _path = request.Path;
            _method = request.Method;
            _body = request.Body;

            _headers.Clear();
            foreach (var key in request.Headers.Keys)
                _headers[key] = request.Headers[key];
            
            _parameters.Clear();
            _parameters.AddRange(request.Parameters);

            return this;
        }

        public HttpRequestBuilder SetMethod(HttpMethod method)
        {
            _method = method;
            return this;
        }

        public HttpRequestBuilder SetUrl(Uri url)
        {
            if (url is null)
                throw new ArgumentNullException(nameof(url));

            if (url.IsAbsoluteUri == false)
                throw new ArgumentException("Must be absolute url", nameof(url));

            _url = url;
            return this;
        }

        public HttpRequestBuilder SetPath(Uri path)
        {
            if (path is not null && path.IsAbsoluteUri)
                throw new ArgumentException("Must be relative", nameof(path));

            return SetPath(path?.ToString());
        }

        public HttpRequestBuilder SetPath(string path)
        {
            _path = path ?? string.Empty;
            return this;
        }

        public HttpRequestBuilder SetParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cannot be null or empty", nameof(key));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Cannot be null or empty", nameof(value));
            
            using (ListPool<KeyValuePair<string, string>>.Get(out var tempParameters))
            {
                foreach (var pair in _parameters)
                {
                    if(pair.Key == key)
                        continue;
                    
                    tempParameters.Add(pair);
                }
                
                _parameters.Clear();
                _parameters.AddRange(tempParameters);
            }
            
            _parameters.Add(new KeyValuePair<string, string>(key, value));
            
            return this;
        }
        
        public HttpRequestBuilder AddParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cannot be null or empty", nameof(key));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Cannot be null or empty", nameof(value));
            
            _parameters.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public HttpRequestBuilder SetHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cannot be null or empty", nameof(key));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Cannot be null or empty", nameof(value));

            _headers[key] = value;
            return this;
        }

        public HttpRequestBuilder SetBody(string body)
        {
            _body = body ?? string.Empty;
            return this;
        }

        public void Clear()
        {
            _method = HttpMethod.Get;
            _url = null;
            _path = string.Empty;
            _body = string.Empty;

            _headers.Clear();
            _parameters.Clear();
        }
    }
}