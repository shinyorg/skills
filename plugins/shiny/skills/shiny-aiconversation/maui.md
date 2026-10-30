# MAUI Chat UI (`Shiny.AiConversation.Maui`)

`AiChatView` is a Shiny.Maui.Controls `ChatView` bound to one `IAiSession`. Every style/template property of the
base control still applies (`MyBubbleColor`, `OtherTextColor`, `MessageTemplateSelector`, `InputActions`, …).
XAML namespace: `clr-namespace:Shiny.AiConversation.Maui;assembly=Shiny.AiConversation.Maui` (prefer it over the `http://shiny.net/maui/aiconversation` URL; see below). The package references `Shiny.AiConversation.Voice`.


> **XAML namespace in Debug builds:** Debug builds inflate XAML at runtime, and the URL namespace
> `http://shiny.net/maui/aiconversation` only resolves once `Shiny.AiConversation.Maui` is loaded. If no C# code
> has touched it yet, the page fails with "Type AiChatView not found in xmlns". Use the assembly-qualified form,
> which loads it: `xmlns:ai="clr-namespace:Shiny.AiConversation.Maui;assembly=Shiny.AiConversation.Maui"`.

## What it does

- Session messages map 1:1 to bubbles. A streamed reply is **one** bubble, created on its first text and updated in
  place (throttled ~100ms) via `MessageUpdated(MessageChangeKind.Edited)`
- History pages by `Sequence` (`session.GetMessages`); compacted messages still show
- Only user text/answers/images and assistant text get bubbles. Tool calls, memory writes, summaries and empty
  replies do not. Failed / interrupted replies carry `Identifier = "failed"` / `"interrupted"`
- Typing indicator follows `Thinking`/`Responding` with a 3s heartbeat (ChatView drops it after 6s)
- Choice chips under the AI bubble that asked (from `session.PendingQuestions`); a tap sends `AiInput.Answer(question, choiceIds)`
- `SessionError` renders as an AI bubble with `Identifier = "error"`
- Sent messages come back with the stored message id, so the session's own `MessageAdded` merges instead of duplicating

## Session ownership

- **Bind `Session`** to the app's shared session (recommended). The view never disposes a bound session.
- Leave `Session` unset and the view resumes `IAiSessionManager.GetLatest()` (or `Start()`s one) and **owns** it,
  disposing it when the handler detaches. `NewConversation()` only works in this mode (it throws when `Session` is bound).

## AiChatView members

| Member | Default | Notes |
|---|---|---|
| `Session` | null | `IAiSession` to show; null = auto-load and own |
| `CurrentSession` | — | The session currently shown |
| `BotName` / `BotAvatar` / `BotBubbleColor` | `Assistant` / null / null | |
| `UserName` / `UserAvatar` / `UserBubbleColor` | `Me` / null / null | |
| `LoadHistory` | `true` | Backfill + page history |
| `ShowTokenUsage` | `false` | Token footer on AI messages |
| `AllowImages` | `false` | Lets the user attach photos (sent via `AiInput.WithImage`, retention from `Settings.ImageRetention`) |
| `ShowChoiceButtons` / `ChoiceSendText` | `true` / `Send` | Built-in selector installed only when you set no `MessageTemplate(Selector)` |
| `GreetingMessage` | null | Shown when there is no history |
| `ShowContextWarnings` / `ContextWarningText` | `false` | Pushes an AI bubble (`Identifier = "context"`) at Warning/Critical |
| `ShowMicrophoneAction` / `MicrophoneActionText` | `false` / `🎤 Voice Input` | Push-to-talk input action |
| `VoiceOptions` | null | Used when the mic action attaches the voice conversation |
| `Settings` | — | Live `AiChatSettings` (also `ImageRetention`, `ShowSpokenPartOfInterruptedReplies`, `ErrorPrefix`) |
| `event ContextPressureChanged` | — | `ContextSnapshot`; show your own banner |
| `Refresh()` / `Compact()` / `NewConversation()` | — | Reload / compact the session / start a fresh owned session |

## XAML

```xml
<ContentPage xmlns:ai="clr-namespace:Shiny.AiConversation.Maui;assembly=Shiny.AiConversation.Maui" ...>
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="New" Command="{Binding NewConversationCommand}" />
    </ContentPage.ToolbarItems>

    <ai:AiChatView Session="{Binding Session}"
                   BotName="Copilot"
                   AllowImages="True"
                   ShowContextWarnings="True"
                   ShowMicrophoneAction="True"
                   ShowTokenUsage="True"
                   GreetingMessage="Ask me anything."
                   MyBubbleColor="{StaticResource Primary}"
                   MyTextColor="White" />
</ContentPage>
```

