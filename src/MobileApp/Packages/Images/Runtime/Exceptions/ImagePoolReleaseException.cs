using System;

namespace Sotbifay.Images
{
    public class ImagePoolReleaseException : Exception
    {
        public ImagePoolReleaseException(string message) : base(message)
        {
        }

        public ImagePoolReleaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}