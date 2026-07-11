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
  - IAudio
  - IAudioMonitor
  - IAudioDevices
  - AudioDevice
  - AudioMonitorOptions
  - microphone monitor
  - mic passthrough
  - live monitoring
  - PA
  - megaphone
  - talk over bluetooth speaker
  - audio route
  - output device
  - input device
  - device selection
  - AddAudioMonitor
  - AddAudioDevices
  - SetInputDevice
  - SetOutputDevice
  - InputLevelChanged
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
  - AudioProcessingOptions
  - noise suppression
  - background noise
  - echo cancellation
  - acoustic echo cancellation
  - AEC
  - automatic gain control
  - AGC
  - voice processing
  - barge-in
  - EchoCancellation
  - NoiseSuppression
  - AutomaticGainControl
  - AddSpeechServices
  - AddSpeechToText
  - AddTextToSpeech
  - AddAudioServices
  - AddAudioSource
  - AddAudioPlayer
  - AddCloudSpeechToText
  - AddCloudTextToSpeech
  - AddAzureSpeech
  - AddElevenLabsTextToSpeech
  - AddElevenLabsSpeechToText
  - AddElevenLabsSpeech
  - ElevenLabsSpeechToTextProvider
  - Scribe
  - scribe_v1
  - AzureSpeechConfig
  - ElevenLabsConfig
  - AddTypecastSpeech
  - AddTypecastTextToSpeech
  - Typecast
  - TypecastConfig
  - TypecastTextToSpeechProvider
  - CloudSpeechToText
  - CloudTextToSpeech
  - Shiny.Speech
  - Shiny.Audio
  - Shiny.Core
  - Shiny.Hosting.Maui
  - UseShiny
  - AndroidPlatform
  - Shiny.Speech.Cloud
  - Shiny.Speech.Azure
  - Shiny.Speech.ElevenLabs
  - Shiny.Speech.Typecast
  - PipeStream
  - IsListening
  - IsSpeaking
  - AudioLevelChanged
  - IsPlayerAnalysisSupported
  - VU meter
  - audio level
  - Volume
  - set volume
  - read volume
  - VolumeChanged
  - IsVolumeControlSupported
  - system volume
  - media volume
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
- `Shiny.Speech.Typecast` — Typecast TTS provider (TTS only, via the `typecast-csharp` SDK)

**Namespace**: `Shiny.Speech`

Shiny Speech provides:
- Platform-native speech-to-text via `ISpeechToTextService` (iOS, Android, Windows, Browser/WASM)
- Event-based recognition model — `ResultReceived`, `KeywordHeard`, `Error` events allow multiple subscribers
- Start/Stop lifecycle — call `Start()` to begin listening, `Stop()` to end; `Start()` throws if already listening
- Built-in keyword detection — set `Keywords` in `SpeechRecognitionOptions` and subscribe to `KeywordHeard`
- Platform-native text-to-speech via `ITextToSpeechService` (iOS, Android, Windows, Browser/WASM)
- Platform-native audio capture via `IAudioSource` (raw PCM 16kHz, 16-bit, mono — all platforms including browser)
- Platform-native audio playback via `IAudioPlayer` — play a `Stream`, or a remote URL / local file path via `PlayAsync(string)` (platform resolves the source natively; browser uses HTML5 Audio)
- Live microphone monitor via `IAudioMonitor` — routes the mic to the current output in near-real-time (PA / "talk over a Bluetooth speaker"); `Start`/`Stop`, adjustable `Gain`, `InputLevelChanged` VU signal, `AudioMonitorOptions` (voice processing + preferred devices), `SetInputDevice`/`SetOutputDevice`. iOS/Mac Catalyst + Android only
- Audio route enumeration/selection via `IAudioDevices` — `GetInputs`/`GetOutputs`, `CurrentInput`/`CurrentOutput`, `Changed` event; normalized `AudioDevice.Type`. iOS/Mac Catalyst + Android only
- One-stop `IAudio` facade exposing `Player` / `Source` / `Monitor` / `Devices` — inject it to discover the whole audio surface (focused interfaces remain independently injectable)
- Pluggable cloud provider architecture via `ISpeechToTextProvider` and `ITextToSpeechProvider`
- Azure AI Speech integration (STT + TTS)
- ElevenLabs integration (Scribe STT + TTS)
- Typecast integration (TTS only)
- Convenience extension methods: `ListenUntilSilence`, `StatementAfterKeyword`, `WaitListenForKeywords`, `ListenForKeywords`
- Permission management via `AccessState` and `RequestAccess()`
- VU meter signal — `AudioLevelChanged` event on `ITextToSpeechService` and `IAudioPlayer` emits a normalized 0.0–1.0 RMS level during playback; `IsPlayerAnalysisSupported` reports per-platform availability

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
builder
    .UseMauiApp<App>()
    .UseShiny(); // REQUIRED for native STT/audio — registers Shiny.Core's AndroidPlatform

