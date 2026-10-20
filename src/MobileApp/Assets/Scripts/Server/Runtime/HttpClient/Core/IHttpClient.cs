using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Server.HttpClient.Core;
using Sotbify.MobileApp.Server.HttpClient.Core.Types;

namespace Sotbify.MobileApp.Server
{
    public interface IHttpClient
    {
        event Action OnRequestTimeout;
        event Action OnRequestSucceed;
        
        /// <summary>
        /// Асинхронно отправляет HTTP-запрос и возвращает полученный ответ
        /// </summary>
        /// <param name="request">Описание HTTP-запроса</param>
        /// <param name="force">Отправка в обход семафора</param>
        /// <param name="cancellationToken">Токен отмены, позволяющий прервать операцию отправки запроса и ожидания ответа</param>
        /// <returns>Экземпляр <see cref="HttpResponse"/>, содержащий статус, заголовки и данные HTTP-ответа</returns>
        UniTask<HttpResponse> SendAsync(HttpRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Асинхронно выполняет HTTP-запрос и скачивает тело ответа в указанный поток
        /// </summary>
        /// <param name="request">Описание HTTP-запроса</param>
        /// <param name="stream">Поток, в который будет записано содержимое ответа. Не может быть <c>null</c></param>
        /// <param name="progress">Прогресс скачивания</param>
        /// <param name="cancellationToken">Токен отмены, позволяющий прервать операцию скачивания</param>
        /// <returns>Экземпляр <see cref="HttpResponse"/>, содержащий статус и метаданные HTTP-ответа</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="stream"/> является <c>null</c></exception>
        UniTask<HttpResponse> DownloadAsync
        (
            HttpRequest request,
            Stream stream,
            IProgress<DownloadReport> progress = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Сбрасывает все активные соединения, прерывая все текущие запросы и очищая внутренние ресурсы,
        /// связанные с сетевыми соединениями. Все запросы, находящиеся в процессе выполнения, будут отменены.
        /// </summary>
        UniTask ResetConnectionsAsync(CancellationToken cancellationToken = default);
    }
}