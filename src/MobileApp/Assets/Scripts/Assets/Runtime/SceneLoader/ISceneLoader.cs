using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Sotbifay.MobileApp.Assets
{
    public interface ISceneLoader
    {
        UniTask<SceneInstance> LoadSceneAsync
        (
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool activeOnLoad = true,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        );

        UniTask UnloadSceneAsync
        (
            SceneInstance sceneInstance,
            UnloadSceneOptions options = UnloadSceneOptions.None,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        );
    }
}