builder.Services.AddSpeechServices(); // Registers STT, TTS, AudioSource, AudioPlayer
// On Browser/WASM: auto-detected via OperatingSystem.IsBrowser()
```

> **`UseShiny()` is mandatory for native speech/audio.** Android runtime permission requests
> (`RECORD_AUDIO`) and current-activity tracking are delegated to `Shiny.Core`'s `AndroidPlatform`.
> Reference the `Shiny.Hosting.Maui` package and call `.UseShiny()` on the `MauiAppBuilder` (before
> building) so `AndroidPlatform` is registered and receives `OnRequestPermissionsResult` callbacks.
> Without it, `RequestAccess()` throws a `TimeoutException` ("no current activity"). This replaced the
> library's old self-contained `ActivityProvider` + `PermissionRequestFragment`.

Or register individually:
```csharp
builder.Services.AddSpeechToText();   // ISpeechToTextService only
builder.Services.AddTextToSpeech();   // ITextToSpeechService only
builder.Services.AddAudioServices();  // IAudioSource + IAudioPlayer + IAudioMonitor + IAudioDevices + IAudio (from Shiny.Audio)
builder.Services.AddAudioSource();    // IAudioSource only
builder.Services.AddAudioPlayer();    // IAudioPlayer only
builder.Services.AddAudioMonitor();   // IAudioMonitor only (iOS/Mac Catalyst + Android)
builder.Services.AddAudioDevices();   // IAudioDevices only (iOS/Mac Catalyst + Android)
```

> **Namespace:** `IAudioSource`, `IAudioPlayer`, and `PipeStream` live in the **`Shiny.Audio`**
> namespace (shipped in the standalone `Shiny.Audio` package, referenced by `Shiny.Speech`). Add
> `using Shiny.Audio;` when consuming them. `AccessState` now comes from **`Shiny.Core`** and lives in
> the **`Shiny`** namespace — because `Shiny` is a parent namespace, code inside `Shiny.Audio`/
> `Shiny.Speech` resolves it automatically; add `using Shiny;` only where you reference it elsewhere.
> All the DI extension methods above are in the `Shiny` namespace regardless of package.
> `AddAudioServices()` / `AddAudioSource()` / `AddAudioPlayer()` come from `Shiny.Audio` and can be used
> **without** `Shiny.Speech` for capture/playback-only scenarios (still require `.UseShiny()` on
> Android).

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

**ElevenLabs (replaces platform-native STT/TTS with cloud — Scribe + TTS):**
```csharp
// Register both STT (Scribe) and TTS at once
builder.Services.AddElevenLabsSpeech("your-api-key");

