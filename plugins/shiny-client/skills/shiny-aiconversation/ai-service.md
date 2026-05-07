# IAiConversationService API Reference

## Interface: `IAiConversationService`

**Namespace**: `Shiny.AiConversation`

The central orchestrator for all AI interactions in a .NET MAUI app.

## Events

### StatusChanged
```csharp
event Action<AiState> StatusChanged;
```
Raised when the service state changes, passing the new `AiState`. Use `MainThread.BeginInvokeOnMainThread()` when updating UI in response.

### AiResponded
```csharp
event Action<AiResponse>? AiResponded;
```
Raised when the AI produces a complete response. The `AiResponse` record contains:
- `Response` (ChatResponse) — the complete chat response including text, tool calls, and usage details
- `WasReadAloud` (bool) — whether text-to-speech was used based on the current Acknowledgement mode

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `WakeWord` | `string?` | Currently active wake word, or null if not running |
| `Status` | `AiState` | Current processing state (Idle/Listening/Thinking/Responding) |
| `Acknowledgement` | `AiAcknowledgement` | Controls response delivery mode (get/set) |
| `SystemPrompts` | `IList<string>` | System prompts prepended to every chat request |
| `CurrentChatMessages` | `IReadOnlyList<ChatMessage>` | In-memory chat messages for current session |
| `SoundResolver` | `Func<string, Task<Stream>>?` | Callback that resolves a sound file name to a playable stream |
| `OkSound` | `string?` | Sound file name played on successful interaction (AudioBlip mode) |
| `CancelSound` | `string?` | Sound file name played on cancellation (AudioBlip mode) |
| `ErrorSound` | `string?` | Sound file name played on error (AudioBlip mode) |
| `ThinkSound` | `string?` | Sound file name played when AI begins processing (AudioBlip mode) |
| `RespondingSound` | `string?` | Sound file name played when AI begins streaming response (AudioBlip mode) |

## Methods

### TalkTo
```csharp
Task TalkTo(string message, CancellationToken cancellationToken);
```
Sends a text message to the AI. Manages the full lifecycle: Thinking → Responding → Idle. Stores messages in IMessageStore if configured. Streams response and uses TTS if Acknowledgement is LessWordy or Full.

### ListenAndTalk
```csharp
Task ListenAndTalk(CancellationToken cancellationToken);
```
Activates speech-to-text for a single utterance, then sends it via TalkTo. Throws if wake word is active.

### StartWakeWord
```csharp
Task StartWakeWord(string wakeWord);
```
Begins continuous wake word detection. On detection, captures utterance via STT and sends to AI. Loops until StopWakeWord is called. Throws if already active.

### StopWakeWord
```csharp
void StopWakeWord();
```
Stops the active wake word detection loop.

### GetChatHistory
```csharp
Task<IReadOnlyList<AiChatMessage>> GetChatHistory(
    string? messageContains = null,
    DateTimeOffset? startDate = null,
    DateTimeOffset? endDate = null,
    int? limit = null
);
```
Retrieves persisted chat history. Throws if IMessageStore is not configured.

### ClearChatHistory
```csharp
Task ClearChatHistory(DateTimeOffset? beforeDate = null, CancellationToken cancellationToken = default);
```
Clears persisted history. If `beforeDate` is specified, only removes older messages.

### ClearCurrentChat
```csharp
void ClearCurrentChat();
```
Clears in-memory chat messages only. Does not affect persisted history.

## Enums

### AiState
| Value | Description |
|-------|-------------|
| `Idle` | Ready for input |
| `Listening` | Actively listening for speech |
| `Thinking` | Waiting for AI to process |
| `Responding` | AI is streaming its response |

### AiAcknowledgement
| Value | Description |
|-------|-------------|
| `None` | No audio feedback or TTS |
| `AudioBlip` | Short sound effects at state transitions |
| `LessWordy` | TTS with concise system prompt injected |
| `Full` | TTS with full unmodified responses |

## Records

### AiResponse
```csharp
public record AiResponse(ChatResponse Response, bool WasReadAloud);
```

### AiChatMessage
```csharp
public record AiChatMessage(string Id, string Message, DateTimeOffset Timestamp, ChatMessageDirection Direction);
```

### ChatMessageDirection
```csharp
public enum ChatMessageDirection { User, AI }
```
