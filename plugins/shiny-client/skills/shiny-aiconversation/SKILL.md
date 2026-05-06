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
- **IAiConversationService**: Central orchestrator for AI interactions — manages state (Idle/Listening/Thinking/Responding), wake word detection, speech-to-text capture, chat client communication, text-to-speech response, acknowledgement modes, sound effects, and persistent chat history
- **IChatClientProvider**: Abstraction for obtaining an `IChatClient` (from Microsoft.Extensions.AI) — a default implementation (`InjectedChatClientProvider`) resolves `IChatClient` from DI; custom implementations handle authentication, token management, and client construction
- **IMessageStore**: Abstraction for persisting and querying chat message history — implementations provide storage (SQLite, file system, cloud, etc.)
- **ChatLookupAITool**: Optional AI tool that allows the AI to search past conversations via IMessageStore, registered as an `AITool` for Microsoft.Extensions.AI tool calling
- **AiChatMessage**: Record representing a persisted chat message with Id, Message, Timestamp, and Direction (User/AI)
- **AiServiceOptions**: Fluent configuration for DI registration — sets chat client provider, message store, and optional AI tools

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
- Add the optional ChatLookupAITool for AI-driven history search
- Build a chat UI that integrates with IAiConversationService
- Handle AI state changes (Idle, Listening, Thinking, Responding)
- Use TalkTo or ListenAndTalk for AI interactions

## Code Generation Instructions

### 1. Registration (MauiProgram.cs)

Always register with `AddShinyAiConversation()`:

```csharp
using Shiny.AiConversation;

// Register an IChatClient in DI (simplest approach)
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());

builder.Services.AddShinyAiConversation(opts =>
{
    opts.SetMessageStore<MyMessageStore>(addAiLookupTool: true); // optional
});
```

- `SetChatClientProvider<T>()` is **optional** — if not set, the default `InjectedChatClientProvider` resolves `IChatClient` from DI. Use it only for advanced scenarios (on-demand auth, token refresh, etc.)
- `SetMessageStore<T>()` is **optional** — enables persistent history and optionally registers the ChatLookupAITool
- Sound effects and system prompts are set on IAiConversationService **after** `builder.Build()`

### 2. Post-Build Configuration

```csharp
var app = builder.Build();
var aiService = app.Services.GetRequiredService<IAiConversationService>();

// System prompts
aiService.SystemPrompts.Add("You are a helpful assistant...");

// Sound resolver + sound file names (files in Resources/Raw/)
aiService.SoundResolver = name => FileSystem.OpenAppPackageFileAsync(name);
aiService.OkSound = "ok.mp3";
aiService.CancelSound = "cancel.mp3";
aiService.ErrorSound = "error.mp3";
aiService.ThinkSound = "think.mp3";
aiService.RespondingSound = "responding.mp3";
```

### 3. Chat Client Setup

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

### 4. Implementing IMessageStore

```csharp
public class MyMessageStore : IMessageStore
{
    public Task Store(AiChatMessage chatMessage, CancellationToken cancellationToken) { ... }
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
aiService.StateChanged += () => { /* update UI */ };
aiService.AiResponded += (response) =>
{
    // response.Message — the AI's text
    // response.Timestamp — when it was generated
    // response.WasReadAloud — whether TTS was used
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

1. **Set sounds externally** — Never set default sound values on AiConversationService directly; configure them in MauiProgram.cs after Build()
2. **Handle auth on-demand** — IChatClientProvider should handle authentication lazily, not force login at startup
3. **Use TwoWay binding** for Acknowledgement in settings UIs
4. **Subscribe/unsubscribe** to StateChanged and AiResponded in page lifecycle (OnAppearing/OnDisappearing)
5. **Use CancellationToken** for all TalkTo/ListenAndTalk calls
6. **MessageStore is optional** — The service works without it but GetChatHistory/ClearChatHistory will throw
7. **ChatLookupAITool is opt-in** — Pass `addAiLookupTool: true` to SetMessageStore to allow the AI to search past conversations
8. **No reflection** — All registrations must be explicit; do not use ActivatorUtilities.CreateInstance or reflection-based patterns