// Or selectively
builder.Services.AddElevenLabsSpeechToText("your-api-key"); // Scribe STT only
builder.Services.AddElevenLabsTextToSpeech("your-api-key"); // TTS only
// Auto-registers IAudioSource and/or IAudioPlayer for platform audio I/O as needed
```

```csharp
// With a config object — overrides default Scribe model / TTS model / voice
builder.Services.AddElevenLabsSpeech(new ElevenLabsConfig
{
    ApiKey = "your-api-key",
    SpeechToTextModel = "scribe_v1",
    TextToSpeechModel = "eleven_multilingual_v2",
    DefaultVoiceId = "21m00Tcm4TlvDq8ikWAM"
});
```

> **ElevenLabs Scribe is request/response, not streaming**: results are yielded as a single final `SpeechRecognitionResult` when the user calls `Stop()` (the captured audio is buffered, wrapped in a WAV container, and posted to `/v1/speech-to-text`). For continuous partial results, use Azure instead.

**Typecast (cloud TTS only — via the `typecast-csharp` SDK):**
```csharp
builder.Services.AddTypecastSpeech("your-typecast-api-key");
// AddTypecastTextToSpeech(...) is an identical alias. Registers ITextToSpeechService + IAudioPlayer.

// With a config object — model, default voice, language, emotion, audio format:
builder.Services.AddTypecastSpeech(new TypecastConfig
{
    ApiKey = "your-typecast-api-key",
    DefaultVoiceId = "<voice-id>",           // required unless you pass TextToSpeechOptions.Voice per call
    Model = Typecast.Models.TTSModel.SsfmV30,
    AudioFormat = Typecast.Models.AudioFormat.Mp3
});
```

> **Typecast is TTS-only** — there is no `AddTypecastSpeechToText`; pair it with Azure/ElevenLabs/OpenAI or native STT if you need recognition. It has **no fixed default voice**: set `TypecastConfig.DefaultVoiceId` or pass `TextToSpeechOptions.Voice`, and call `ITextToSpeechService.GetVoicesAsync()` to discover the voice ids available to your account. `TypecastConfig` also exposes optional `Language`, `Emotion` (+`EmotionIntensity`) hints.

**Changing API keys / credentials at runtime:**
All cloud provider config objects (`AzureSpeechConfig`, `ElevenLabsConfig`, `OpenAiSpeechConfig`, `TypecastConfig`) are **mutable singletons**. Register them normally, then change the key (or region/model/voice) at any time — the provider uses the new value on its next call, no re-registration needed. Keep a reference to the config you pass in, or resolve it from DI:
```csharp
var config = new TypecastConfig { ApiKey = "initial" };
builder.Services.AddTypecastSpeech(config);
// ...later:
config.ApiKey = "rotated-key";                              // via your retained reference
serviceProvider.GetRequiredService<AzureSpeechConfig>().SubscriptionKey = "new-key"; // or resolve from DI
```
Providers that cache an SDK/HTTP client (ElevenLabs, Typecast) rebuild it automatically when the key changes (via `RefreshableClient<T>` in `Shiny.Speech.Cloud`); Azure and OpenAI read the config on every call. Do **not** re-call `AddXxxSpeech(...)` to change a key — just mutate the config.

### 3. Platform Permissions

**Android** — Add to `AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS" />
```
`MODIFY_AUDIO_SETTINGS` is required for the TTS audio-level Visualizer and for the native STT beep suppression.

**iOS** — Add to `Info.plist`:
```xml
<key>NSSpeechRecognitionUsageDescription</key>
<string>This app uses speech recognition</string>
<key>NSMicrophoneUsageDescription</key>
<string>This app uses the microphone for speech recognition</string>
```

**Browser (Blazor WebAssembly)** — No manifest changes and **no `<script>` tag** needed. The browser prompts for microphone access automatically, and the JS interop module ships **inside the `Shiny.Audio` package** as a static web asset (`_content/Shiny.Audio/shiny-audio.js`), loaded on demand via `JSHost.ImportAsync`. Do **not** copy the JS into `wwwroot` or add a `<script>` reference — just reference the NuGet package.

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
using Shiny.Audio; // IAudioSource, IAudioPlayer, PipeStream
using Shiny;       // AccessState (from Shiny.Core)

public class MyViewModel(IAudioSource audioSource)
{
    async Task CaptureAudio(CancellationToken ct)
    {
        // Returns raw PCM stream (16kHz, 16-bit, mono).
        // NOTE: StartCaptureAsync(AudioProcessingOptions?, CancellationToken) — the first
        // parameter is the processing options, so pass the token by name (or null first).
        await using var stream = await audioSource.StartCaptureAsync(cancellationToken: ct);

        // Read audio data from stream...
        // Stream remains open until StopCaptureAsync is called

        await audioSource.StopCaptureAsync();
    }
}
```

