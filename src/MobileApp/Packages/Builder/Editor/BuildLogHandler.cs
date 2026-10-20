using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sotbifay.Builder.Editor
{
    public sealed class BuildLogHandler : ILogHandler
    {
        private readonly List<string> _errors = new();
        private readonly ILogHandler _parentLogHandler;

        public BuildLogHandler(ILogHandler parentLogHandler)
        {
            _parentLogHandler = parentLogHandler ?? throw new ArgumentNullException(nameof(parentLogHandler));
        }

        public IReadOnlyList<string> Errors => _errors;

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (logType is LogType.Error or LogType.Exception or LogType.Assert) 
                _errors.Add(string.Format(format, args));
            
            _parentLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            _errors.Add(exception.Message);
            _parentLogHandler.LogException(exception, context);
        }
    }
}