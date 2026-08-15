---
name: shiny-aiconversation
description: Generate code for Shiny.AiConversation - a centralized AI service library for .NET MAUI apps with chat client abstraction, wake word detection, speech-to-text/text-to-speech, acknowledgement modes (None/AudioBlip/LessWordy/Full), structured AI turns with typed questions and multiple-choice answers, persistent message store, optional AI chat history lookup tool, and configurable sound effects
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
  - structured output
  - structured turn
  - aiturn
  - ai turn
  - aiquestion
  - ai question
  - aichoice
  - ai choice
  - multiple choice
  - choice buttons
  - showchoicebuttons
  - pending questions
  - pendingquestions
  - follow up timeout
  - followuptimeout
  - aistructuredoutputmode
  - structured output mode
  - json schema response
  - aiturnserializer
  - aichoicetemplateselector
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
  - voice selection
  - voice tools
  - change voice
  - play voice sample
  - get available voices
  - voiceselection
  - addvoiceselectiontools
  - add voice selection tools
  - voice sampling
  - tts voice
  - switch voice
  - aichatview
  - ai chat view
  - ai chat ui
  - maui chat control
  - chat control
  - chat screen
  - chat page
  - chat bubbles
  - shiny.aiconversation.maui
  - aichatsettings
  - aichatsessionprovider
  - addchatsessionprovider
  - chat session provider
  - ichatsessionprovider
  - bot name
  - bot avatar
  - chatbot avatar
  - greeting message
  - show token usage
  - microphone action
  - push to talk button
references:
  - ai-service.md
  - registration.md
  - message-store.md
  - chat-client-provider.md
  - chat-lookup-tool.md
  - voice-selection-tools.md
  - structured-turns.md
---

# Shiny.AiConversation Skill

You are an expert in the Shiny.AiConversation library, a centralized AI service for .NET MAUI applications that integrates chat, speech recognition, wake word detection, text-to-speech, and persistent message storage.

## Library Overview

**GitHub**: https://github.com/shinyorg/speech (Shiny.AiConversation lives under the `ai/` directory)
**NuGet**: `Shiny.AiConversation`
**Namespace**: `Shiny.AiConversation`
**Infrastructure Namespace**: `Shiny.AiConversation.Infrastructure` (internal implementations)

The library provides:
- **IAiConversationService**: Central orchestrator for AI interactions — manages access checking, state (Idle/Listening/Thinking/Responding), wake word detection, speech-to-text capture, chat client communication, text-to-speech response, acknowledgement modes, sound effects, persistent chat history, conversation continuation (keeps listening while a turn carries questions, bounded by `FollowUpTimeout`), and voice interruption (quiet words to stop TTS, or speak over the AI to redirect the conversation)
- **AiTurn / AiQuestion / AiChoice**: The structured reply shape. The model answers with `Reply` (what the user sees and hears) plus `Questions` (what it needs back, optionally with fixed `Choices`), so "keep listening" is a typed signal rather than a guess at the wording. Degrades to plain text automatically. See `structured-turns.md`
- **IChatClientProvider**: Abstraction for obtaining an `IChatClient` (from Microsoft.Extensions.AI) — a default implementation (`InjectedChatClientProvider`) resolves `IChatClient` from DI; custom implementations handle authentication, token management, and client construction
- **IMessageStore**: Abstraction for persisting and querying chat message history — implementations provide storage (SQLite, file system, cloud, etc.)
- **ChatLookupAITool**: AI tool that allows the AI to search past conversations via IMessageStore — automatically added by `ContextProvider` when an `IMessageStore` is registered
- **VoiceSelectionContextProvider**: Optional `IContextProvider` that adds three AI tools — `get_available_voices`, `play_voice_sample`, and `change_voice` — enabling the AI to list voices, play audio samples, and change its own TTS voice mid-conversation. Enabled via `opts.AddVoiceSelectionTools()`.
- **AiChatMessage**: Record representing a persisted chat message with Id, Message, Timestamp, and Direction (User/AI)
- **IContextProvider**: Visitor-pattern abstraction for populating an `AiContext` per request. Each provider's `Apply(AiContext)` method receives a mutable context and adds its contributions. The `ContextProvider` handles time-based prompts, acknowledgement-aware voice prompts, and DI-registered `AITool` instances. Implement custom providers to add domain-specific system prompts, tools, or override speech settings.
- **AiContext**: Mutable context object passed to `IContextProvider.Apply()` — contains `Acknowledgement`, `SystemPrompts`, `Tools`, `QuietWords`, `SpeechToTextOptions`, and `TextToSpeechOptions` that providers populate or modify
- **AiConversationOptions**: Fluent configuration for DI registration — sets chat client provider, message store, sound provider, and additional context providers via `AddContextProvider<T>()`

