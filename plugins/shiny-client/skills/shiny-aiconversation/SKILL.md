---
name: shiny-aiconversation
description: Generate code for Shiny.AiConversation - a centralized AI service library for .NET MAUI apps with chat client abstraction, wake word detection, speech-to-text/text-to-speech, acknowledgement modes (None/AudioBlip/LessWordy/Full), persistent message store, optional AI chat history lookup tool, and configurable sound effects
auto_invoke: true
triggers:
  - shiny ai
  - shiny maui ai
  - ai service
  - aiservice
  - iai service
  - iaiservice
  - chat client provider
  - ichatclientprovider
  - message store
  - imessagestore
  - wake word
  - wakeword
  - listen and talk
  - speech to text
  - text to speech
  - ai acknowledgement
  - aiacknowledgement
  - audio blip
  - ai chat message
  - aichatmessage
  - chat history
  - ai tool lookup
  - chat lookup
  - talk to ai
  - ai response
  - ai state
  - addshinyaiconversation
  - add shiny ai conversation
  - ai conversation service
  - aiconversationservice
  - iaiconversationservice
  - ai service options
  - aiserviceoptions
  - quiet words
  - quietwords
  - voice interruption
  - speech interruption
  - expects response
  - conversation continuation
  - speech to text options
  - text to speech options
  - github copilot
  - copilot chat
  - addgithubcopilotchatclient
  - addstaticopenaichatclient
  - openai static
  - openaistaticChatprovider
  - request access
  - requestaccess
references:
  - ai-service.md
  - registration.md
  - message-store.md
  - chat-client-provider.md
  - chat-lookup-tool.md
---

# Shiny.AiConversation Skill

You are an expert in the Shiny.AiConversation library, a centralized AI service for .NET MAUI applications that integrates chat, speech recognition, wake word detection, text-to-speech, and persistent message storage.

## Library Overview

**NuGet**: `Shiny.AiConversation`
**Namespace**: `Shiny.AiConversation`
**Infrastructure Namespace**: `Shiny.AiConversation.Infrastructure` (internal implementations)

The library provides:
- **IAiConversationService**: Central orchestrator for AI interactions — manages access checking, state (Idle/Listening/Thinking/Responding), wake word detection, speech-to-text capture, chat client communication, text-to-speech response, acknowledgement modes, sound effects, persistent chat history, conversation continuation (auto-listens when AI asks a question), and voice interruption (quiet words to stop TTS, or speak over the AI to redirect the conversation)
- **IChatClientProvider**: Abstraction for obtaining an `IChatClient` (from Microsoft.Extensions.AI) — a default implementation (`InjectedChatClientProvider`) resolves `IChatClient` from DI; custom implementations handle authentication, token management, and client construction
- **IMessageStore**: Abstraction for persisting and querying chat message history — implementations provide storage (SQLite, file system, cloud, etc.)
- **ChatLookupAITool**: AI tool that allows the AI to search past conversations via IMessageStore — automatically added by `ContextProvider` when an `IMessageStore` is registered
- **AiChatMessage**: Record representing a persisted chat message with Id, Message, Timestamp, and Direction (User/AI)
- **IContextProvider**: Visitor-pattern abstraction for populating an `AiContext` per request. Each provider's `Apply(AiContext)` method receives a mutable context and adds its contributions. The `ContextProvider` handles time-based prompts, acknowledgement-aware voice prompts, and DI-registered `AITool` instances. Implement custom providers to add domain-specific system prompts, tools, or override speech settings.
- **AiContext**: Mutable context object passed to `IContextProvider.Apply()` — contains `Acknowledgement`, `SystemPrompts`, `Tools`, `QuietWords`, `SpeechToTextOptions`, and `TextToSpeechOptions` that providers populate or modify
- **AiConversationOptions**: Fluent configuration for DI registration — sets chat client provider, message store, sound provider, and additional context providers via `AddContextProvider<T>()`

