# Registration & Configuration

## DI Registration

Register the AI service in `MauiProgram.cs` using `AddShinyAiConversation()`:

```csharp
using Shiny.AiConversation;

var builder = MauiApp.CreateBuilder();

builder.Services.AddShinyAiConversation(opts =>
{
    // Required: set the chat client provider
    opts.SetChatClientProvider<MyChatClientProvider>();

    // Optional: enable persistent message storage
    // Pass addAiLookupTool: true to register ChatLookupAITool
    opts.SetMessageStore<MyMessageStore>(addAiLookupTool: true);
});

var app = builder.Build();
```

## Post-Build Configuration

Sound effects and system prompts must be set **after** `builder.Build()` on the resolved IAiConversationService instance:

```csharp
var aiService = app.Services.GetRequiredService<IAiConversationService>();

// Add system prompts
aiService.SystemPrompts.Add(
    """
    You are a helpful assistant. If you don't know the answer,
    it's okay to say you don't know.
    """
);

// Sound resolver + sound file names (files must exist in Resources/Raw/)
aiService.SoundResolver = name => FileSystem.OpenAppPackageFileAsync(name);
aiService.OkSound = "ok.mp3";
aiService.CancelSound = "cancel.mp3";
aiService.ErrorSound = "error.mp3";
aiService.ThinkSound = "think.mp3";
aiService.RespondingSound = "responding.mp3";
```

## AiServiceOptions

### SetChatClientProvider<T>()
- **Required** — an `InvalidOperationException` is thrown at startup if not configured
- Registers `T` as a singleton implementing `IChatClientProvider`
- Uses `TryAddSingleton` so only the first registration wins

### SetMessageStore<T>(bool addAiLookupTool = true)
- **Optional** — without it, GetChatHistory/ClearChatHistory will throw
- Registers `T` as a singleton implementing `IMessageStore`
- When `addAiLookupTool` is true, also registers:
  - `ChatLookupAITool` as a singleton
  - An `AITool` singleton that the AI can invoke to search past conversations

## Auto-Registered Dependencies

`AddShinyAiConversation()` automatically registers (via TryAddSingleton):
- `TimeProvider.System`
- Speech services via `AddSpeechServices()` (when `AutoAddSpeechServices` is true, the default) — includes ISpeechToTextService, ITextToSpeechService, and IAudioPlayer from Shiny.Speech

These can be overridden by registering your own implementations before calling `AddShinyAiConversation()`, or by setting `opts.AutoAddSpeechServices = false`.
