using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;
using Sotbify.MobileApp.Utils.Schemas.Notifications;

namespace Sotbify.MobileApp.Server
{
    public sealed partial  class Client
    {
        public async UniTask<NotificationsLogResponseSchema> GetNotificationsHistory(int? cursor, int limit, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var requestUnbilded = requestBuilder
                    .SetMethod(HttpMethod.Get)
                    .SetParameter("limit", limit.ToString())
                    .SetPath(GetPath("NotificationLog"));

                if (cursor.HasValue)
                    requestUnbilded.SetParameter("offset", cursor.ToString());

                var request = requestUnbilded.Create();

                var response = await _server.SendAsync(SavedQR, request, cancellationToken);
                return await _serializer.DeserializeAsync<NotificationsLogResponseSchema>(response.DataAsText,
                    cancellationToken);
            }
        }
        
        public async UniTask ReadNotifications(int lastId, CancellationToken cancellationToken = default)
        {
            using (_httpRequestBuildersPool.Get(out var requestBuilder))
            {
                var schema = new NotificationRequestSchema()
                {
                    LastNotificationId = lastId
                };
                
                var requestBody = await _serializer.SerializeAsync(schema, cancellationToken);
                
                var request = requestBuilder
                    .SetMethod(HttpMethod.Post)
                    .SetPath(GetPath("ReadNotification"))
                    .SetBody(requestBody)
                    .SetJsonContentType()
                    .Create();
                
                var isCompleted = await _server.SendAsync(SavedQR, request, cancellationToken);
                
                if (_logger.IsEnabled(LogLevel.Information) && isCompleted.IsSuccess)
                    _logger.Information("Notification read");
            }
        }
    }
}