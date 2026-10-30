# Registration & Configuration

## DI Registration

Register with `AddShinyAiConversation()` (namespace `Shiny`). The configure callback is optional.

```csharp
using Shiny;
using Shiny.AiConversation;

var builder = MauiApp.CreateBuilder();   // or any IServiceCollection host

builder.Services.AddShinyAiConversation(opts =>
{
    // Choose one chat client approach:

    // Option A: Register IChatClient in DI (resolved by the default provider)
    // builder.Services.AddChatClient(new OpenAIClient("key").GetChatClient("gpt-4o").AsIChatClient());

    // Option B: Static OpenAI-compatible provider (NuGet: Shiny.AiConversation.OpenAi)
    // opts.AddStaticOpenAIChatClient("your-api-key", "https://api.openai.com/v1", "gpt-4o");

    // Option C: GitHub Copilot with device code auth (NuGet: Shiny.AiConversation.Maui.GithubCopilot)
    opts.AddGithubCopilotChatClient();

    // Optional: persistent stores (default keeps sessions and memory in memory)
    // opts.SetSqliteDocDbStores(Path.Combine(FileSystem.AppDataDirectory, "ai.db"));   // NuGet: Shiny.AiConversation.Stores.SqliteDocDb
    // opts.SetSessionStore<MySessionStore>();
    // opts.SetMemoryStore<MyMemoryStore>();

    // Optional: built-in features
    opts.AddSessionTitles();
    opts.AddConversationSearch();
    opts.AddLongTermMemory(sessionCount: 3);
    // opts.ConfigureMemory(m => m with { Enabled = false });

    // Optional: tools and prompts
    opts.AddInterceptor<AssistantInterceptor>();
    opts.AddTool(AIFunctionFactory.Create(GetWeather, "get_weather"));
});

var app = builder.Build();
```

Then inject `IAiSessionManager` wherever you need a session.

## AiConversationOptions

### SetChatClientProvider<T>()
- **Optional** - without it, `InjectedChatClientProvider` resolves `IChatClient` from DI
- Registers `T` as a singleton `IChatClientProvider` (`TryAddSingleton`, first registration wins)

### SetSessionStore<T>()
- **Optional** - defaults to `InMemoryAiSessionStore` (nothing survives a restart)
- Registers `T` as a singleton `IAiSessionStore` (`TryAddSingleton`)

### SetMemoryStore<T>()
- **Optional** - defaults to `InMemoryAiMemoryStore`
- Registers `T` as a singleton `IAiMemoryStore` (`TryAddSingleton`)

### ConfigureMemory(Func<AiMemoryOptions, AiMemoryOptions>)
- Memory limits and on/off: `Enabled` (default true), `DisabledProfiles`, `MaxDocumentBytes` (16 KB),
  `MaxIndexEntries` (50), `MaxIndexTokens` (1500), `MaxScopeBytes` (256 KB), `MaxWritesPerTurn` (20); null disables a limit
- See `memory.md`

