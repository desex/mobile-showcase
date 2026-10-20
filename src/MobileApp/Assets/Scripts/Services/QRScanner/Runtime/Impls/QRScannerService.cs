using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Db.ScanParameters;
using Sotbify.MobileApp.Services.QRScanner.Utils;
using UnityEngine;
using ZXing;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Services.QRScanner
{
    public class QRScannerService : IQRScannerService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly BarcodeReaderGeneric _qrReader;
        private readonly IScanWindowParameters _scanWindowParameters;

        private WebCamTexture _webCamTexture;
        private Color32LuminanceSource _luminanceSource;
        private Color32[] _colors = Array.Empty<Color32>();

        public QRScannerService(
            ILogger logger,
            BarcodeReaderGeneric qrReader,
            IScanWindowParameters scanWindowParameters
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _qrReader = qrReader ?? throw new ArgumentNullException(nameof(qrReader));
            _scanWindowParameters = scanWindowParameters ?? throw new ArgumentNullException(nameof(scanWindowParameters));
        }

        public WebCamTexture Start()
        {
            var webCamTexture = GetOrCreateWebCamTexture();

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug(
                    $"Web texture created, requested: {webCamTexture.requestedWidth}x{webCamTexture.requestedHeight} real: {webCamTexture.width}x{webCamTexture.height}");

            _webCamTexture = webCamTexture;
            webCamTexture.Play();
            return webCamTexture;
        }

        public async UniTask<string> ScanAsync(CancellationToken token = default)
        {
            if (_webCamTexture == null)
                throw new InvalidOperationException("Texture is not initialized");

            while (token.IsCancellationRequested == false)
            {
                if (_webCamTexture == null)
                    break;

                var arraySize = _webCamTexture.width * _webCamTexture.height;
                if (arraySize != _colors.Length)
                    Array.Resize(ref _colors, arraySize);

                _webCamTexture.GetPixels32(_colors);

                if (_luminanceSource is null || _luminanceSource.Width != _webCamTexture.width ||
                    _luminanceSource.Height != _webCamTexture.height)
                {
                    _luminanceSource = new Color32LuminanceSource(_webCamTexture.width, _webCamTexture.height);
                }

                _luminanceSource.SetPixels(_colors);

                if (_qrReader == null)
                {
                    _logger.Error($"{nameof(_qrReader)} is null", context: this);
                    continue;
                }

                var result = _qrReader.Decode(_luminanceSource);

                if (result != null && string.IsNullOrEmpty(result.Text) == false)
                {
                    _logger.Error($"Scan result: {result.Text}", context: this);
                    return result.Text;
                }

                await UniTask.WaitForSeconds(_scanWindowParameters.ScanDelay, cancellationToken: token,
                    ignoreTimeScale: true);
            }

            return string.Empty;
        }

        public void Stop()
        {
            if (_webCamTexture == null || !_webCamTexture.isPlaying)
                return;

            try
            {
                if (_webCamTexture == null)
                    return;
                _webCamTexture.Stop();
                UnityEngine.Object.Destroy(_webCamTexture);
                _webCamTexture = null;
            }
            finally
            {
                _logger.Debug("Scan stopped", context: this);
            }
        }

        private WebCamTexture GetOrCreateWebCamTexture()
        {
            if (_webCamTexture != null)
                return _webCamTexture;

            var devices = WebCamTexture.devices;

            if (devices.Length == 0)
                throw new NullReferenceException("No camera detected");

            var chosenDevice = ChooseDevice(devices);

            var maxResolution = _scanWindowParameters.MaxCameraResolution;
            var width = Mathf.Min(maxResolution.x, chosenDevice.Resolution?.width ?? Screen.width);
            var height = Mathf.Min(maxResolution.y, chosenDevice.Resolution?.height ?? Screen.height);

            var webCamTexture = new WebCamTexture(chosenDevice.WebCamDevice.name, width, height);


            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug(
                    $"Web camera created: {chosenDevice.WebCamDevice.name};{Environment.NewLine}Resolution: {width}x{height};",
                    context: this);

            return webCamTexture;
        }

        private static Device ChooseDevice(WebCamDevice[] devices)
        {
            var bestDevice = new Device();

            foreach (var device in devices)
            {
#if UNITY_EDITOR == false
                    if(device.isFrontFacing)
                        continue;
#endif

                if (device.availableResolutions == null)
                {
                    bestDevice.WebCamDevice = device;
                    bestDevice.Resolution = null;
                    break;
                }

                foreach (var resolution in device.availableResolutions)
                {
                    var totalPixels = resolution.width * resolution.height;
                    var maxResolution = 0;
                    if (bestDevice.Resolution != null)
                        maxResolution = bestDevice.Resolution.Value.width * bestDevice.Resolution.Value.height;
                    if (totalPixels <= maxResolution)
                        continue;

                    bestDevice.WebCamDevice = device;
                    bestDevice.Resolution = resolution;
                }
            }

            return bestDevice;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}