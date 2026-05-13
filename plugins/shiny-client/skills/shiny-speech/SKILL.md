---
name: shiny-speech
description: Generate code using Shiny.Speech for cross-platform speech-to-text, text-to-speech, audio capture, and audio playback with pluggable cloud providers
auto_invoke: true
triggers:
  - speech to text
  - text to speech
  - speech recognition
  - voice recognition
  - tts
  - stt
  - speak
  - dictation
  - transcribe
  - synthesize speech
  - audio capture
  - audio playback
  - microphone
  - ISpeechToTextService
  - ITextToSpeechService
  - IAudioSource
  - IAudioPlayer
  - ISpeechToTextProvider
  - ITextToSpeechProvider
  - SpeechRecognitionResult
  - SpeechRecognitionOptions
  - SpeechRecognitionError
  - TextToSpeechOptions
  - VoiceInfo
  - AccessState
  - ResultReceived
  - KeywordHeard
  - StatementAfterKeyword
  - WaitListenForKeywords
  - ListenForKeywords
  - ListenUntilSilence
  - SpeakAsync
  - GetVoicesAsync
  - StartCaptureAsync
  - StopCaptureAsync
  - AddSpeechServices
  - AddSpeechToText
  - AddTextToSpeech
  - AddAudioSource
  - AddAudioPlayer
  - AddCloudSpeechToText
  - AddCloudTextToSpeech
  - AddAzureSpeech
  - AddElevenLabsTextToSpeech
  - AzureSpeechConfig
  - ElevenLabsConfig
  - CloudSpeechToText
  - CloudTextToSpeech
  - Shiny.Speech
  - Shiny.Speech.Cloud
  - Shiny.Speech.Azure
  - Shiny.Speech.ElevenLabs
  - PipeStream
  - IsListening
  - IsSpeaking
  - wake word
  - keyword detection
  - hey siri
  - voice activation
  - blazor speech
  - blazor wasm speech
  - browser speech
  - webassembly speech
  - web speech api
  - BrowserSpeechToTextService
  - BrowserTextToSpeechService
  - BrowserAudioPlayer
  - BrowserAudioSource
  - OperatingSystem.IsBrowser
---

# Shiny Speech Skill

You are an expert in Shiny Speech, a library that provides cross-platform speech-to-text, text-to-speech, audio capture, and audio playback for .NET MAUI and Blazor WebAssembly with pluggable cloud providers.

## When to Use This Skill

Invoke this skill when the user wants to:
- Add speech-to-text (STT) or text-to-speech (TTS) to a .NET MAUI app
- Capture audio from the device microphone
- Play audio streams on the device
- Use Azure AI Speech for cloud-based STT/TTS
- Use ElevenLabs for cloud-based TTS
- Implement a custom cloud speech provider
- Configure speech recognition options (language, silence timeout, on-device preference, keywords)
- Configure text-to-speech options (voice, rate, pitch, volume)
- List available TTS voices
- Start/stop continuous speech recognition with event-based results
- Implement listen-until-silence dictation
- Implement wake word / keyword activation ("Hey Siri" style)
- Implement keyword listening (listen until a specific keyword is detected)
- Listen for keywords continuously as an async stream
- Add speech-to-text or text-to-speech to a Blazor WebAssembly app
- Use the Web Speech API via Shiny.Speech in the browser

## Library Overview

**GitHub**: https://github.com/shinyorg/speech
**NuGet Packages**:
- `Shiny.Speech` — Core library with platform-native STT, TTS, audio capture, and playback
- `Shiny.Speech.Cloud` — Cloud provider abstractions
- `Shiny.Speech.Azure` — Azure AI Speech provider
- `Shiny.Speech.ElevenLabs` — ElevenLabs TTS provider

**Namespace**: `Shiny.Speech`