**Built-in Provider Packages**:
- **Shiny.AiConversation.OpenAi** (`OpenAiStaticChatProvider`): Static OpenAI-compatible provider. Accepts API key, endpoint URI, and model name. Works with OpenAI, Azure OpenAI, Ollama, or any OpenAI-compatible API. Register with `opts.AddStaticOpenAIChatClient(apiToken, endpointUri, modelName)`.
- **Shiny.AiConversation.Maui** (`AiChatView`): Drop-in .NET MAUI chat UI. `AiChatView` derives from the Shiny.Maui.Controls `ChatView` (so every base style/template property still applies) and wires the provider, session, message-store history paging, typing indicator, voice turns and errors to `IAiConversationService`. Also ships `AiChatSettings`, `AiChatSessionProvider` (an `IChatSessionProvider` usable with a plain `ChatView` via `opts.AddChatSessionProvider()`), and `AiMicrophoneInputAction`. XAML namespace: `http://shiny.net/maui/aiconversation`.
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
- Enable voice selection tools so the AI can list voices, play samples, and switch its own voice
- Build a chat UI that integrates with IAiConversationService (use `AiChatView` — do not hand-roll one)
- Style the chat screen, or set the chatbot's name/avatar/bubble colors
- Show chat history from the message store in the UI
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
- `AddVoiceSelectionTools()` is **optional** — registers `VoiceSelectionContextProvider`, giving the AI tools to list voices, play samples, and change its own voice
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
    // store assistantMessage, NOT response.Text - the latter is the raw structured JSON envelope
    public Task Store(string? userTriggeringMessage, string? assistantMessage, ChatResponse response, CancellationToken cancellationToken) { ... }
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
    // response.Text — the display reply. ALWAYS use this, never response.Response.Text
    //                 (that is the raw JSON envelope when structured output is on)
    // response.Turn — the parsed AiTurn, or null when the reply was plain text
    // response.Questions — questions carried by this turn (may have Choices)
    // response.WasReadAloud — whether TTS was used
    // response.ExpectsResponse — true when the AI is waiting on the user (listener stays open)
};

// The live question queue — replaced on every turn, never merged
foreach (var question in aiService.PendingQuestions)
    Console.WriteLine(question.Text);
```

### 6. Structured Turns & Multiple Choice

The AI answers with a typed `AiTurn` by default — see `structured-turns.md` for the full contract.
Key rules when generating code:

- Render and speak `AiResponse.Text`, **never** `AiResponse.Response.Text`
- Read "is the AI waiting on me" off `AiResponse.ExpectsResponse` / `PendingQuestions` — do not inspect
  the reply text for a question mark
- `PendingQuestions` is **replaced** each turn; never maintain a parallel queue
- Send an answer as plain text via `TalkTo`; do not fuzzy-match spoken answers to choices locally
- Always null-check `AiResponse.Turn` — any parse or provider failure degrades to plain text
- Set `IChatClientProvider.StructuredOutputMode` when writing a provider for an endpoint that rejects
  schema-constrained responses (`Json`, `Prompt`, or `None`)

```csharp
aiService.FollowUpTimeout = TimeSpan.FromSeconds(30);       // null waits indefinitely
aiService.StructuredOutputMode = AiStructuredOutputMode.None; // opt out entirely
```

### 7. Acknowledgement Modes

| Mode | Behavior |
|------|----------|
| `None` | No audio feedback or TTS |
| `AudioBlip` | Short sound effects at state transitions |
| `LessWordy` | TTS with "be concise" system prompt injected |
| `Full` | TTS with full unmodified responses |

### 8. AI States

| State | Description |
|-------|-------------|
| `Idle` | Ready for input |
| `Listening` | Actively listening for speech |
| `Thinking` | Waiting for AI to process |
| `Responding` | AI is streaming its response |

### 9. MAUI Chat UI (`Shiny.AiConversation.Maui`)

For any chat screen, use `AiChatView` — never hand-roll a message collection, send command, or
`IChatSessionProvider` over `IAiConversationService`. It resolves `IAiConversationService` from the
app's service provider, so there is nothing to bind:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:ai="http://shiny.net/maui/aiconversation"
             x:Class="MyApp.ChatPage"
             Title="Chat">

    <ai:AiChatView BotName="Aura"
                   BotAvatar="bot.png"
                   GreetingMessage="Hi! What can I help you with?"
                   ShowMicrophoneAction="True"
                   ShowTokenUsage="True"
                   MyBubbleColor="{StaticResource Primary}"
                   MyTextColor="White"
                   OtherBubbleColor="#F0EEFF"
                   BubbleCornerRadius="16"
                   PlaceholderText="Ask me something..." />
</ContentPage>
```

