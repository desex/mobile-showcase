# System Patterns: MobileApp

## Архитектура системы

### Общая структура

Приложение построено на модульной архитектуре с использованием:
- **VContainer** — внедрение зависимостей
- **UState** — конечные автоматы (FSM) для управления состояниями

### Две главные FSM

#### Application (IApplication)
Глобальная FSM для управления жизненным циклом приложения. Реализует `IStateMachine<IApplication>`.

```
Application (IApplication)
├── InitializationState    → Загрузка сцены Application, инициализация локализации
├── AuthenticationState    → Проверка токена, аутентификация
└── ...
```

- `Initialize()` — входная точка, запускает `InitializationState`
- `StartAsync()` — бесконечный цикл выполнения FSM

#### ApplicationUI (IApplicationUI)
FSM для управления UI состояниями. Реализует `IStateMachine<IApplicationUI>`.

```
ApplicationUI (IApplicationUI)
├── GreetingsWindowState    → Приветственное окно
├── LoadingScreenState      → Экран загрузки
├── HomeWindowState         → Главный экран
├── ChatWindowState         → Чат
├── NewsWindowState         → Новости
├── ProfileState            → Профиль
├── BonusesWindowState      → Бонусы
├── Tutorial States         → Состояния туториала
└── Stab States            → Альтернативный UI для других пользователей
```

### Взаимодействие FSM

```
Application (глобальные состояния)
    │
    ├── InitializationState
    │       │
    │       └── Загружает сцену "Application"
    │       └── Создаёт ApplicationUIScopeFacade
    │       └── Передаёт управление → AuthenticationState
    │
    └── AuthenticationState
            │
            └── Проверяет токен
            └── applicationUI.RequestTransition<...>() → Передаёт управление в ApplicationUI
```

### Иерархия Scopes

```
ProjectLifetimeScope (Bootstrap -> Application)
│   ├── Глобальные инсталлеры (работают между сценами)
│   ├── LoggerInstaller
│   ├── ServerInstaller
│   ├── AssetsProviderInstaller
│   ├── SceneLoaderInstaller
│   ├── PreferencesInstaller
│   └── ...
│
└── MainGameLifetimeScope (Application)
    ├── Инсталлеры приложения (UI, окна, модули)
    ├── WindowInstallers
    ├── ChatInstaller
    ├── NewsInstaller
    └── ...
```

1. **ProjectLifetimeScope** — глобальный scope, живёт между сценами (Bootstrap → Application). Содержит глобальные сервисы: Logger, Server, AssetsProvider, SceneLoader, Preferences и т.п.
2. **MainGameLifetimeScope** — scope приложения, отдельный LifetimeScope с parent = ProjectLifetimeScope. Содержит инсталлеры: окна, модули UI, конкретные функции.
3. **Child Scopes** — могут создаваться по необходимости для специфичных задач

## Ключевые технические решения

### Dependency Injection (VContainer)

```csharp
// Паттерн регистрации сервисов
builder.Register<Service>(Lifetime.Singleton)
    .As<IService>();

// Паттерн регистрации конфигов
builder.RegisterInstance(scriptableObject)
    .AsImplementedInterfaces();

// Паттерн EntryPoint
builder.RegisterEntryPoint<Presenter>()
    .As<IPresenter>();
```

### Реактивное программирование (R3)

- Использование `ReactiveProperty<T>` для состояния
- **События**: приватный `Subject<T>`, публично — `Observable<T>` (инкапсуляция)
- **Отмена подписок**: `CompositeDisposable` для управления множеством подписок
- **Коллекции**: `ObservableCollections` (ObservableList, ObservableDictionary и т.д.)

```csharp
// Паттерн событий
private readonly Subject<Unit> _onEvent = new();
public Observable<Unit> OnEvent => _onEvent;

// Паттерн подписок
private readonly CompositeDisposable _disposable = new();

public void Subscribe()
{
    observable.Subscribe(x => { }).AddTo(_disposable);
}

public void Dispose()
{
    _disposable.Dispose();
}
```

### ObservableCollections (R3)

Для реактивных коллекций используется `ObservableCollections` из R3:

```csharp
// Интерфейс — только чтение
public interface IHome
{
    IReadOnlyObservableList<IHomeModel> HomeModels { get; }
}

// Реализация
private readonly ObservableList<IHomeModel> _homeModels = new();
public IReadOnlyObservableList<IHomeModel> HomeModels => _homeModels;

// Подписки в Presenter
Model.HomeModels
    .ObserveAdd()
    .Subscribe(View, (@event, view) => view.HomeListView.Data.Add(@event.Value))
    .AddTo(_disposables);

Model.HomeModels
    .ObserveRemove()
    .Subscribe(View, (@event, view) => view.HomeListView.Data.RemoveAt(@event.Index))
    .AddTo(_disposables);

Model.HomeModels
    .ObserveClear()
    .Subscribe(View, (_, view) => view.HomeListView.Data.Clear())
    .AddTo(_disposables);
```