Shiny Speech provides:
- Platform-native speech-to-text via `ISpeechToTextService` (iOS, Android, Windows, Browser/WASM)
- Event-based recognition model — `ResultReceived`, `KeywordHeard`, `Error` events allow multiple subscribers
- Start/Stop lifecycle — call `Start()` to begin listening, `Stop()` to end; `Start()` throws if already listening
- Built-in keyword detection — set `Keywords` in `SpeechRecognitionOptions` and subscribe to `KeywordHeard`
- Platform-native text-to-speech via `ITextToSpeechService` (iOS, Android, Windows, Browser/WASM)
- Platform-native audio capture via `IAudioSource` (raw PCM 16kHz, 16-bit, mono — all platforms including browser)
- Platform-native audio playback via `IAudioPlayer` (MP3 format; browser uses HTML5 Audio via base64 data URL)
- Pluggable cloud provider architecture via `ISpeechToTextProvider` and `ITextToSpeechProvider`
- Azure AI Speech integration (STT + TTS)
- ElevenLabs integration (TTS only)
- Convenience extension methods: `ListenUntilSilence`, `StatementAfterKeyword`, `WaitListenForKeywords`, `ListenForKeywords`
- Permission management via `AccessState` and `RequestAccess()`

## Setup

### 1. Install NuGet Packages

For platform-native speech only:
```bash
dotnet add package Shiny.Speech
```

For Azure AI Speech (cloud STT + TTS):
```bash
dotnet add package Shiny.Speech
dotnet add package Shiny.Speech.Azure
```

For ElevenLabs (cloud TTS):
```bash
dotnet add package Shiny.Speech
dotnet add package Shiny.Speech.ElevenLabs
```

### 2. Configure in MauiProgram.cs (or Blazor Program.cs)

**Platform-native speech services:**
```csharp
builder.Services.AddSpeechServices(); // Registers STT, TTS, AudioSource, AudioPlayer
// On Browser/WASM: auto-detected via OperatingSystem.IsBrowser()
```

Or register individually:
```csharp
builder.Services.AddSpeechToText();   // ISpeechToTextService only
builder.Services.AddTextToSpeech();   // ITextToSpeechService only
builder.Services.AddAudioSource();    // IAudioSource only
builder.Services.AddAudioPlayer();    // IAudioPlayer only
```

**Azure AI Speech (replaces platform-native with cloud):**
```csharp
builder.Services.AddAzureSpeech("your-subscription-key", "eastus");
// Automatically registers IAudioSource and IAudioPlayer for platform audio I/O
```

Or with config object and selective services:
```csharp
builder.Services.AddAzureSpeech(
    new AzureSpeechConfig { SubscriptionKey = "key", Region = "eastus" },
    speechToText: true,
    textToSpeech: true
);
```

**ElevenLabs TTS (replaces platform-native TTS with cloud):**
```csharp
builder.Services.AddElevenLabsTextToSpeech("your-api-key");
// Automatically registers IAudioPlayer for platform audio playback
```

### 3. Platform Permissions

**Android** — Add to `AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

**iOS** — Add to `Info.plist`:
```xml
<key>NSSpeechRecognitionUsageDescription</key>
<string>This app uses speech recognition</string>
<key>NSMicrophoneUsageDescription</key>
<string>This app uses the microphone for speech recognition</string>
```

**Browser (Blazor WebAssembly)** — No manifest changes needed. The browser prompts the user for microphone access automatically. Include the JS interop module in `index.html`:
```html
<script src="shiny-speech.js"></script>
```

> **Note:** `IAudioSource` captures raw PCM audio in the browser using the Web Audio API (`getUserMedia` + `ScriptProcessorNode`), downsampled to 16kHz 16-bit mono — the same format as other platforms.

## Code Generation Instructions

### 1. Speech-to-Text Usage

Always check permissions before using STT. The service uses a Start/Stop model with events.

```csharp
public class MyViewModel(ISpeechToTextService stt)
{
    async Task StartListening()
    {
        var access = await stt.RequestAccess();
        if (access != AccessState.Available)
            return;

        // Subscribe to events (multiple subscribers allowed)
        stt.ResultReceived += (s, result) =>
        {
            // result.Text — recognized text
            // result.IsFinal — true when segment is finalized
            // result.Confidence — optional confidence score (0-1)
        };

        stt.KeywordHeard += (s, keyword) =>
        {
            // keyword — the matched keyword string
        };

        stt.Error += (s, error) =>
        {
            // error.Message — error description
            // error.Exception — optional exception
        };

        // Start listening (throws InvalidOperationException if already listening)
        await stt.Start(new SpeechRecognitionOptions
        {
            Culture = CultureInfo.GetCultureInfo("en-US"),
            SilenceTimeout = TimeSpan.FromSeconds(3),
            PreferOnDevice = true,
            Keywords = ["Yes", "No", "Maybe"]  // optional keyword detection
        });
    }