#### Voice Processing (noise suppression & echo cancellation)

Request platform voice-processing effects via `AudioProcessingOptions` to strip background noise and
cancel the device's own speaker/TTS output from the mic (barge-in) so it isn't re-captured:

```csharp
using Shiny.Audio;

// All three effects (AEC + noise suppression + AGC)
var stream = await audioSource.StartCaptureAsync(AudioProcessingOptions.VoiceChat, ct);

// Or select individually
var stream2 = await audioSource.StartCaptureAsync(new AudioProcessingOptions
{
    EchoCancellation = true,       // subtracts speaker/TTS output — stops TTS bleeding into the mic
    NoiseSuppression = true,       // attenuates steady background noise
    AutomaticGainControl = true    // normalizes capture level
}, ct);
```

With cloud STT providers, set it on `SpeechRecognitionOptions.AudioProcessing` (they capture through
`IAudioSource`):

```csharp
await stt.Start(new SpeechRecognitionOptions
{
    Culture = CultureInfo.GetCultureInfo("en-US"),
    AudioProcessing = AudioProcessingOptions.VoiceChat
});
```

Behavior notes when generating code:
- Effects are **best-effort** and device/driver dependent — never assume an effect is active.
- **Apple** bundles all three into one Voice-Processing I/O unit: any flag enables the whole chain (no independent toggles).
- **Android**: each flag maps to `AcousticEchoCanceler` / `NoiseSuppressor` / `AutomaticGainControl`, gated on `.IsAvailable`; echo cancellation also routes capture through `VoiceCommunication`.
- **Windows**: no per-effect API — any flag selects the `Communications` capture category (driver-provided AEC/NS when present).
- **Browser**: maps to `getUserMedia` constraints (`echoCancellation` / `noiseSuppression` / `autoGainControl`); AEC uses WebRTC AEC3 and cancels page-rendered TTS.
- **Native on-device `ISpeechToTextService`** manages its own mic and ignores `AudioProcessing` — the setting only affects `IAudioSource` capture (cloud providers, raw capture).

### 4. Audio Playback

`IAudioPlayer.PlayAsync` has two overloads: a `Stream`, or a `string` that is **either a remote
`http`/`https` URL or a local file path**. Pass a plain URL/path — each platform resolves it natively;
never construct a platform-specific file URI. Remote sources stream progressively on Android / Windows /
Browser and are buffered on Apple. In the browser a "local path" means an app-relative URL (no device
file system).

```csharp
public class MyViewModel(IAudioPlayer audioPlayer)
{
    Task PlayRemote(CancellationToken ct) => audioPlayer.PlayAsync("https://example.com/clip.mp3", ct);
    Task PlayLocal(CancellationToken ct)  => audioPlayer.PlayAsync(Path.Combine(FileSystem.AppDataDirectory, "chime.mp3"), ct);

    async Task PlayStream(Stream mp3Stream, CancellationToken ct)
    {
        await audioPlayer.PlayAsync(mp3Stream, ct); // e.g. MP3

        if (audioPlayer.IsPlaying)
            await audioPlayer.StopAsync();
    }
}
```

### 5. VU Meter (Audio Level)

Subscribe to `AudioLevelChanged` on `ITextToSpeechService` (native + cloud TTS) or `IAudioPlayer` (generic audio playback). Each emitted value is a normalized RMS level in `0.0`–`1.0`. Always gate UI on `IsPlayerAnalysisSupported` — it is `false` on Windows native TTS and Browser.

