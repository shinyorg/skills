# Registration & Configuration

## DI Registration

Register the AI service using `AddShinyAiConversation()`:

```csharp
using Shiny.AiConversation;

var builder = MauiApp.CreateBuilder();

builder.Services.AddShinyAiConversation(opts =>
{
    // Choose one chat client approach:

    // Option A: Register IChatClient in DI (resolved by default provider)
    // builder.Services.AddChatClient(new OpenAIClient("key").GetChatClient("gpt-4o").AsIChatClient());

    // Option B: Static OpenAI-compatible provider (NuGet: Shiny.AiConversation.OpenAi)
    // opts.AddStaticOpenAIChatClient("your-api-key", "https://api.openai.com/v1", "gpt-4o");

    // Option C: GitHub Copilot with device code auth (NuGet: Shiny.AiConversation.Maui.GithubCopilot)
    opts.AddGithubCopilotChatClient();

    // Optional: enable persistent message storage (ChatLookupAITool added automatically)
    opts.SetMessageStore<MyMessageStore>();
});

var app = builder.Build();
```

## Context Providers

System prompts, tools, quiet words, and speech options are provided via `IContextProvider` implementations registered in DI using the visitor pattern — each provider's `Apply(AiContext)` method populates a shared `AiContext`. The `AiContext` contains:

- `Acknowledgement` — the current acknowledgement mode (set by the service)
- `SystemPrompts` — system prompt strings to include in the chat request
- `Tools` — AI tools available for the request
- `QuietWords` — words that stop TTS and break the conversation loop (defaults provided)
- `SpeechToTextOptions` — speech recognition options (culture, silence timeout, etc.)
- `TextToSpeechOptions` — text-to-speech options (culture, voice, speech rate, etc.)

## AiServiceOptions

### SetChatClientProvider<T>()
- **Optional** — if not configured, a default `InjectedChatClientProvider` resolves `IChatClient` from DI
- Registers `T` as a singleton implementing `IChatClientProvider`
- Uses `TryAddSingleton` so only the first registration wins
- Use this for advanced scenarios like on-demand authentication or token refresh

### SetMessageStore<T>()
- **Optional** — without it, GetChatHistory/ClearChatHistory will throw
- Registers `T` as a singleton implementing `IMessageStore`
- The built-in `ContextProvider` automatically adds `ChatLookupAITool` to every request when a message store is present

### AddContextProvider<T>()
- Registers an additional `IContextProvider` implementation
- Multiple providers are supported and executed in sequence

### SetSoundProvider<T>()
- Registers a custom `ISoundProvider` implementation for audio feedback

## Auto-Registered Dependencies

`AddShinyAiConversation()` automatically registers (via TryAddSingleton):
- `TimeProvider.System`
- `ContextProvider` as an `IContextProvider` — uses the visitor pattern (`Apply(AiContext)`) to add time-based system prompts, acknowledgement-aware voice prompts, DI-registered `AITool` instances, and `ChatLookupAITool` when an `IMessageStore` is available. The `AiContext` also carries `QuietWords`, `SpeechToTextOptions`, and `TextToSpeechOptions` that providers can modify. `ContextProvider` can also be injected directly to add/remove system prompts, tools, and quiet words at runtime.
- `InjectedChatClientProvider` as the default `IChatClientProvider` — resolves `IChatClient` from DI. If no `IChatClient` is registered and no custom provider is set, an `InvalidOperationException` is thrown at runtime.
- Speech services via `AddSpeechServices()` (when `AutoAddSpeechServices` is true, the default) — includes ISpeechToTextService, ITextToSpeechService, and IAudioPlayer from Shiny.Speech

These can be overridden by registering your own implementations before calling `AddShinyAiConversation()`, or by setting `opts.AutoAddSpeechServices = false`.