**Правило инкапсуляции**: Если нужно передать коллекцию вне скрипта — использовать `IReadOnlyObservableList`, `IReadOnlyList`, `IReadOnlyDictionary` и т.п.

### Асинхронность (UniTask)

- `async UniTask` для асинхронных операций
- `CancellationToken` для отмены
- **Публичные UniTask методы**: обязательно `CancellationToken cancellationToken = default`

```csharp
// Публичный асинхронный метод
public async UniTask DoSomethingAsync(CancellationToken cancellationToken = default)
{
    await SomeOperation(cancellationToken);
}
```

## Паттерны проектирования

### MVP для окон (WindowService)

WindowService обеспечивает стандартизированный подход к созданию окон:

```
INameOfWindow (интерфейс)
    ↓
NameOfWindow (модель)
    ↓
NameOfWindowPresenter : WindowPresenter<INameOfWindow, NameOfWindowView>
    ↓
NameOfWindowView : FadeWindowView
```

### Паттерн выхода из состояния

При выходе из состояния используется `CloseAsync` с безопасным extension-методом для UniTask:

```csharp
protected override UniTask ExitAsync(CancellationToken cancellationToken)
{
    _windowService.CloseAsync<IHome>(cancellationToken)
        .ForgetIgnoreOperationCancelledException(_logger, "Failed to close home", this);
    
    return UniTask.CompletedTask;
}
```

**`ForgetIgnoreOperationCancelledException`** — extension метод из `Sotbify.MobileApp.Utils`:
- Игнорирует `OperationCanceledException`
- Остальные исключения логирует через `ILogger` с указанным сообщением и контекстом
- Аргументы: `ILogger logger`, `string message`, `object context` (обычно `this`)

**Структура модуля:**
```
ModuleName/
├── Runtime/
│   ├── Sotbify.MobileApp.ModuleName.asmdef
│   ├── INameOfWindow.cs
│   └── Impls/
│       ├── NameOfWindow.cs              # Модель (без суффикса Window)
│       ├── NameOfWindowInstaller.cs     # Инсталлер (с суффиксом Window)
│       ├── NameOfWindowView.cs          # View (с суффиксом Window)
│       └── NameOfWindowPresenter.cs     # Presenter (с суффиксом Window)
```

**Регистрация окна:**
```csharp
public class NameOfWindowInstaller : IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.RegisterWindow<INameOfWindow, NameOfWindow, NameOfWindowPresenter, NameOfWindowView>();
    }
}
```

**Важно:** Модель называется `NameOfWindow` (без дополнительного суффикса), а Installer/Presenter/View имеют суффикс `Window` для избежания конфликтов имён.

### MVP для сервисов (Repository Pattern)

```
Model (IChatItemsRepository)
    ↓
Presenter (ChatPresenter)
    ↓
View (ChatView, IChatView)
```

### Repository Pattern

```csharp
public interface IChatItemsRepository
{
    IReadOnlyReactiveProperty<IReadOnlyList<IChatItemModel>> Items { get; }
    void AddItem(IChatItemModel item);
    void Clear();
}
```

### Installer Pattern

```csharp
public class ChatInstaller : IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<ChatItemsRepository>(Lifetime.Singleton)
            .AsImplementedInterfaces();
        
        builder.Register<ChatPresenter>(Lifetime.Singleton);
    }
}
```

### Factory Pattern через VContainer

```csharp
builder.RegisterFactory<IChatItemModel, ChatItemViewsHolder>(container =>
{
    return model => container.Resolve<ChatItemViewsHolder>();
}, Lifetime.Scoped);
```

## Взаимосвязи компонентов

### Навигация

```
Dock (Navigation)
    ├── HomeWindow
    ├── ChatWindow
    ├── NewsWindow
    ├── CasinoWindow
    ├── ProfileWindow
    └── BonusWindow
```

### Слой данных

```
Server (API)
    ↓
Repository (Local Cache - SQLite)
    ↓
Presenter
    ↓
View
```

## Работа с сетью (BestHttp)

### Архитектура HTTP клиента

```
IHttpClient (интерфейс)
    ↓
BestHttpClient (реализация на Best.HTTP)
    ↓
Server (фасад для API, аутентификация)
    ↓
Client (partial классы по доменам)
```

### BestHttpClient

Обёртка над Best.HTTP с throttling и логированием:

```csharp
public class BestHttpClient : IHttpClient, IDisposable
{
    private const int MaxActiveRequestsCount = 3;
    private readonly SemaphoreSlim _semaphoreSlim = new(MaxActiveRequestsCount, MaxActiveRequestsCount);
    private readonly ObjectPool<StringBuilder> _stringBuilderPool;
    
    public async UniTask<HttpResponse> SendAsync(HttpRequest request, CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            // Создание запроса, отправка, логирование
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}
```

**Особенности:**
- Максимум 3 одновременных запроса (`SemaphoreSlim`)
- ObjectPool для StringBuilder (оптимизация логирования)
- Поддержка `SendAsync` и `DownloadAsync` с прогрессом

### HttpRequestBuilder

Builder паттерн для создания запросов. Пулинг через `IObjectPool<HttpRequestBuilder>`:

```csharp
using (_httpRequestBuildersPool.Get(out var requestBuilder))
{
    requestBuilder
        .SetMethod(HttpMethod.Post)
        .SetPath("/mobile/chat/send_message")
        .SetBody(requestBody)
        .SetJsonContentType();

    var request = requestBuilder.Create();
    var response = await _server.SendAsync(SavedQR, FirebaseToken, request, cancellationToken);
}
```

### Server — фасад для API

```csharp
public class Server : IServer
{
    public string AccessToken => _signInSchema?.AccessToken ?? string.Empty;
    
    // Аутентификация
    public UniTask<SignInSchema> AuthenticationAsync(string qrContent, string firebaseToken, CancellationToken cancellationToken = default);
    
    // Отправка запроса с auto-retry при 401
    public async UniTask<HttpResponse> SendAsync(string qrContent, string firebaseToken, HttpRequest request, CancellationToken cancellationToken = default)
    {
        // При 401 — реаутентификация и retry
        if (isRetry == false && response is { IsSuccess: false, StatusCode: 401 })
        {
            _signInSchema = null;
            isRetry = true;
            continue;
        }
    }
}
```

### Client — partial классы по доменам

Разделение API по функциональным областям:

```
Client.cs           — общая логика, IAuthenticatorProvider
Client.Chat.cs      — чат API
Client.Feed.cs      — лента API
Client.Auth.cs      — аутентификация
Client.NewsContent.cs — контент
Client.Partners.cs  — партнёры
Client.Promocode.cs — промокоды
```

```csharp
public partial class Client
{
    public async UniTask<ChatMessageResponseSchema> SendChatMessageAsync(string message,
        CancellationToken cancellationToken = default)
    {
        using (_httpRequestBuildersPool.Get(out var requestBuilder))
        {
            var request = requestBuilder
                .SetMethod(HttpMethod.Post)
                .SetPath("/mobile/chat/send_message")
                .SetBody(requestBody)
                .SetJsonContentType()
                .Create();
                
            var response = await _server.SendAsync(SavedQR, FirebaseToken, request, cancellationToken);
            return await _serializer.DeserializeAsync<ChatMessageResponseSchema>(response.DataAsText, cancellationToken);
        }
    }
}
```

### SSE (Server-Sent Events)

```csharp
public ISSEFacade CreateChatSse()
{
    var uri = new UriBuilder(_apiClientConfiguration.BaseUrl)
        .WithPathSegment("sse/chat/mobile")
        .Uri;

    return new SSEFacade(uri, this, _logger);
}
```

## Критические пути реализации

### Инициализация приложения

1. **Bootstrap**: `ProjectLifetimeScope` создаёт глобальные сервисы (Logger, Server, AssetsProvider, SceneLoader, Preferences)
2. `Application.Initialize()` → `RequestTransition<InitializationState>()`
3. **InitializationState**:
   - Инициализация локализации (Unity Localization)
   - Загрузка сцены "Application" через `ISceneLoader`
   - Создание `MainGameLifetimeScope` с parent = `ProjectLifetimeScope`
   - Получение `IApplicationUI` из нового scope
   - `applicationUI.RequestTransition<...>()` через `ApplicationUIScopeFacade`
4. **AuthenticationState**:
   - Проверка QR-токена из `IPreferences`
   - Валидация токена через `Client.TryAuthenticate()`
   - Переход в `LoadingScreenState` или `GreetingsWindowState`
5. **ApplicationUI states**:
   - `LoadingScreenState` → загрузка данных
   - `HomeWindowState` → главный экран с `WindowService.OpenAsync<IHome>()`

### Поток сообщений чата

1. SSE соединение (`ChatMessagesStream`)
2. Десериализация JSON
3. Добавление в `ChatItemsRepository`
4. Обновление UI через `MessagesListView` (OSA)

### Система туториалов

1. Проверка состояния туториала
2. Последовательное отображение окон
3. Сохранение прогресса в Preferences