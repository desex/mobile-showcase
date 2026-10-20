using System;
using Sotbifay.MobileApp.Logger;
using UnityEngine.Localization.Settings;

namespace Sotbify.MobileApp.Localization
{
    public sealed class StringLocalizer : IStringLocalizer
    {
        private readonly ILogger _logger;

        public StringLocalizer(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetLocalizedString(string tableName, string key)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));
            
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var result = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);

            if (string.IsNullOrEmpty(result))
                _logger.Warning($"Missing key: {tableName}.{key}", context: this);

            return result;
        }
    }
}