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
  - TextToSpeechOptions
  - VoiceInfo
  - AccessState
  - ContinuousRecognize
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
  - ListenWithWakeWord
  - ListenForKeyword
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
- Configure speech recognition options (language, silence timeout, on-device preference)
- Configure text-to-speech options (voice, rate, pitch, volume)
- List available TTS voices
- Implement continuous speech recognition with streaming results
- Implement listen-until-silence dictation
- Implement wake word detection ("Hey Siri" style activation)
- Implement keyword listening (listen until a specific keyword is detected)
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
- Platform-native text-to-speech via `ITextToSpeechService` (iOS, Android, Windows, Browser/WASM)
- Platform-native audio capture via `IAudioSource` (raw PCM 16kHz, 16-bit, mono — not supported in browser)
- Platform-native audio playback via `IAudioPlayer` (MP3 format; browser uses HTML5 Audio via base64 data URL)
- Pluggable cloud provider architecture via `ISpeechToTextProvider` and `ITextToSpeechProvider`
- Azure AI Speech integration (STT + TTS)
- ElevenLabs integration (TTS only)
- Streaming recognition results via `IAsyncEnumerable<SpeechRecognitionResult>`
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
builder.Services.AddAudioSource();    // Still need platform audio capture for STT
builder.Services.AddAudioPlayer();    // Still need platform audio playback for TTS
builder.Services.AddAzureSpeech("your-subscription-key", "eastus");
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
builder.Services.AddAudioPlayer();    // Still need platform audio playback
builder.Services.AddElevenLabsTextToSpeech("your-api-key");
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

> **Note:** `IAudioSource` (raw PCM capture) throws `PlatformNotSupportedException` in the browser. The Web Speech API handles audio capture internally.

## Code Generation Instructions

### 1. Speech-to-Text Usage

Always check permissions before using STT:

```csharp
public class MyViewModel(ISpeechToTextService stt)
{
    async Task StartListening()
    {
        var access = await stt.RequestAccess();
        if (access != AccessState.Available)
        {
            // Handle denied permission
            return;
        }

        // Listen until silence (simple dictation)
        var result = await stt.ListenUntilSilence(new SpeechRecognitionOptions
        {
            Culture = CultureInfo.GetCultureInfo("en-US"),
            SilenceTimeout = TimeSpan.FromSeconds(3),
            PreferOnDevice = true
        });

        if (result != null)
        {
            // Use transcribed text
        }
    }

    async Task StartContinuousListening(CancellationToken ct)
    {
        await foreach (var result in stt.ContinuousRecognize(cancellationToken: ct))
        {
            // result.Text — recognized text
            // result.IsFinal — true when segment is finalized
            // result.Confidence — optional confidence score (0-1)
        }
    }
}
```

### Wake Word Listening ("Hey Siri" style)

```csharp
public class MyViewModel(ISpeechToTextService stt)
{
    async Task ListenForWakeWord(CancellationToken ct)
    {
        var access = await stt.RequestAccess();
        if (access != AccessState.Available)
            return;

        // Continuously listens until wake phrase is detected,
        // then captures everything spoken after it until silence.
        // If user says wake phrase then pauses, it waits for the next utterance.
        var command = await stt.ListenWithWakeWord("Hey Computer", cancellationToken: ct);
        // "Hey Computer, what's the weather" → "what's the weather"
        // "Hey Computer" [pause] "what's the weather" → "what's the weather"
    }
}
```

### Keyword Listening

```csharp
public class MyViewModel(ISpeechToTextService stt)
{
    async Task ListenForAnswer(CancellationToken ct)
    {
        var access = await stt.RequestAccess();
        if (access != AccessState.Available)
            return;

        // Listens until one of the specified keywords is detected (case-insensitive, whole word)
        var answer = await stt.ListenForKeyword(["Yes", "No", "Maybe"], cancellationToken: ct);
        // User says "I think yes" → returns "Yes" (original casing from input list)
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

// Register in DI
builder.Services.AddAudioSource();
builder.Services.AddCloudSpeechToText<MyCloudSttProvider>();
```

## Best Practices

1. **Always check permissions** — Call `RequestAccess()` before STT operations
2. **Use cancellation tokens** — All async methods accept `CancellationToken` for proper cancellation
3. **Dispose audio resources** — `IAudioSource` and `IAudioPlayer` implement `IAsyncDisposable`
4. **Prefer `ListenUntilSilence`** — For simple dictation scenarios, use this over `ContinuousRecognize`
5. **Use `ContinuousRecognize`** — For real-time streaming transcription with partial results
6. **Use `ListenWithWakeWord`** — For "Hey Siri" style activation where a wake phrase triggers command capture
7. **Use `ListenForKeyword`** — For yes/no/choice scenarios where you need to detect a specific word from a set
6. **Register platform services** — Cloud providers still need `AddAudioSource()` and `AddAudioPlayer()` for microphone/speaker access
7. **Handle `AccessState`** — Check for `NotSupported`, `Denied`, and `Restricted` states
8. **Use `IsSpeaking`/`IsPlaying`** — Check state before starting new speech/playback
9. **Configure silence timeout** — Default 2 seconds; adjust for your use case
10. **Use `PreferOnDevice`** — Set to `true` for offline-capable STT when available
11. **Browser detection is automatic** — `AddSpeechServices()` uses `OperatingSystem.IsBrowser()` at runtime to register browser implementations; no conditional code needed in your app
12. **Browser audio capture is not supported** — `IAudioSource` throws `PlatformNotSupportedException` in browser; the Web Speech API handles audio internally
13. **Include the JS interop module** — Blazor WASM apps must include `shiny-speech.js` in `index.html` for speech services to work

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
