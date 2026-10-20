using System;
using System.Text;

namespace Sotbify.MobileApp.Server.HttpClient.Core
{
    public readonly struct HttpResponse
    {
        public HttpResponse
        (
            int statusCode,
            byte[] data,
            string dataAsText,
            string error
        ) : this(statusCode, data?.AsMemory() ?? ReadOnlyMemory<byte>.Empty, dataAsText, error)
        {
        }
        
        public HttpResponse
        (
            int statusCode,
            ReadOnlyMemory<byte> data,
            string dataAsText,
            string error
        )
        {
            StatusCode = statusCode;
            Data = data;
            DataAsText = dataAsText ?? string.Empty;
            Error = error ?? string.Empty;
        }

        public int StatusCode { get; }
        public ReadOnlyMemory<byte> Data { get; }
        public string DataAsText { get; }
        public string Error { get; }

        public bool IsSuccess => StatusCode is >= 200 and < 300;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder
                .Append("Is success: ").Append(IsSuccess).AppendLine()
                .Append("StatusCode: ").Append(StatusCode).AppendLine()
                .Append("Data: ").Append(DataAsText).AppendLine()
                .Append("Error: ").Append(Error);
                
            return stringBuilder.ToString();
        }
    }
}