### SetContextCompactor<T>()
- **Optional** - defaults to `ChatClientContextCompactor` (asks the session's model for a rolling summary)
- See `compaction.md`

### AddSessionTitles() / AddConversationSearch(params string[] profiles) / AddLongTermMemory(int sessionCount = 3, params string[] profiles)
- Built-in interceptors; see `built-in-tools.md`

### SetSqliteDocDbStores(string databasePath) / SetDocDbStores(string documentStoreKey)
- From `Shiny.AiConversation.Stores.SqliteDocDb` (Shiny.DocumentDb 14.x); registers both `IAiSessionStore` and `IAiMemoryStore`
- `SetDocDbStores(key)` resolves the keyed `IDocumentStore` registered by Shiny.DocumentDb's `services.AddDocumentStore(key, o => { ...; o.MapAiConversation(); })`
- See `session-store.md`

### SetAttachmentStore<T>() / UseFileAttachments(string directory)
- Where image attachments go; defaults to `InMemoryAiAttachmentStore`. `UseFileAttachments` stores files under `{directory}/{sessionId}/`
- See `images.md`

### AddInterceptor<T>()
- Registers an `ISessionInterceptor` singleton; multiple are supported and run in registration order
- See `interceptors.md`

### AddTool(AITool tool, AiToolFlags flags = None, params string[] profiles)
- Adds the tool to every session, or only to sessions whose `AiSessionSettings.Profile` is in `profiles`
- `AiToolFlags.Silent` marks tools that should never trigger a spoken "one moment" acknowledgement (surfaced as `ToolCallStarted.Silent`)
- Plain `AITool` singletons registered directly in DI are also added to every session

## Auto-Registered Dependencies

`AddShinyAiConversation()` registers (via `TryAdd`, so register your own first to override):
- `TimeProvider.System`
- `InjectedChatClientProvider` as `IChatClientProvider`
- `InMemoryAiSessionStore` as `IAiSessionStore`
- `InMemoryAiMemoryStore` as `IAiMemoryStore`
- `ChatClientContextCompactor` as `IContextCompactor`
- `InMemoryAiAttachmentStore` as `IAiAttachmentStore`
- `AiMemoryOptions` (from `ConfigureMemory`)
- `AiSessionManager` as `IAiSessionManager`
- `DefaultSessionInterceptor` - adds tools from `AddTool` / DI to each session and the current local time as a system prompt on every turn

It does **not** register any speech or audio services.

## Voice registration (`Shiny.AiConversation.Voice`)

```csharp
builder.Services.AddSpeechServices();     // or your cloud Shiny.Speech providers - required, never auto-registered
builder.Services.AddAudioPlayer();        // optional: sounds + gapless cloud-voice playback
builder.Services.AddShinyAiConversationVoice(v => v
    .AddVoiceSelectionTools()             // ISessionInterceptor adding get_available_voices / play_voice_sample / change_voice
    .SetSoundProvider<MySounds>()         // optional; default DefaultSoundProvider (embedded WAVs via IAudioPlayer)
);
```

Registers (`TryAdd`): `TimeProvider.System`, `ISoundProvider` → `DefaultSoundProvider`,
`IVoiceConversation` → `VoiceConversation` (singleton). Register an `IWakeWordDetector` to keep the
recognizer closed until the wake word. See `voice.md`.

## Camera registration (`Shiny.AiConversation.Maui.Camera`)

```csharp
builder.UseShinyCamera();                       // IMediaService
builder.Services.AddAiConversationCamera(c => c
    .Configure(o => o with { RequireConfirmation = true })
    .AddScanner(CameraMode.Barcode, async (media, req, ct) => (await media.ScanBarcodeAsync())?.Value)
    .SetConsent<MyConsent>());                  // default AlertCameraConsent
```

Registers `ICameraCapture` (`MediaServiceCameraCapture`), `ICameraConsent` and the `CameraToolInterceptor`. See `camera.md`.

## MAUI chat UI (`Shiny.AiConversation.Maui`)

No registration is required: `AiChatView` resolves `IAiSessionManager` / `IAiAttachmentStore` from the app's
services. `opts.ConfigureChatView(s => s.BotName = "Aura")` registers shared `AiChatSettings`. See `maui.md`.

## Per-Session Settings

`AiSessionSettings` is persisted with the session and passed to `Start`:

```csharp
var session = await manager.Start(new AiSessionSettings
{
    Profile = "cooking",           // interceptors / AddTool profiles key off this
    ContextWindow = 128_000,       // enables context pressure; falls back to IChatClientProvider.ContextWindow
    ContextWarningThreshold = 0.75,
    ContextCriticalThreshold = 0.90,
    Tags = ["kitchen"],
    ExcludeFromSearch = false,     // keep out of conversation search / long-term memory
    Compaction = new CompactionSettings { KeepRecentMessages = 10, AutoCompactThreshold = 0.95 },
    Metadata = new Dictionary<string, string> { ["recipeId"] = "42" }
});

await session.UpdateSettings(s => s with { Tags = ["kitchen", "dinner"] });
```
