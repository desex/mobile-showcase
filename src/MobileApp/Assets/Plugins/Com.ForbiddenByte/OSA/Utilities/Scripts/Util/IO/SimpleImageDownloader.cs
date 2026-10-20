// WWW class usage
#pragma warning disable 0618

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Com.ForbiddenByte.OSA.Util.IO
{
    /// <summary>
    /// <para>A utility singleton class for downloading images using a LIFO queue for the requests. <see cref="MaxConcurrentRequests"/> can be used to limit the number of concurrent requests. </para> 
    /// <para>Default is <see cref="DEFAULT_MAX_CONCURRENT_REQUESTS"/>. Each request is executed immediately if there's room for it. When the queue is full, the downloader checks each frame if a slot was freed, after which re-enters the loop.</para>
    /// </summary>
    public class SimpleImageDownloader : MonoBehaviour
    {
        public static SimpleImageDownloader Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject(typeof(SimpleImageDownloader).Name).AddComponent<SimpleImageDownloader>();

                return _Instance;
            }
        }

        static SimpleImageDownloader _Instance;


        public int MaxConcurrentRequests { get; set; }

        const int DEFAULT_MAX_CONCURRENT_REQUESTS = 20;

        List<Request> _QueuedRequests = new List<Request>();
        List<Request> _ExecutingRequests = new List<Request>();


        IEnumerator Start()
        {
            if (MaxConcurrentRequests == 0)
                MaxConcurrentRequests = DEFAULT_MAX_CONCURRENT_REQUESTS;

            while (true)
            {
                while (_ExecutingRequests.Count >= MaxConcurrentRequests)
                {
                    // Checking each frame, so a freed slot is reused right away
                    yield return null;
                }

                int lastIndex = _QueuedRequests.Count - 1;
                if (lastIndex >= 0)
                {
                    var lastRequest = _QueuedRequests[lastIndex];
                    _QueuedRequests.RemoveAt(lastIndex);

                    StartCoroutine(DownloadCoroutine(lastRequest));
                }

                yield return null;
            }
        }

		void OnDestroy()
		{
			_Instance = null;
		}

		public void Enqueue(Request request)
        { _QueuedRequests.Add(request); }

        IEnumerator DownloadCoroutine(Request request)
        {
            _ExecutingRequests.Add(request);
            UnityWebRequest www = null;
            try
            {
                UnityWebRequestAsyncOperation operation = null;
                try
                {
                    www = UnityWebRequestTexture.GetTexture(request.url);
                    operation = www.SendWebRequest();
                }
                catch (Exception e)
                {
                    // E.g. plain HTTP is disallowed in Player Settings. Reported through onError below
                    Debug.LogException(e);
                }

                // If sending failed, operation is null and this only waits a frame, so callbacks are never called from inside Start()'s loop
                yield return operation;

                if (operation != null && string.IsNullOrEmpty(www.error))
                {
                    if (request.onDone != null)
                    {
                        var result = new Result(www);
                        request.onDone(result);
                    }
                }
                else
                {
                    if (request.onError != null)
                        request.onError();
                }
            }
            finally
            {
                // Also reached if a callback throws. Otherwise the request would never be disposed and its slot would stay taken forever
                if (www != null)
                    www.Dispose();
                _ExecutingRequests.Remove(request);
            }
        }

        public class Request
        {
            public string url;
            public Action<Result> onDone;
            public Action onError;
        }

        public class Result
        {
            UnityEngine.Networking.UnityWebRequest _UsedRequest;


			public Result(UnityWebRequest www)
            { _UsedRequest = www; }


            public Texture2D CreateTextureFromReceivedData()
            { return DownloadHandlerTexture.GetContent(_UsedRequest); }

        }
    }
}
#pragma warning restore 0618