**Built-in Provider Packages**:
- **Shiny.AiConversation.OpenAi** (`OpenAiStaticChatProvider`): Static OpenAI-compatible provider. Accepts API key, endpoint URI, and model name. Works with OpenAI, Azure OpenAI, Ollama, or any OpenAI-compatible API. Register with `opts.AddStaticOpenAIChatClient(apiToken, endpointUri, modelName)`.
- **Shiny.AiConversation.Maui.GithubCopilot** (`GitHubCopilotChatClientProvider`): MAUI-specific provider using GitHub device code OAuth flow and the Copilot API. Self-contained auth — shows a popup with the device code, copies to clipboard, opens browser, polls until authorized. Tokens stored in SecureStorage. Register with `opts.AddGithubCopilotChatClient()`. Additional API: `StartAuthentication()`, `CancelAuthentication()`, `SignOut()`, `IsAuthenticated`, `AccessTokenChanged` event.

## Dependencies

- `Microsoft.Extensions.AI` — IChatClient, ChatMessage, ChatRole, AITool, ChatOptions
- `Shiny.Speech` — ISpeechToTextService, ITextToSpeechService, IAudioPlayer for voice interactions and sound effects

## When to Use This Skill

Invoke this skill when the user wants to:
- Set up an AI chat service in a .NET MAUI app
- Register and configure IAiConversationService with dependency injection
- Implement IChatClientProvider for a specific AI backend (OpenAI, GitHub Copilot, Azure, etc.)
- Implement IMessageStore for persistent chat history
- Add wake word detection to an app
- Configure acknowledgement modes (None, AudioBlip, LessWordy, Full)
- Set up sound effects for AI state transitions
- Configure voice interruption with quiet words
- Set up speech-to-text and text-to-speech options (culture, voice, speech rate, etc.)
- Add the optional ChatLookupAITool for AI-driven history search
- Build a chat UI that integrates with IAiConversationService
- Handle AI state changes (Idle, Listening, Thinking, Responding)
- Use TalkTo or ListenAndTalk for AI interactions
- Check speech/microphone access before starting voice features

## Code Generation Instructions

### 1. Registration (MauiProgram.cs)

Always register with `AddShinyAiConversation()`:

```csharp
using Shiny.AiConversation;

// Option A: Register an IChatClient in DI (simplest approach)
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());

// Option B: Use a built-in provider
builder.Services.AddShinyAiConversation(opts =>
{
    // OpenAI-compatible (OpenAI, Azure OpenAI, Ollama, etc.)
    opts.AddStaticOpenAIChatClient("your-api-key", "https://api.openai.com/v1", "gpt-4o");

    // OR GitHub Copilot (MAUI only — self-contained auth with SecureStorage)
    opts.AddGithubCopilotChatClient();

    opts.SetMessageStore<MyMessageStore>(); // optional — ChatLookupAITool added automatically
});
```

- Built-in providers: `AddStaticOpenAIChatClient()` for OpenAI-compatible APIs, `AddGithubCopilotChatClient()` for GitHub Copilot on MAUI
- `SetChatClientProvider<T>()` is for custom providers — if not set and no built-in provider is used, the default `InjectedChatClientProvider` resolves `IChatClient` from DI
- `SetMessageStore<T>()` is **optional** — enables persistent history; the `ContextProvider` automatically adds `ChatLookupAITool` when a store is present
- `AddContextProvider<T>()` registers additional `IContextProvider` implementations
- `SetSoundProvider<T>()` registers a custom `ISoundProvider` implementation
- System prompts, tools, quiet words, and speech options are provided via `IContextProvider` implementations registered in DI (a `ContextProvider` is auto-registered)

### 2. Chat Client Setup

**Simple approach** — register `IChatClient` in DI (the default provider resolves it automatically):

```csharp
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());
```

**Advanced approach** — implement `IChatClientProvider` for on-demand auth or token refresh:

```csharp
public class MyChatClientProvider : IChatClientProvider
{
    public async Task<IChatClient> GetChatClient(CancellationToken cancelToken = default)
    {
        // Obtain/refresh tokens, authenticate if needed, build client
        return new OpenAIChatClient(...);
    }
}
```

- Handle token expiry and re-authentication inside GetChatClient
- Can inject INavigator to navigate to a login page if authentication is needed on-demand