    async Task StopListening()
    {
        await stt.Stop(); // no-op if not listening
    }
}
```

### Extension Methods (Convenience Patterns)

```csharp
public class MyViewModel(ISpeechToTextService stt)
{
    async Task SimpleDictation(CancellationToken ct)
    {
        // Listen until silence — starts, waits for first final result, stops
        var text = await stt.ListenUntilSilence(
            new SpeechRecognitionOptions
            {
                Culture = CultureInfo.GetCultureInfo("en-US"),
                SilenceTimeout = TimeSpan.FromSeconds(3)
            },
            ct
        );
    }

    async Task WakeWordActivation(CancellationToken ct)
    {
        // "Hey Computer, do something" → returns "do something"
        // Waits for keyword, then captures next final statement
        var command = await stt.StatementAfterKeyword(
            ["Hey Computer"],
            cancellationToken: ct
        );
    }

    async Task WaitForAnswer(CancellationToken ct)
    {
        // Wait for one specific keyword (with optional timeout)
        var answer = await stt.WaitListenForKeywords(
            ["Yes", "No", "Maybe"],
            timeout: TimeSpan.FromSeconds(30),
            cancellationToken: ct
        );
        // Returns matched keyword or null on timeout
    }

    async Task ContinuousKeywords(CancellationToken ct)
    {
        // Stream keywords continuously as IAsyncEnumerable
        await foreach (var keyword in stt.ListenForKeywords(
            ["Up", "Down", "Left", "Right"],
            cancellationToken: ct))
        {
            Console.WriteLine($"Direction: {keyword}");
        }
    }
}
```

### 2. Text-to-Speech Usage

```csharp
public class MyViewModel(ITextToSpeechService tts)
{
    async Task Speak()
    {
        // Simple speech
        await tts.SpeakAsync("Hello, world!");

        // With options
        await tts.SpeakAsync("Hello, world!", new TextToSpeechOptions
        {
            SpeechRate = 1.2f,
            Pitch = 1.0f,
            Volume = 0.8f,
            Culture = CultureInfo.GetCultureInfo("en-US")
        });

        // List available voices
        var voices = await tts.GetVoicesAsync();
        var voice = voices.FirstOrDefault(v => v.Name.Contains("Neural"));

        // Speak with specific voice
        await tts.SpeakAsync("Hello!", new TextToSpeechOptions { Voice = voice });

        // Stop speaking
        if (tts.IsSpeaking)
            await tts.StopAsync();
    }
}
```

### 3. Audio Capture

```csharp
public class MyViewModel(IAudioSource audioSource)
{
    async Task CaptureAudio(CancellationToken ct)
    {
        // Returns raw PCM stream (16kHz, 16-bit, mono)
        await using var stream = await audioSource.StartCaptureAsync(ct);

        // Read audio data from stream...
        // Stream remains open until StopCaptureAsync is called

        await audioSource.StopCaptureAsync();
    }
}
```

### 4. Audio Playback

```csharp
public class MyViewModel(IAudioPlayer audioPlayer)
{
    async Task PlayAudio(Stream mp3Stream, CancellationToken ct)
    {
        // Play MP3 format audio
        await audioPlayer.PlayAsync(mp3Stream, ct);

        // Check playback state
        if (audioPlayer.IsPlaying)
            await audioPlayer.StopAsync();
    }
}
```

### 5. Custom Cloud Provider

Implement `ISpeechToTextProvider` and/or `ITextToSpeechProvider`:

```csharp
public class MyCloudSttProvider : ISpeechToTextProvider
{
    public async IAsyncEnumerable<SpeechRecognitionResult> RecognizeAsync(
        Stream audioStream,
        SpeechRecognitionOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Send audioStream to your cloud API
        // Yield results as they arrive
        yield return new SpeechRecognitionResult("Hello", IsFinal: true, Confidence: 0.95f);
    }
}

