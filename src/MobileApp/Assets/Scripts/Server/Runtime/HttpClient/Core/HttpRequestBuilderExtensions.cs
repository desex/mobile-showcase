using System;

namespace Sotbify.MobileApp.Server.HttpClient.Core
{
    public static class HttpRequestBuilderExtensions
    {
        public static HttpRequestBuilder SetJsonContentType(this HttpRequestBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.SetHeader("Content-Type", "application/json");
            return builder;
        }
        
        public static HttpRequestBuilder SetAuthorization(this HttpRequestBuilder builder, string tokenType, string accessToken)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            
            if (string.IsNullOrEmpty(tokenType))
                throw new ArgumentException("Value cannot be null or empty", nameof(tokenType));
            
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("Value cannot be null or empty", nameof(accessToken));

            builder.SetHeader("Authorization", $"{tokenType} {accessToken}");
            return builder;
        }
    }
}