### 3. Implementing IContextProvider (Custom)

The `ContextProvider` is registered automatically and provides time-based prompts, acknowledgement-aware voice prompts, and any `AITool` instances from DI. To add custom system prompts, tools, or modify speech settings, implement `IContextProvider` using the visitor pattern and register it:

```csharp
public class MyContextProvider : IContextProvider
{
    public Task Apply(AiContext context)
    {
        context.SystemPrompts.Add("You are a helpful assistant for our company.");
        // context.Tools.Add(...) to add AI tools
        // context.Acknowledgement is available to adjust behavior per mode
        // context.QuietWords — modify quiet words for voice interruption
        // context.SpeechToTextOptions — set/override speech recognition options
        // context.TextToSpeechOptions — set/override text-to-speech options
        return Task.CompletedTask;
    }
}

// Register in DI — multiple providers are supported, executed in sequence
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddContextProvider<MyContextProvider>();
});
```

### 4. Implementing IMessageStore

```csharp
public class MyMessageStore : IMessageStore
{
    public Task Store(string? userTriggeringMessage, ChatResponse response, CancellationToken cancellationToken) { ... }
    public Task Clear(DateTimeOffset? beforeDate = null) { ... }
    public Task<IReadOnlyList<AiChatMessage>> Query(
        string? messageContains = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        int? limit = null,
        CancellationToken cancellationToken = default) { ... }
}
```

### 5. Using IAiConversationService

```csharp
// Check access before using voice features
var access = await aiService.RequestAccess();
if (access != AccessState.Available)
{
    // Speech is not available — handle accordingly
    return;
}

// Send a text message
await aiService.TalkTo("What's the weather?", cancellationToken);

// Listen via microphone and send to AI
await aiService.ListenAndTalk(cancellationToken);

// Wake word detection (loops until stopped)
await aiService.StartWakeWord("Hey Assistant");
aiService.StopWakeWord();

// Query chat history
var history = await aiService.GetChatHistory(limit: 25);
var filtered = await aiService.GetChatHistory(messageContains: "weather", startDate: yesterday);

// Clear history
await aiService.ClearChatHistory();
await aiService.ClearChatHistory(beforeDate: oneWeekAgo);

// React to state changes
aiService.StatusChanged += (state) => { /* update UI with new AiState */ };
aiService.AiResponded += (response) =>
{
    // response.Response — the complete ChatResponse
    // response.Response.Text — the text content of the response
    // response.WasReadAloud — whether TTS was used
    // response.ExpectsResponse — true if AI asked a question (will auto-listen for reply)
};
```

### 6. Acknowledgement Modes

| Mode | Behavior |
|------|----------|
| `None` | No audio feedback or TTS |
| `AudioBlip` | Short sound effects at state transitions |
| `LessWordy` | TTS with "be concise" system prompt injected |
| `Full` | TTS with full unmodified responses |

### 7. AI States

| State | Description |
|-------|-------------|
| `Idle` | Ready for input |
| `Listening` | Actively listening for speech |
| `Thinking` | Waiting for AI to process |
| `Responding` | AI is streaming its response |

## Best Practices

1. **Use IContextProvider for configuration** — System prompts, tools, quiet words, and speech options are all configured via `IContextProvider.Apply(AiContext)` — not set directly on the service
2. **Handle auth on-demand** — IChatClientProvider should handle authentication lazily, not force login at startup
3. **Use TwoWay binding** for Acknowledgement in settings UIs
4. **Subscribe/unsubscribe** to StatusChanged and AiResponded in page lifecycle (OnAppearing/OnDisappearing)
5. **Use CancellationToken** for all TalkTo/ListenAndTalk calls
6. **MessageStore is optional** — The service works without it but GetChatHistory/ClearChatHistory will throw
7. **ChatLookupAITool is opt-in** — Pass `addAiLookupTool: true` to SetMessageStore to allow the AI to search past conversations
8. **No reflection** — All registrations must be explicit; do not use ActivatorUtilities.CreateInstance or reflection-based patterns
