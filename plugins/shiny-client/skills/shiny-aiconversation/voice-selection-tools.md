# VoiceSelectionTools

## Overview

An optional set of three AI tools that allow the AI to list available text-to-speech voices, play voice samples, and change its own speaking voice during a conversation.

**Namespace**: `Shiny.AiConversation.Infrastructure`  
**Class**: `VoiceSelectionContextProvider` (implements `IContextProvider`)

## Tools Exposed

| Tool name | Parameters | Description |
|-----------|-----------|-------------|
| `get_available_voices` | `culture?` (BCP-47 string) | Returns a formatted list of all available TTS voices, optionally filtered by culture |
| `play_voice_sample` | `voiceId`, `sampleText?` | Speaks a sample phrase using the specified voice so the user can hear it |
| `change_voice` | `voiceId` | Changes the AI's TTS voice; the selection persists for the lifetime of the service |

## Registration

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddStaticOpenAIChatClient(...);
    opts.AddVoiceSelectionTools();
});
```

Requires `ITextToSpeechService`, which is auto-registered when `AutoAddSpeechServices = true` (the default).

## How It Works

`VoiceSelectionContextProvider` is registered as a singleton `IContextProvider`. Its `Apply(AiContext)` method adds the three tools to `context.Tools` on every request. When `change_voice` is called, the tool writes directly to `IAiConversationService.TextToSpeechOptions` — this is the same property the service uses for all TTS output, so the new voice takes effect immediately on the confirmation response and all subsequent responses. `IAiConversationService` is resolved lazily from `IServiceProvider` to avoid a circular DI dependency.

The `play_voice_sample` tool calls `ITextToSpeechService.SpeakAsync()` directly during the AI's thinking phase (before the AI's verbal response), so there is no audio overlap.

## Example Conversation Flow

```
User:  "Can you play a few different voices for me?"
AI:    [calls get_available_voices → gets list]
       [calls play_voice_sample for 2–3 voices]
       "I just played three voices. Which one did you prefer?"
User:  "The second one"
AI:    [calls change_voice with that voice ID]
       "Done! I'll use that voice from now on." ← spoken with the new voice
```

## Disabling

Simply do not call `AddVoiceSelectionTools()`. The tools are entirely opt-in.