## Share one session across pages (ConversationHost pattern)

```csharp
public class ConversationHost(IAiSessionManager manager, IVoiceConversation voice)
{
    readonly SemaphoreSlim gate = new(1, 1);
    IAiSession? session;

    public IVoiceConversation Voice => voice;
    public VoiceOptions VoiceOptions { get; private set; } = new();
    public event EventHandler<IAiSession>? SessionChanged;

    public async Task<IAiSession> GetSession()
    {
        await gate.WaitAsync();
        try { return session ??= await manager.GetLatest() ?? await manager.Start(); }
        finally { gate.Release(); }
    }

    public async Task<IAiSession> NewConversation()
    {
        await gate.WaitAsync();
        IAiSession next;
        try
        {
            var old = session;
            session = next = await manager.Start();
            if (old != null)
            {
                var attached = ReferenceEquals(voice.Session, old);
                if (attached) await voice.Detach();
                await old.DisposeAsync();
                if (attached) await voice.Attach(next, VoiceOptions);
            }
        }
        finally { gate.Release(); }
        SessionChanged?.Invoke(this, next);
        return next;
    }

    // voice and chat talk to the SAME instance
    public async Task EnsureVoice()
    {
        var current = await GetSession();
        if (ReferenceEquals(voice.Session, current) && voice.Options == VoiceOptions)
            return;
        await voice.Detach();
        await voice.Attach(current, VoiceOptions);
    }
}

// registration
builder.Services.AddSingleton<ConversationHost>();

// view model for the chat page
public partial class ChatViewModel(ConversationHost host) : ObservableObject
{
    [ObservableProperty] IAiSession? session;
    public async void OnAppearing() => this.Session = await host.GetSession();
    [RelayCommand] async Task NewConversation() => this.Session = await host.NewConversation();
}
```

## AiMicrophoneInputAction

Push-to-talk `ChatInputAction` over `IVoiceConversation` (added by `ShowMicrophoneAction`). On invoke it requests
access, attaches the voice conversation to the chat's session (detaching whatever it held: one session holds the
mic), then calls `PushToTalk()`. Invoking while the AI speaks stops the reply and listens. Properties: `Session`,
`Voice` (resolved from DI when null), `VoiceOptions`, `ListenText`. Needs `AddShinyAiConversationVoice()` plus
registered Shiny.Speech providers.

## AiChatSessionProvider (plain ChatView)

```csharp
var provider = new AiChatSessionProvider(session, new AiChatSettings { BotName = "Aura" }, attachmentStore);
provider.ContextPressureChanged += (_, context) => ShowBanner(context);
chatView.Provider = provider;
```

`ConfigureChatView(Action<AiChatSettings>)` on `AiConversationOptions` registers app-wide `AiChatSettings` defaults; every `AiChatView` applies them to the properties it doesn't set itself.
Reading choices in your own template: `AiChoiceTemplateSelector.ReadQuestions(chatMessage)`.

## AiMemoryView

A `ContentView` listing every memory document (`IAiSessionManager.Memory`, or bind `Memory`), sorted by scope and
path. Tap to edit content (Save / Delete / Cancel); a "Forget everything" button calls `Memory.Clear()` after a
confirmation (`ConfirmForget`, default true). `Reload()` refreshes.

```xml
<ContentPage Title="What the AI Remembers">
    <ai:AiMemoryView />
</ContentPage>
```

## iOS: UIScene lifecycle is required

Current iOS terminates apps at launch (`EXC_BREAKPOINT` in `_UIApplicationEvaluateRuntimeIssueForNoSceneLifecycleAdoption`)
unless they adopt the scene lifecycle. Add to `Platforms/iOS/Info.plist`:

```xml
<key>UIApplicationSceneManifest</key>
<dict>
    <key>UIApplicationSupportsMultipleScenes</key>
    <false/>
    <key>UISceneConfigurations</key>
    <dict>
        <key>UIWindowSceneSessionRoleApplication</key>
        <array>
            <dict>
                <key>UISceneConfigurationName</key>
                <string>__MAUI_DEFAULT_SCENE_CONFIGURATION__</string>
                <key>UISceneDelegateClassName</key>
                <string>SceneDelegate</string>
            </dict>
        </array>
    </dict>
</dict>
```

```csharp
[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate;
```

Do a clean build after changing `Info.plist`; an incremental build can keep the old one in the bundle.
