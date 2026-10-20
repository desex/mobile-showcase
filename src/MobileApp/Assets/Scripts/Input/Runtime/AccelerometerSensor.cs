using System;
using Sotbifay.MobileApp.Logger;
using UnityEngine.InputSystem;

namespace Sotbify.MobileApp.Input
{
    public sealed class AccelerometerSensor : IDisposable
    {
        private readonly ILogger _logger;

        public AccelerometerSensor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsActive { get; private set; }

        public void Enable()
        {
            if (IsActive)
                return;
            
            IsActive = true;
            
            if(Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
            
            if(LinearAccelerationSensor.current != null) 
                InputSystem.EnableDevice(LinearAccelerationSensor.current);
            
            if(_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Accelerometer enabled");
        }

        public void Disable()
        {
            if (IsActive == false)
                return;
            
            IsActive = false;

            try
            {
                InputSystem.DisableDevice(Accelerometer.current);
            }
            catch (Exception e)
            {
                if(_logger.IsEnabled(LogLevel.Warning))
                    _logger.Warning("Failed to disable accelerometer sensor", e, this);
            }
            
            try
            {
                InputSystem.DisableDevice(LinearAccelerationSensor.current);
            }
            catch (Exception e)
            {
                if(_logger.IsEnabled(LogLevel.Warning))
                    _logger.Warning("Failed to disable linear acceleration sensor", e, this);
            }
            
            if(_logger.IsEnabled(LogLevel.Information))
                _logger.Information("Accelerometer disabled");
        }

        public void Dispose()
        {
            Disable();
        }
    }
}