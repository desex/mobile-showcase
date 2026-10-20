using System;
using System.IO;
using System.Text;
using Best.HTTP;
using Best.HTTP.Request.Upload;
using Best.HTTP.Shared.PlatformSupport.Memory;
using Best.HTTP.Shared.Streams;

namespace Sotbify.MobileApp.Server.HttpClient.Implementations
{
    public sealed class RawJsonDataStream : UploadStreamBase
    {
        private string _json;

        public override long Length => _innerStream.Length;

        private BufferPoolMemoryStream _innerStream;

        public RawJsonDataStream(string json)
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
        }
        
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override void BeforeSendHeaders(HTTPRequest request)
        {
            if (_innerStream != null)
            {
                _innerStream.Position = 0;
                return;
            }

            var byteLength = Encoding.UTF8.GetByteCount(_json);
            var buffer = BufferPool.Get(byteLength, true);
            Encoding.UTF8.GetBytes(_json, 0, _json.Length, buffer, 0);

            _innerStream = new BufferPoolMemoryStream(byteLength);
            _innerStream.Write(buffer, 0, byteLength);
            _innerStream.Position = 0;

            BufferPool.Release(buffer);
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            _innerStream.Read(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _json = string.Empty;
            _innerStream?.Dispose();
            _innerStream = null;
        }

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
    }
}