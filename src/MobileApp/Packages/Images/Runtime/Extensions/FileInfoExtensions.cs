using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Sotbifay.Images
{
    public static class FileInfoExtensions
    {
        public static async UniTask<Texture2D> ReadAsTextureOrNullAsync
        (
            this FileInfo imageFileInfo,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (imageFileInfo is null)
                throw new ArgumentNullException(nameof(imageFileInfo));
            
            var fileUrl = new Uri(imageFileInfo.FullName);
            using var textureRequest = UnityWebRequestTexture.GetTexture(fileUrl.AbsoluteUri);
            var response = await textureRequest.SendWebRequest()
                .ToUniTask(progress: progress, cancellationToken: cancellationToken);

            return response.result is UnityWebRequest.Result.Success
                ? DownloadHandlerTexture.GetContent(textureRequest)
                : null;
        }
    }
}