using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.Db.ChatParameters;
using Sotbify.MobileApp.Server;
using Sotbify.MobileApp.Services.ServerConstConverter;
using Sotbify.MobileApp.TextureProvider;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions;
using Sotbify.MobileApp.Utils.Schemas.Blackjack;
using Sotbify.MobileApp.Utils.Schemas.Chat;
using UnityEngine;
using UnityEngine.Pool;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ChatMessages
{
    //TODO: Optimize this repository, refactor this
    public class ChatItemsRepository : IChatItemsRepository
    {
        private readonly Client _client;
        private readonly IChatParameters _chatParameters;
        private readonly ITextureProvider _textureProvider;
        private readonly ILogger _logger;
        private readonly IServerConstConverter _serverConstConverter;
        private readonly Subject<int> _scrollTo = new();
        private readonly Subject<Unit> _chatMessageRead = new();
        private readonly Subject<Unit> _cancelActiveBlackjack = new();
        private readonly ObservableList<IChatItemModel> _loadedItems = new();
        private readonly ReactiveProperty<int> _unreadCount = new();
        private readonly ReactiveProperty<bool> _shouldShowLoading = new();
        private readonly SemaphoreSlim _addToEndSemaphore = new(1, 1);
        
        private int? _currentCursor = null;
        private const int Limit = 5;
        private bool _isFirstHistoryLoaded = false;
        private int _lastSeenMessageId = -1;
        private int _lastReadIndex = -1;
        private AsyncLazy _readMessagesTaskLazy;
        private int _cachedUnreadCount;
        private DateTime _lastBlackjackDateTime = DateTime.MinValue;

        public ChatItemsRepository
        (
            Client client,
            IChatParameters chatParameters,
            ITextureProvider textureProvider,
            ILogger logger,
            IServerConstConverter serverConstConverter
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _chatParameters = chatParameters ?? throw new ArgumentNullException(nameof(chatParameters));
            _textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serverConstConverter = serverConstConverter ?? throw new ArgumentNullException(nameof(serverConstConverter));
        }

        public ReadOnlyReactiveProperty<int> UnreadCount => _unreadCount;
        public ReadOnlyReactiveProperty<bool> ShouldShowLoading => _shouldShowLoading;
        public Observable<Unit> CancelActiveBlackjack => _cancelActiveBlackjack;
        public Observable<Unit> ChatMessageRead => _chatMessageRead;
        public IReadOnlyObservableList<IChatItemModel> LoadedItems => _loadedItems;
        public Observable<int> ScrollToObservable => _scrollTo;
        
        public bool IsLoading { get; private set; } = false;

        public async UniTask<bool> TryLoadMoreAsync(CancellationToken cancellationToken = default)
        {
            if (_currentCursor == null && _isFirstHistoryLoaded)
                return false;

            if (_isFirstHistoryLoaded is false)
                _shouldShowLoading.Value = true;

            await LoadMoreAsyncInternal(cancellationToken);
            _shouldShowLoading.Value = false;
            _lastReadIndex = _loadedItems.Count - 1;
            return true;
        }

        public async UniTask LoadMoreAfterReconnectAsync(CancellationToken cancellationToken = default)
        {
            var visitedCursors = new HashSet<int>();
            var loadedItemIds = GetLoadedItemIds();
            int? cursor = null;
            int? lastMessageId = _loadedItems.IsEmpty()
                ? null
                : _loadedItems[^1].ID;

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug($"Loading messages after reconnect from {lastMessageId} id", context: this);

            IsLoading = true;
            try
            {
                var isAllLoaded = false;
                do
                {
                    using (ListPool<IChatItemModel>.Get(out var chatData))
                    {
                        var chatPage =
                            await _client.GetChatHistoryAsync(cursor, cancellationToken: cancellationToken);
                        cursor = chatPage.NextCursor;
                        
                        if (cursor != null && visitedCursors.Add(cursor.Value) == false)
                        {
                            if (_logger.IsEnabled(LogLevel.Warning))
                                _logger.Warning($"Messages sync stopped because cursor loop was detected: {cursor}", context: this);
                            break;
                        }
                        
                        chatPage.Messages = FilterMessages(chatPage.Messages);
                        
                        foreach (var schema in chatPage.Messages)
                        {
                            if (loadedItemIds.Add(schema.Id) == false)
                                continue;
                            
                            var data = await ConvertToMessageData(schema, cancellationToken : cancellationToken);
                            
                            chatData.Add(data);
                        }

                        chatData.Reverse();

                        PrepareBlackjacksBeforeInsert(chatData);
                        AddDatesForAppend(chatData);
                        _loadedItems.AddRange(chatData);
                        isAllLoaded = cursor is null || cursor <= lastMessageId;
                    }
                } while (isAllLoaded == false && cancellationToken.IsCancellationRequested == false);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async UniTask ReadMessagesAsync(CancellationToken cancellationToken = default)
        {
            var unreadCount = await _client.GetUnreadCountAsync(cancellationToken);
            _unreadCount.Value = unreadCount;
            
            if (unreadCount == 0)
                return;
            
            if (_loadedItems.IsEmpty())
                return;
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.Debug("Has unread messages", context: this);
            
            var targetID = _loadedItems[^1].ID;
            _lastReadIndex = _loadedItems.Count - 1;
            
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Information($"Reading messages", context: this);

            await _client.ReadMessageAsync
            (
                targetID,
                cancellationToken
            );
            _unreadCount.Value = _loadedItems.Count - 1 - _lastReadIndex;
        }

        public async UniTask AddToEnd(ChatMessageResponseSchema schema, CancellationToken cancellationToken = default, bool isUser = false)
        {
            await _addToEndSemaphore.WaitAsync(cancellationToken);
            try
            {
                var type = _serverConstConverter.ToEMessageContentType(schema.MessageType);

                var loadedItemIndex = GetLoadedItemIndex(schema.Id);
                if (loadedItemIndex >= 0)
                {
                    // The SSE event can arrive before the HTTP response. In that case
                    // the item is already present, but the user's send still needs to
                    // move the list to the confirmed message.
                    if (isUser)
                        _scrollTo?.OnNext(loadedItemIndex);

                    return;
                }

                if (type == EMessageContentType.BlackjackResult)
                {
                    TryCancelBlackjack(schema);
                    return;
                }

                var data = await ConvertToMessageData(schema, false, true, cancellationToken);

                if (type == EMessageContentType.Blackjack)
                {
                    UpdateLastBlackjackDateTime(schema);
                    TryDeletePreviousBlackjacks();
                }
                /*else if (ContainsLoadedItem(data.ID))
                    return;*/
                
                AddDateBeforeAppendingIfNeeded(data);
                _loadedItems.Add(data);
                
                var lastCollectionItemIndex = _loadedItems.Count - 1;
                if (isUser || lastCollectionItemIndex - _lastSeenMessageId <= 2)
                    _scrollTo?.OnNext(lastCollectionItemIndex);

                if (isUser)
                    _lastReadIndex = lastCollectionItemIndex;

                if (!_isFirstHistoryLoaded)
                    _unreadCount.Value = _cachedUnreadCount + _loadedItems.Count;
                else
                    _unreadCount.Value = _loadedItems.Count - 1 - _lastReadIndex;
            
                await TryReadMessages(lastCollectionItemIndex, cancellationToken);
            }
            finally
            {
                _addToEndSemaphore.Release();
            }
        }

        public void LastSeenMessageUpdated(int itemIndex)
        {
            if (itemIndex < 0)
                return;
            
            _lastSeenMessageId = itemIndex;

            TryReadMessages(_loadedItems.Count - 1)
                .ForgetIgnoreOperationCancelledException(_logger, "Failed to read messages", this);
        }
        
        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            _cachedUnreadCount = await _client.GetUnreadCountAsync(cancellationToken);
            _unreadCount.Value = _cachedUnreadCount;
        }
        
        private void TryDeletePreviousBlackjacks()
        {
            for (var i = _loadedItems.Count - 1; i >= 0 ; i--)
                if (_loadedItems[i].CachedType == typeof(BlackjackModel))
                    _loadedItems.Remove(_loadedItems[i]);
        }
        
        private void TryCancelBlackjack(ChatMessageResponseSchema schema)
        {
            var blackjackResultSchema = JsonConvert.DeserializeObject<BlackjackResultCRMSchema>(schema.Text);
            var result = _serverConstConverter.ToEResult(blackjackResultSchema.Result);
            var sender = _serverConstConverter.ToEChatSender(schema.Sender);
            var isCancelled = result == EGameResult.Cancelled;
            
            if (isCancelled == false)
                return;
            
            for (var i = _loadedItems.Count - 1; i >= 0; i--)
            {
                if (_loadedItems[i].CachedType != typeof(BlackjackModel)) 
                    continue;
                
                _cancelActiveBlackjack.OnNext(Unit.Default);
                return;

            }
        }
        
        private async UniTask TryReadMessages(int lastCollectionItemIndex, CancellationToken cancellationToken = default)
        {
            if (_lastSeenMessageId == lastCollectionItemIndex)
                _chatMessageRead.OnNext(Unit.Default);
            
            if (_lastReadIndex < lastCollectionItemIndex && _lastSeenMessageId == lastCollectionItemIndex)
            {
                _chatMessageRead.OnNext(Unit.Default);
                _readMessagesTaskLazy ??= ReadMessagesAsync(cancellationToken).ToAsyncLazy();
                try
                {
                    await _readMessagesTaskLazy;
                }
                finally
                {
                    _readMessagesTaskLazy = null;
                }
            }
        }

        private async UniTask LoadMoreAsyncInternal(CancellationToken cancellationToken = default)
        {
            var shouldScrollBottom = false;
            var visitedCursors = new HashSet<int>();
            var loadedItemIds = GetLoadedItemIds();
            IsLoading = true;
            try
            {
                using (ListPool<UniTask<IChatItemModel>>.Get(out var chatData))
                {
                    // ReSharper disable once RedundantAssignment
                    var chatPage = new ChatMessagePaginationSchema();

                    do
                    {
                        chatPage = await _client.GetChatHistoryAsync(_currentCursor, Limit,
                            cancellationToken: cancellationToken);

                        if (_currentCursor == null)
                        {
                            shouldScrollBottom = true;
                            _isFirstHistoryLoaded = true;
                        }

                        _currentCursor = chatPage.NextCursor;

                        if (_currentCursor != null && visitedCursors.Add(_currentCursor.Value) == false)
                        {
                            if (_logger.IsEnabled(LogLevel.Warning))
                                _logger.Warning(
                                    $"History loading stopped because cursor loop was detected: {_currentCursor}",
                                    context: this);
                            _currentCursor = null;
                            break;
                        }

                        chatPage.Messages = FilterMessages(chatPage.Messages);

                        foreach (var schema in chatPage.Messages)
                        {
                            if (loadedItemIds.Add(schema.Id) == false)
                                continue;
                            
                            chatData.Add(ConvertToMessageData(schema, cancellationToken: cancellationToken));
                        }

                    } while (chatData.Count <= Limit && _currentCursor != null);

                    chatData.Reverse();

                    var convertedItems = await UniTask.WhenAll(chatData);

                    using (ListPool<IChatItemModel>.Get(out var items))
                    {
                        items.AddRange(convertedItems);
                        PrepareBlackjacksBeforeInsert(items);
                        AddDatesForPrepend(items);
                        _loadedItems.InsertRange(0, items);
                    }

                    if (shouldScrollBottom)
                        _scrollTo?.OnNext(_loadedItems.Count - 1);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Failed to load messages", e, this);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private ChatMessageResponseSchema[] FilterMessages(ChatMessageResponseSchema[] chatPageMessages)
        {
            var filteredMessageResponseSchema = new List<ChatMessageResponseSchema>();
            
            foreach (var schema in chatPageMessages)
            {
                var messageType = _serverConstConverter.ToEMessageContentType(schema.MessageType);
                
                if (messageType != EMessageContentType.Blackjack) 
                    continue;
                
                var createdAt = DateTime.Parse(schema.CreatedAt, null, DateTimeStyles.RoundtripKind);
                if (createdAt > _lastBlackjackDateTime)
                    _lastBlackjackDateTime = createdAt;
            }

            foreach (var schema in chatPageMessages)
            {
                var messageType = _serverConstConverter.ToEMessageContentType(schema.MessageType);
                
                switch (messageType)
                {
                    case EMessageContentType.BlackjackResult:
                        continue;
                    case EMessageContentType.Blackjack:
                    {
                        var createdAt = DateTime.Parse(schema.CreatedAt, null, DateTimeStyles.RoundtripKind);
                    
                        if (createdAt < _lastBlackjackDateTime)
                            continue;
                        break;
                    }
                }

                filteredMessageResponseSchema.Add(schema);
            }
            
            return filteredMessageResponseSchema.ToArray();
        }

        private void AddDatesForPrepend(List<IChatItemModel> chatItemModels)
        {
            RemoveFirstLoadedDateIfConnectedWith(chatItemModels);
            AddDates(chatItemModels, previousMessageDateTime: null);
        }

        private void AddDatesForAppend(List<IChatItemModel> chatItemModels)
        {
            AddDates(chatItemModels, GetLastLoadedMessageDateTime());
        }

        private void AddDates(List<IChatItemModel> chatItemModels, DateTime? previousMessageDateTime)
        {
            if (chatItemModels.Count == 0)
                return;

            var messagesWithDates = new List<IChatItemModel>();

            foreach (var model in chatItemModels)
            {
                var createdAt = model.CreatedAt;
                if (ShouldShowDateBefore(createdAt, previousMessageDateTime))
                    messagesWithDates.Add(CreateDateModel(createdAt));

                messagesWithDates.Add(model);
                previousMessageDateTime = createdAt;
            }
            
            chatItemModels.Clear();
            chatItemModels.AddRange(messagesWithDates);
        }

        private void AddDateBeforeAppendingIfNeeded(IChatItemModel model)
        {
            var createdAt = model.CreatedAt;
            if (ShouldShowDateBefore(createdAt, GetLastLoadedMessageDateTime()))
                _loadedItems.Add(CreateDateModel(createdAt));
        }

        private void RemoveFirstLoadedDateIfConnectedWith(List<IChatItemModel> prependingItems)
        {
            if (_loadedItems.Count == 0 || prependingItems.Count == 0)
                return;

            var lastPrependingMessageDateTime = GetLastMessageDateTime(prependingItems);
            var firstLoadedMessageIndex = GetFirstMessageIndex(_loadedItems);

            if (lastPrependingMessageDateTime == null || firstLoadedMessageIndex == -1)
                return;

            var firstLoadedMessageDateTime = _loadedItems[firstLoadedMessageIndex].CreatedAt;
            var shouldKeepDateBeforeLoadedMessage = ShouldShowDateBefore(firstLoadedMessageDateTime, lastPrependingMessageDateTime);

            if (shouldKeepDateBeforeLoadedMessage)
            {
                if (firstLoadedMessageIndex == 0)
                    _loadedItems.Insert(0, CreateDateModel(firstLoadedMessageDateTime));

                return;
            }

            while (_loadedItems.Count > 0 && IsDateModel(_loadedItems[0]))
                _loadedItems.RemoveAt(0);
        }

        private DateTime? GetLastLoadedMessageDateTime()
        {
            return GetLastMessageDateTime(_loadedItems);
        }

        private DateTime? GetLastMessageDateTime(IList<IChatItemModel> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                if (IsDateModel(items[i]))
                    continue;

                return items[i].CreatedAt;
            }

            return null;
        }

        private int GetFirstMessageIndex(IList<IChatItemModel> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (IsDateModel(items[i]))
                    continue;

                return i;
            }

            return -1;
        }

        private bool ShouldShowDateBefore(DateTime createdAt, DateTime? previousMessageDateTime)
        {
            if (previousMessageDateTime == null)
                return true;

            return (createdAt - previousMessageDateTime.Value).TotalMinutes >= _chatParameters.DateShowTimeSpan;
        }

        private static DateModel CreateDateModel(DateTime createdAt)
        {
            return new DateModel
            {
                ID = -1,
                FreezeEndEdge = true,
                KeepVelocityOnCountChange = true,
                CreatedAt = createdAt,
            };
        }

        private static bool IsDateModel(IChatItemModel model)
        {
            return model.CachedType == typeof(DateModel);
        }

        private async UniTask<IChatItemModel> ConvertToMessageData(ChatMessageResponseSchema schema, bool freezeEndEdge = true, bool keepVelocityOnCountChange = true, CancellationToken cancellationToken = default)
        {
            var createdAt = DateTime.Parse(schema.CreatedAt, null, DateTimeStyles.RoundtripKind);
            var messageType = _serverConstConverter.ToEMessageContentType(schema.MessageType);

            if (messageType == EMessageContentType.Blackjack)
                return await ConvertToBlackjackModel(schema, freezeEndEdge, keepVelocityOnCountChange, cancellationToken);
                
            var sender = _serverConstConverter.ToEChatSender(schema.Sender);
            Sprite sprite = null;

            if (messageType == EMessageContentType.Image)
                sprite = await _textureProvider.LoadSpriteAsync(new Uri(schema.MediaUrl), cancellationToken: cancellationToken);

            switch (sender)
            {
                case EChatSender.User:
                    return new UserMessageModel
                    {
                        ID = schema.Id,
                        Message = schema.Text,
                        CreatedAt = createdAt,
                        FreezeEndEdge = freezeEndEdge,
                        KeepVelocityOnCountChange = keepVelocityOnCountChange
                    };
                
                case EChatSender.Manager:
                    return new ManagerMessageModel
                    {
                        ID = schema.Id,
                        Image = sprite,
                        Message = schema.Text,
                        ContentType = messageType,
                        CreatedAt = createdAt,
                        FreezeEndEdge = freezeEndEdge,
                        KeepVelocityOnCountChange = keepVelocityOnCountChange
                    };
            }
            
            throw new ArgumentException($"Unknown sender: {sender}");
        }

        private async UniTask<IChatItemModel> ConvertToBlackjackModel(ChatMessageResponseSchema schema, bool freezeEndEdge, bool keepVelocityOnCountChange, CancellationToken cancellationToken)
        {
            try
            {
                var blackjackChatSchema = JsonConvert.DeserializeObject<BlackjackChatSchema>(schema.Text);
                var blackjackRequestSchema = await _client.GetPokerRequest(blackjackChatSchema.RequestId, cancellationToken);
                var blackjackResult = _serverConstConverter.ToEResult(blackjackRequestSchema.Result);
                var createdAt = DateTime.Parse(schema.CreatedAt, null, DateTimeStyles.RoundtripKind);
                return new BlackjackModel()
                {
                    ID = schema.Id,
                    GameResult =  blackjackResult,
                    FreezeEndEdge = freezeEndEdge,
                    KeepVelocityOnCountChange = keepVelocityOnCountChange,
                    CreatedAt = createdAt,
                };
            }
            catch (Exception exception)
            {
                _logger.Error("Error while converting blackjack chat schema", exception, this);
                throw;
            }
            
        }

        private bool ContainsLoadedItem(int id)
        {
            return GetLoadedItemIndex(id) >= 0;
        }

        private int GetLoadedItemIndex(int id)
        {
            if (id == -1)
                return -1;

            for (var i = 0; i < _loadedItems.Count; i++)
            {
                if (_loadedItems[i].ID == id)
                    return i;
            }

            return -1;
        }
        
        private HashSet<int> GetLoadedItemIds()
        {
            var loadedItemIds = new HashSet<int>();
            foreach (var item in _loadedItems)
                loadedItemIds.Add(item.ID);

            return loadedItemIds;
        }
        
        private void PrepareBlackjacksBeforeInsert(IList<IChatItemModel> items)
        {
            var latestBlackjackId = -1;
            foreach (var item in _loadedItems)
            {
                if (item.CachedType == typeof(BlackjackModel) && item.ID > latestBlackjackId)
                    latestBlackjackId = item.ID;
            }

            foreach (var item in items)
            {
                if (item.CachedType == typeof(BlackjackModel) && item.ID > latestBlackjackId)
                    latestBlackjackId = item.ID;
            }

            if (latestBlackjackId < 0)
                return;

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item.CachedType == typeof(BlackjackModel) && item.ID != latestBlackjackId)
                    items.RemoveAt(i);
            }

            var shouldKeepLoadedBlackjack = false;
            foreach (var item in _loadedItems)
            {
                if (item.CachedType == typeof(BlackjackModel) && item.ID == latestBlackjackId)
                {
                    shouldKeepLoadedBlackjack = true;
                    break;
                }
            }

            if (shouldKeepLoadedBlackjack)
                return;

            for (var i = _loadedItems.Count - 1; i >= 0; i--)
            {
                var item = _loadedItems[i];
                if (item.CachedType == typeof(BlackjackModel))
                    _loadedItems.Remove(item);
            }
        }

        private void UpdateLastBlackjackDateTime(ChatMessageResponseSchema schema)
        {
            var createdAt = DateTime.Parse(schema.CreatedAt, null, DateTimeStyles.RoundtripKind);
            if (createdAt > _lastBlackjackDateTime)
                _lastBlackjackDateTime = createdAt;
        }
    }
}