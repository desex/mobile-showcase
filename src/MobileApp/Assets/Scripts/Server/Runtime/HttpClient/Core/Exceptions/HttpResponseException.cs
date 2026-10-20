using System;

namespace Sotbify.MobileApp.Server.HttpClient.Core.Exceptions
{
    public sealed class HttpResponseException : Exception
    {
        public int StatusCode { get; }

        public HttpResponseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}