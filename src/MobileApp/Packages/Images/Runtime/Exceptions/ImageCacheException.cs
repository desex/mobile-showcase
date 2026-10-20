using System;

namespace Sotbifay.Images
{
    public class ImageCacheException : Exception
    {
        public ImageCacheException(string message) : base(message)
        {
        }

        public ImageCacheException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}