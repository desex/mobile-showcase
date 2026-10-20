# Tech Context: MobileApp

## Используемые технологии

### Движок и среда

| Технология | Версия | Назначение |
|------------|--------|------------|
| Unity | 6000.0.58f2 | Игровой движок |
| .NET | Standard 2.1 | Runtime |
| Rider | - | IDE |

### Render Pipeline

- **URP (Universal Render Pipeline)** 17.0.4 — основной рендеринг

### Dependency Injection

- **VContainer** 1.17.0 — DI-контейнер

### Реактивное программирование

- **R3** (Cysharp) — реактивные расширения для Unity
- **UniTask** 2.5.10 — async/await без аллокаций

### Анимации

- **LitMotion** — высокопроизводительные анимации

### UI

- **UGUI** 2.0.0 — UI система
- **SoftMask-for-ugui** 3.5.0 — мягкие маски
- **UI-Effect** — эффекты для UI
- **OSA (Optimized ScrollView Adapter)** — оптимизированные списки

### Сеть и API

- **Best HTTP** — HTTP-клиент
- **Best Server-Sent Events** — SSE соединения
- **Best TLS Security** — безопасность соединений

### База данных

- **SQLite-net** 1.3.2 (gilzoide) — локальное хранение

### Загрузка ресурсов

- **Addressables** 2.7.4 — асинхронная загрузка
- **AssetBundle** — система бандлов

### Firebase

- **Firebase App** 13.6.0
- **Firebase Messaging** 13.6.0 — Push-уведомления

### Мультимедиа

- **AVPro Video** — видеоплеер
- **Unity Recorder** 5.1.3 — запись видео

### Локализация

- **Unity Localization** 1.5.9

### Инспектор

- **Odin Inspector** (Sirenix) — расширенный инспектор

### Утилиты

- **Burst** 1.8.25 — компиляция для высокой производительности
- **Collections** 2.5.1 — нативные коллекции
- **Input System** 1.14.2 — новая система ввода

### Платформенные сервисы

- **App Tracking Transparency** 0.7.0 — iOS ATT
- **Google External Dependency Manager** 1.2.187

### Внутренние пакеты (GitLab)

- **UState** — конечные автоматы
- **Builder** — паттерн builder

## Настройка среды разработки

### Требования

1. Unity 6000.0.58f2 (Unity 6)
2. JetBrains Rider или Visual Studio
3. Git
4. Android SDK / iOS Xcode (для сборки)

### Структура проекта

```
Assets/
├── Scripts/           # Исходный код
│   ├── Application/   # Ядро приложения
│   ├── Chat/          # Модуль чата
│   ├── News/          # Модуль новостей
│   └── ...            # Другие модули
├── ResourcesStatic/   # Статические ресурсы
├── ResourcesAddressables/  # Адресуемые ресурсы
├── Settings/          # Настройки
├── StreamingAssets/   # Потоковые ассеты
└── Plugins/           # Нативные плагины
```

### Assembly Definitions

Каждый модуль имеет свой `.asmdef` файл:
- `Sotbify.MobileApp.Chat`
- `Sotbify.MobileApp.News`
- и т.д.

## Технические ограничения

### Память

- Внимательное управление памятью для мобильных устройств
- Использование Object Pooling для часто создаваемых объектов
- Addressables для выгрузки неиспользуемых ресурсов

### Производительность

- OSA для списков с большим количеством элементов
- Burst Compiler для вычислений
- Избегание GC аллокаций в Update

### Сеть

- Обработка потери соединения
- Reconnect логика для SSE
- Кэширование данных локально

## Зависимости

### NuGet пакеты (через NuGetForUnity)

Пакеты управляются через `Packages/nuget-packages/`

### Scoped Registries

- **Open UPM** — для Cysharp, VContainer и других пакетов
- **GitLab** — для внутренних пакетов Sotbifay

## Паттерны использования инструментов

### Логирование

```csharp
// ZLogger интеграция
ILogger logger = resolver.Resolve<ILogger>();
logger.Info("Message");
logger.Warning("Warning");
logger.Error("Error");
```

### Preferences

```csharp
// Сохранение данных
IPreferences preferences = resolver.Resolve<IPreferences>();
preferences.SetString("key", "value");
preferences.Save();
```

### Scene Loading

```csharp
// Загрузка сцен
ISceneLoader sceneLoader = resolver.Resolve<ISceneLoader>();
await sceneLoader.LoadSceneAsync("SceneName");