| Property | Default | Description |
|----------|---------|-------------|
| `AiService` | resolved from DI | The `IAiConversationService` to drive |
| `BotName` | `Assistant` | AI display name (also the session name) |
| `BotAvatar` | `null` | `ImageSource` for the AI |
| `BotBubbleColor` | `null` | AI bubble color; falls back to `OtherBubbleColor` |
| `UserName` / `UserAvatar` / `UserBubbleColor` | `Me` / `null` / `null` | Device user identity; color falls back to `MyBubbleColor` |
| `LoadHistory` | `true` | Backfill + page history from the registered `IMessageStore` |
| `GreetingMessage` | `null` | AI message shown when there is no history |
| `ShowTokenUsage` | `false` | Appends a token usage footer to AI messages |
| `ShowMicrophoneAction` | `false` | Push-to-talk action in the input bar (calls `ListenAndTalk`) |
| `MicrophoneActionText` | `🎤 Voice Input` | Label of that action |
| `ShowChoiceButtons` | `true` | Renders a button per `AiChoice` under AI bubbles that ask a multiple-choice question |
| `ChoiceSendText` | `Send` | Commit button label for questions allowing more than one choice |
| `Refresh()` | — | Method — reloads the conversation (call after `ClearChatHistory`) |

All `ChatView` properties are inherited and are the way to style the chat: `MyBubbleColor`,
`MyTextColor`, `OtherBubbleColor`, `OtherTextColor`, `ChatBackgroundColor`, `BubbleFontSize`,
`BubbleFontFamily`, `BubbleCornerRadius`, `TimestampFontSize`, `PlaceholderText`, `SendButtonText`,
`SendButtonBackgroundColor`, `SendButtonTextColor`, `InputBarBackgroundColor`, `InputBarBorderColor`,
`IsInputBarVisible`, `ShowTypingIndicator`, `MessageTemplate`, `MessageTemplateSelector`,
`InputActions`, `CustomBubbleActions`, `PageSize`, `UseFeedback`, `AdjustForKeyboard`
(set `False` inside a `FloatingPanel`).

Behavior that is already wired — do not re-implement it:
- Typed sends → `TalkTo`; replies (`AiResponded`) render as AI bubbles
- Voice turns from wake word or push-to-talk render as user bubbles (`SpeechOccurred` / `Heard`)
- History comes from the registered `IMessageStore`; with no store the chat starts empty (no exception)
- Typing indicator follows `AiState` (Thinking / Responding)
- `TalkTo` failures and `ErrorOccurred` render as AI bubbles with `Identifier = "error"`
- Multiple-choice turns render tappable chips under the bubble; the tapped label is sent as the answer. Setting `MessageTemplate` / `MessageTemplateSelector` takes precedence — then read choices with `AiChoiceTemplateSelector.ReadQuestions(chatMessage)`

To drive a plain `ChatView` instead, register the provider and bind `Provider` + `SessionId`:

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddGithubCopilotChatClient();
    opts.AddChatSessionProvider(cfg =>
    {
        cfg.BotName = "Aura";
        cfg.ShowTokenUsage = true;
    });
});
```

## Best Practices

1. **Use IContextProvider for configuration** — System prompts, tools, quiet words, and speech options are all configured via `IContextProvider.Apply(AiContext)` — not set directly on the service
2. **Handle auth on-demand** — IChatClientProvider should handle authentication lazily, not force login at startup
3. **Use TwoWay binding** for Acknowledgement in settings UIs
4. **Subscribe/unsubscribe** to StatusChanged and AiResponded in page lifecycle (OnAppearing/OnDisappearing)
5. **Use CancellationToken** for all TalkTo/ListenAndTalk calls
6. **MessageStore is optional** — The service works without it but GetChatHistory/ClearChatHistory will throw
7. **ChatLookupAITool is opt-in** — Pass `addAiLookupTool: true` to SetMessageStore to allow the AI to search past conversations
8. **No reflection** — All registrations must be explicit; do not use ActivatorUtilities.CreateInstance or reflection-based patterns
9. **Never render `Response.Text`** — use `AiResponse.Text`; with structured output the former is the raw JSON envelope
10. **Never re-derive "was that a question"** — `ExpectsResponse` and `PendingQuestions` are the typed signal
11. **Use AiChatView for chat UI** — do not build a message collection + send command by hand, and do not implement `IChatSessionProvider` over `IAiConversationService`; that bridge already ships in `Shiny.AiConversation.Maui`