```csharp
public partial class TtsViewModel(ITextToSpeechService tts) : ObservableObject
{
    [ObservableProperty] double audioLevel; // bind to ProgressBar.Progress
    public bool IsVuSupported => tts.IsPlayerAnalysisSupported;

    public TtsViewModel(ITextToSpeechService tts) : this(tts)
        => tts.AudioLevelChanged += (_, level) =>
            MainThread.BeginInvokeOnMainThread(() => AudioLevel = level);
}
```

Platform behaviour:

| Surface | iOS / macOS | Android | Windows | Browser |
|---|---|---|---|---|
| Native TTS (`ITextToSpeechService`) | ✅ AVAudioEngine + player-node tap | ✅ `OnAudioAvailable` PCM RMS | ❌ | ❌ |
| Cloud TTS (`CloudTextToSpeech`) | ✅ forwarded from `IAudioPlayer` | ✅ forwarded from `IAudioPlayer` | ❌ | ❌ |
| Generic playback (`IAudioPlayer`) | ✅ `AVAudioPlayer.MeteringEnabled` | ✅ `Visualizer` on session | ❌ | ❌ |

Apple native TTS plays through `AVAudioEngine` + `AVAudioPlayerNode` so a tap on the player node can compute RMS. The engine is created lazily on first speak and kept warm — first utterance adds ~50–150 ms; subsequent utterances are indistinguishable. Reset `AudioLevel` to `0` on speak completion / `StopAsync` so the meter drains.

### 6. Microphone Monitor & Device Selection (`IAudio` / `IAudioMonitor` / `IAudioDevices`)

Inject the `IAudio` facade to reach the whole surface (`Player` / `Source` / `Monitor` / `Devices`),
or inject the focused interfaces directly. `IAudioMonitor` routes the mic to the current output live
(PA / "talk over a Bluetooth speaker"); `IAudioDevices` enumerates routes.

```csharp
using Shiny.Audio;
using Shiny; // AccessState

public partial class MicViewModel(IAudio audio) : ObservableObject
{
    [ObservableProperty] double level;

    async Task Toggle()
    {
        if (audio.Monitor.IsMonitoring)
        {
            await audio.Monitor.Stop();
            return;
        }
        if (await audio.Monitor.RequestAccess() != AccessState.Available)
            return;

        audio.Monitor.InputLevelChanged += (_, l) =>
            MainThread.BeginInvokeOnMainThread(() => Level = l);

        await audio.Monitor.Start(new AudioMonitorOptions
        {
            Gain = 1.0,
            Processing = AudioProcessingOptions.VoiceChat,   // AEC/NS/AGC — fights feedback
            InputDevice = audio.Devices.CurrentInput
        });
    }

    void ListOutputs()
    {
        foreach (var d in audio.Devices.GetOutputs())      // AudioDevice: Id, Name, Io, Type, IsCurrent
            Console.WriteLine($"{d.Name} ({d.Type}){(d.IsCurrent ? " *" : "")}");
    }

    Task ChooseMic(AudioDevice mic) => audio.Monitor.SetInputDevice(mic);   // live device switch
}
```

Behavior notes when generating code:
- `IAudioMonitor` and `IAudioDevices` are implemented on **iOS/Mac Catalyst and Android only**. The `IAudio` facade throws `PlatformNotSupportedException` if `Monitor`/`Devices` are accessed elsewhere — guard by platform.
- **Bluetooth speaker vs. echo cancellation (iOS):** leave `AudioMonitorOptions.Processing` **null** to route to a Bluetooth A2DP speaker (phone mic + BT output). Enabling processing (AEC) engages iOS's voice-processing unit, which forces Bluetooth onto the low-quality HFP profile — an A2DP-only speaker then drops back to the phone. Only enable processing for phone-speaker output where feedback is a problem.
- **AirPlay (HomePod / Apple TV) is NOT supported for a live mic** — iOS only permits AirPlay for playback, not while recording. Use Bluetooth for a wireless live PA.
- **Feedback** (mic hearing the phone speaker) — keep distance, lower `Gain`, or enable AEC (accepting the Bluetooth trade-off). **Always stop the monitor on page-disappear / app-background.**
- **Device selection is Android-first.** `IAudioMonitor.SetInputDevice`/`SetOutputDevice`: Android enumerates and selects both input and output; iOS can select the input but treats **output as observe-only** (no app-level output enumeration/selection — AirPlay/Bluetooth output is owned by the system picker). Use `IAudioDevices.CurrentInput`/`CurrentOutput` (with `.Type`) as a **display** property everywhere.
- `IAudioSource` (pull-stream capture) and `IAudioMonitor` (live passthrough) are different tools — use the monitor for real-time mic→speaker, not a capture-stream-into-player loop.

