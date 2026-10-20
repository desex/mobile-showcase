using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Sotbifay.Images
{
    public static class UriExtensions
    {
        public static async UniTask DownloadToStreamAsync
        (
            this Uri url,
            Stream stream,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (url is null)
                throw new ArgumentNullException(nameof(url));
            
            if(stream is null)
                throw new ArgumentNullException(nameof(stream));
            
            using var request = UnityWebRequest.Get(url);
            using var downloadHandlerStream = new DownloadHandlerStream(stream);
            request.downloadHandler = downloadHandlerStream;

            var response = await request.SendWebRequest()
                .ToUniTask(progress: progress, cancellationToken: cancellationToken);

            if (response.result != UnityWebRequest.Result.Success)
                throw new Exception($"Failed to download. Url: {url}, Error: {response.error};");
        }
        
        private sealed class DownloadHandlerStream : DownloadHandlerScript
        {
            private readonly Stream _stream;

            public DownloadHandlerStream(Stream stream, long bufferSize = 1024 * 64) : base(new byte[bufferSize])
            {
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            }

            protected override bool ReceiveData(byte[] receivedData, int dataLength)
            {
                _stream.Write(receivedData, 0, dataLength);
                return true;
            }
        }
    }
}