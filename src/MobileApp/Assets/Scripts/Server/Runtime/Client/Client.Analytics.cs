using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using Sotbify.MobileApp.Utils.Schemas;
using Sotbify.MobileApp.Utils.Schemas.Firebase;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.Server
{
    public partial class Client
    {
        public async UniTask NotificationOpened(string uuid, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var schema = new NotificationUUIDSchema()
                {
                    Uuid = uuid
                };
                
                var requestBody = await _serializer.SerializeAsync(schema, cancellationToken);
                
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("NotificationOpen"))
                    .SetBody(requestBody)
                    .SetJsonContentType()
                    .Create();
                
                var isCompleted = await _server.SendAsync(SavedQR, request, cancellationToken);
                
                if (_logger.IsEnabled(LogLevel.Information) && isCompleted.IsSuccess)
                    _logger.Information("Sended notification uuid", context: this);
            }
        }
        
        public async UniTask<bool> SendAnalyticsEventAsync
        (
            string eventName,
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentException("Cannot be null or empty", nameof(eventName));

            parameters ??= Enumerable.Empty<KeyValuePair<string, object>>();    

            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            using (parameters.ToPooledDictionary(out var parametersDictionary))
            {

                foreach (var (key, value) in parametersDictionary)
                {
                    _logger.Warning($"Key: {key} Value: {value}", context: this);
                }

                if (parametersDictionary.TryGetValue("notificationUUID", out var notificationUUID) == false)
                    return false;
                
                var requestSchema = new AnalyticsEventSchema
                {
                    Action = eventName,
                    NotificationUUID = notificationUUID.ToString(),
                };

                var requestBody = await _serializer.SerializeAsync(requestSchema, cancellationToken);
                requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("NotificationAction"))
                    .SetJsonContentType()
                    .SetBody(requestBody);

                var request = requestBuilder.Create();
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                
                if (response.IsSuccess)
                    _logger.Information("Send notification action", context: this);
                else
                    _logger.Information("Failed to send notification action", context: this);
                
                return response.IsSuccess;
            }
        }

        public async UniTask<bool> SendAnalyticsEventsAsync
        (
            IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, object>>>> events,
            CancellationToken cancellationToken = default
        )
        {
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            using (ListPool<AnalyticsEventSchema>.Get(out var eventsList))
            {
                foreach (var (eventName, parameters) in events)
                {
                    var parametersDictionary = DictionaryPool<string, object>.Get();
                    if (parameters is not null)
                    {
                        foreach (var (parameterName, parameterValue) in parameters)
                            parametersDictionary[parameterName] = parameterValue;
                    }
                        
                    if (parametersDictionary.TryGetValue("notificationUUID", out var notificationUUID) == false)
                        return false;
                    
                    var eventSchema = new AnalyticsEventSchema
                    {
                        Action = eventName,
                        NotificationUUID = notificationUUID.ToString()
                    };
                    
                    eventsList.Add(eventSchema);
                }

                var requestBody = await _serializer.SerializeAsync(eventsList, cancellationToken);
                requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("NotificationAction"))
                    .SetJsonContentType()
                    .SetBody(requestBody);
                
                var request = requestBuilder.Create();
                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                    
                if (response.IsSuccess)
                    _logger.Information("Send notification action", context: this);
                else
                    _logger.Information("Failed to send notification action", context: this);
                    
                return response.IsSuccess;
            }
        }
    }
}