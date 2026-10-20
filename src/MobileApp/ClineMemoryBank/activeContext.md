# Active Context: MobileApp

## Текущий фокус работы

Реализован сценарий настройки PIN из профиля: HiddenModeWindow → PinCreationWindow → ProfileState; при наличии PIN из профиля открывается TwoFactorWindow.

## Недавние изменения

- Инициализация банка памяти Cline
- 2026-09-11: подключены HiddenModeWindow, PinCreationWindow и TwoFactorWindow к MVP, ApplicationUI, MainGameLifetimeScope и Addressables. Привязаны кнопки и поле ввода в префабах.
- PIN состоит ровно из четырёх ASCII-цифр; сохраняется строкой в PlayerPrefs по ключу `Profile.PinPassword` через `PinPasswordStore.SavePassword`, с сохранением ведущих нулей.
- Назад и отмена вызывают `IApplicationUI.Undo()`. Изменение PIN открывает PinCreationWindow; сохранение возвращает в существующий `ProfileState` (класса `ProfileWindowState` в проекте нет).
- Отмена в TwoFactorWindow трактуется как возврат без удаления PIN. Проверка PIN при запуске приложения в эту задачу не входит.
- Выполнена отдельная проверка компиляции изменённых модулей и scope с локальными Unity-зависимостями, проверены ссылки префабов и 10 случаев валидации PIN. Сценарий в Play Mode пока не проверен.
- 2026-09-11: ввод PIN визуально разделён на четыре фиксированные ячейки. Один скрытый `TMP_InputField` принимает цифры и управляет цифровой клавиатурой, а каждая цифра отображается отдельным `TMP_Text` поверх соответствующего кружка. Общий текст, каретка и выделение input скрыты, поэтому после четвёртой цифры ряд не смещается.
- `ActiveView` подсвечивает текущую свободную PIN-ячейку: после ввода цифры выключается у заполненной ячейки и включается у следующей; после четырёх цифр все `ActiveView` выключены. При удалении подсветка возвращается назад.
- `PinCreationWindowState` использует вложенный `Payload` с полем `IsChangingPin`. Переход из HiddenMode передаёт `false`, а переход по кнопке изменения PIN из TwoFactor — `true`. В режиме изменения View показывает `ChangingHeader` и `ChangingDescription`, скрывая тексты создания PIN.
- В `ProfileWindow` поле `IsEnabled` показывает `Включен` цветом `_enabledColor`, если сохранён корректный PIN, иначе `Выключен` цветом `_disabledColor`. Статус вычисляется заново при каждом открытии окна профиля.

## Следующие шаги

Ожидают определения на основе будущих задач.

## Принимаемые решения и соображения

### Архитектурные решения

- **VContainer** — DI-контейнер для модульности и тестируемости
- **UState** — конечные автоматы (FSM) для управления состояниями
- **R3** — реактивное программирование (замена UniRx)
- **UniTask** — асинхронные операции без аллокаций
- **Addressables** — гибкая загрузка ресурсов

### Две главные FSM

#### Application (IApplication)
Глобальная FSM для жизненного цикла приложения:
- `InitializationState` — загрузка сцены Application, локализация
- `AuthenticationState` — проверка токена, аутентификация

#### ApplicationUI (IApplicationUI)
FSM для UI состояний:
- `GreetingsWindowState`, `LoadingScreenState`, `HomeWindowState`
- `ChatWindowState`, `NewsWindowState`, `ProfileState`, `BonusesWindowState`
- Tutorial States, Stab States

**Передача управления:** `applicationUI.RequestTransition<...>()` через `ApplicationUIScopeFacade`

### Структура модулей

Каждый функциональный модуль следует общей структуре:
```
ModuleName/
├── Runtime/
│   ├── Sotbify.MobileApp.ModuleName.asmdef
│   ├── I{Service}.cs           # Интерфейсы
│   ├── {Module}Installer.cs    # VContainer installer
│   └── Impls/
│       ├── {Service}.cs         # Реализации
│       ├── {Presenter}.cs       # Презентеры
│       └── {View}.cs            # Представления
```

**Важно:** В каждом модуле используется `IInstaller`. Глобальные инсталлеры регистрируются в `ProjectLifetimeScope`, инсталлеры UI/окон — в `MainGameLifetimeScope`.

## Важные паттерны и предпочтения

### Код-стайл

- Namespace: `Sotbify.MobileApp.{Module}`
- Интерфейсы с префиксом `I`
- Приватные поля с подчёркиванием `_field`
- Использование Odin Inspector для сериализации
- Новые ScriptableObject-конфиги создавать в формате **без `ScriptableParameters`** (`: ScriptableObject` + нужный интерфейс)
- Для новых SO использовать `CreateAssetMenu` формата: `menuName = "Settings/<Name>_Config"`, `fileName = "<Name>_Config"`

### DI паттерны

- **ProjectLifetimeScope** — глобальный scope (Bootstrap → Application), работает между сценами. Содержит глобальные инсталлеры: LoggerInstaller, ServerInstaller и т.п.
- **MainGameLifetimeScope** — scope приложения, отдельный LifetimeScope с parent = ProjectLifetimeScope. Содержит инсталлеры UI, окон, модулей.
- `IInstaller` для регистрации зависимостей
- ScriptableObject конфиги через `AsImplementedInterfaces()`
- LifetimeScope могут создаваться по необходимости

### UI паттерны

- **OSA (Optimized ScrollView Adapter)** — для всех списков и скроллов с большим количеством элементов
- **WindowService** — сервис для открытия/закрытия окон, обеспечивает лёгкую реализацию MVP

### Структура модуля с окном

```
ModuleName/
├── Runtime/
│   ├── Sotbify.MobileApp.ModuleName.asmdef
│   ├── INameOfWindow.cs                    # Интерфейс окна
│   ├── NameOfWindowInstaller.cs            # Инсталлер
│   └── Impls/
│       ├── NameOfWindow.cs                 # Модель окна
│       ├── NameOfWindowView.cs             # Вью (на префабе)
│       └── NameOfWindowPresenter.cs        # Презентер
```

### Паттерн окна (MVP через WindowService)

```csharp
// Модель
public class NameOfWindow : INameOfWindow { }

// Вью (на префабе)
public class NameOfWindowView : FadeWindowView { }

// Презентер
public class NameOfWindowPresenter : WindowPresenter<INameOfWindow, NameOfWindowView> { }

// Инсталлер
public class NameOfWindowInstaller : IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.RegisterWindow<INameOfWindow, NameOfWindow, NameOfWindowPresenter, NameOfWindowView>();
    }
}
```

## Выводы и инсайты проекта

- Приложение активно развивается с множеством интеграций
- Важна стабильность SSE соединения для чата
- Система туториалов критична для удержания пользователей
