using System;

namespace Sotbifay.Images
{
    public class ImagePoolLoadException : Exception
    {
        public ImagePoolLoadException(string message) : base(message)
        {
        }

        public ImagePoolLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}