// Register in DI (IAudioSource is auto-registered)
builder.Services.AddCloudSpeechToText<MyCloudSttProvider>();
```

## Best Practices

1. **Always check permissions** — Call `RequestAccess()` before STT operations
2. **Use Start/Stop lifecycle** — Call `Start()` to begin, `Stop()` to end; `Start()` throws if already listening
3. **Subscribe before starting** — Attach event handlers before calling `Start()` to avoid missing results
4. **Unsubscribe on stop** — Remove event handlers after calling `Stop()` to avoid leaks
5. **Use extension methods for common patterns** — `ListenUntilSilence`, `StatementAfterKeyword`, `WaitListenForKeywords`, `ListenForKeywords` handle Start/Stop/event wiring for you
6. **Dispose audio resources** — `IAudioSource` and `IAudioPlayer` implement `IAsyncDisposable`
7. **Use `ListenUntilSilence`** — For simple dictation scenarios
8. **Use `StatementAfterKeyword`** — For "Hey Siri" style wake word activation
9. **Use `WaitListenForKeywords`** — For yes/no/choice scenarios
10. **Use `ListenForKeywords`** — For continuous keyword detection as an async stream
11. **Platform services auto-registered** — Cloud providers automatically register `IAudioSource` and `IAudioPlayer` as needed via `TryAdd`, so manual registration is no longer required
12. **Handle `AccessState`** — Check for `NotSupported`, `Denied`, and `Restricted` states
13. **Use `IsListening`/`IsSpeaking`/`IsPlaying`** — Check state before starting new listening/speech/playback
14. **Configure silence timeout** — Default 2 seconds; adjust for your use case
15. **Use `PreferOnDevice`** — Set to `true` for offline-capable STT when available
16. **Browser detection is automatic** — `AddSpeechServices()` uses `OperatingSystem.IsBrowser()` at runtime to register browser implementations; no conditional code needed in your app
17. **Browser audio capture is supported** — `IAudioSource` captures raw PCM via the Web Audio API (`getUserMedia` + `ScriptProcessorNode`), downsampled to 16kHz 16-bit mono
18. **Include the JS interop module** — Blazor WASM apps must include `shiny-speech.js` in `index.html` for speech services to work
19. **CarPlay compatible** — iOS audio session uses `PlayAndRecord` with `AllowBluetooth` / `AllowBluetoothA2dp` / `DefaultToSpeaker`, so when CarPlay is active iOS automatically routes audio through the car's microphone and speakers — no CarPlay-specific code needed

## Reference Files

For detailed API documentation, see:
- `reference/api-reference.md` - Full API surface, interfaces, records, and configuration

## Common Packages

```bash
dotnet add package Shiny.Speech                  # Core platform-native speech services
dotnet add package Shiny.Speech.Cloud            # Cloud provider abstractions (included by Azure/ElevenLabs)
dotnet add package Shiny.Speech.Azure            # Azure AI Speech provider
dotnet add package Shiny.Speech.ElevenLabs       # ElevenLabs TTS provider
```
