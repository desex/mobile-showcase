using System;

namespace Sotbify.MobileApp.TextureProvider
{
    public class TextureProviderException : Exception
    {
        public TextureProviderException(string message) : base(message)
        {
        }
        
        public TextureProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}