### 5. Custom Cloud Provider

Implement `ISpeechToTextProvider` and/or `ITextToSpeechProvider`:

```csharp
public class MyCloudSttProvider : ISpeechToTextProvider
{
    // Required: surface non-fatal errors (e.g. a transient network blip between
    // chunked requests in continuous mode) without aborting the IAsyncEnumerable.
    // CloudSpeechToText subscribes to this and forwards to ISpeechToTextService.Error.
    public event EventHandler<SpeechRecognitionError>? Error;

    public async IAsyncEnumerable<SpeechRecognitionResult> RecognizeAsync(
        Stream audioStream,
        SpeechRecognitionOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Send audioStream to your cloud API
        // Yield results as they arrive
        try
        {
            yield return new SpeechRecognitionResult("Hello", IsFinal: true, Confidence: 0.95f);
        }
        catch (HttpRequestException ex)
        {
            // Non-fatal: signal the error and let the session keep running.
            // Throwing instead would terminate the enumerator and end the session.
            Error?.Invoke(this, new SpeechRecognitionError(ex.Message, ex));
        }
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
18. **No JS setup in the browser** — `shiny-audio.js` ships as a static web asset inside the `Shiny.Audio` package (`_content/Shiny.Audio/shiny-audio.js`) and is imported automatically via `JSHost.ImportAsync`; never copy it into `wwwroot` or add a `<script>` tag
19. **CarPlay compatible** — iOS audio session uses `PlayAndRecord` with `AllowBluetooth` / `AllowBluetoothA2dp` / `DefaultToSpeaker`, so when CarPlay is active iOS automatically routes audio through the car's microphone and speakers — no CarPlay-specific code needed
20. **VU meter gating** — always check `IsPlayerAnalysisSupported` before showing meter UI; events do not fire on platforms where metering isn't available (Windows native TTS, Browser)
21. **Marshal `AudioLevelChanged` to the UI thread** — the event fires from the audio render / synthesizer thread; use `MainThread.BeginInvokeOnMainThread` in MAUI or equivalent in Blazor before mutating bound properties
22. **Reset audio level on completion** — set your bound `AudioLevel` back to `0` after `SpeakAsync` returns or `StopAsync` is called so the meter drains visually
23. **Stop the mic monitor when leaving** — always `IAudioMonitor.Stop()` on page-disappear and app-background; a live monitor left open is feedback and battery drain
24. **Guard monitor/devices by platform** — `IAudioMonitor` / `IAudioDevices` exist on iOS/Mac Catalyst + Android only; accessing them via the `IAudio` facade elsewhere throws `PlatformNotSupportedException`
25. **Prefer the `IAudio` facade for discovery** — inject one `IAudio` to reach `Player`/`Source`/`Monitor`/`Devices`; inject the focused interface directly when a class only needs one

## Reference Files

For detailed API documentation, see:
- `reference/api-reference.md` - Full API surface, interfaces, records, and configuration

## Common Packages

```bash
dotnet add package Shiny.Speech                  # Core platform-native speech services
dotnet add package Shiny.Speech.Cloud            # Cloud provider abstractions (included by Azure/ElevenLabs)
dotnet add package Shiny.Speech.Azure            # Azure AI Speech provider
dotnet add package Shiny.Speech.ElevenLabs       # ElevenLabs TTS provider
dotnet add package Shiny.Speech.Typecast         # Typecast TTS provider (TTS only)
```
