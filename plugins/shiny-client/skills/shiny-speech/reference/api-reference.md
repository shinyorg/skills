# API Reference

## Installation

```bash
dotnet add package Shiny.Speech
dotnet add package Shiny.Audio                   # Referenced transitively by Shiny.Speech; add directly for audio-only use
dotnet add package Shiny.Speech.Azure            # Optional: Azure AI Speech
dotnet add package Shiny.Speech.ElevenLabs       # Optional: ElevenLabs STT (Scribe) + TTS
dotnet add package Shiny.Speech.Typecast         # Optional: Typecast TTS (TTS only)
```

## Namespaces

```csharp
using Shiny;        // AccessState (from Shiny.Core), AddSpeechServices/AddAudio* DI extensions, UseShiny
using Shiny.Speech; // ISpeechToTextService, ITextToSpeechService, VoiceInfo, SpeechRecognition*, TextToSpeechOptions
using Shiny.Audio;  // IAudio, IAudioSource, IAudioPlayer, IAudioMonitor, IAudioDevices, AudioDevice, PipeStream
```

The audio interfaces live in the standalone `Shiny.Audio` package/namespace. `AccessState` comes from
`Shiny.Core` (namespace `Shiny`) — it is a parent namespace of `Shiny.Audio`/`Shiny.Speech`, so those
resolve it without an extra `using`. DI extension methods (`AddSpeechServices`, `AddAudioServices`,
`AddAudioSource`, `AddAudioPlayer`, …) and `UseShiny()` are in the `Shiny` namespace.

> **Native speech/audio require `.UseShiny()`** (from `Shiny.Hosting.Maui`) so Shiny.Core's
> `AndroidPlatform` is registered — it performs the Android `RECORD_AUDIO` permission request and
> current-activity tracking that `RequestAccess()` relies on.

## AccessState Enum

Permission/availability states from `Shiny.Core` (namespace `Shiny`). The speech/audio services return
`Unknown`, `NotSupported`, `Denied`, `Restricted`, and `Available`; the full enum also defines
`NotSetup` and `Disabled` for other Shiny modules.

```csharp
public enum AccessState
{
    Unknown,        // State has not been determined
    NotSupported,   // Feature is not supported on this platform
    NotSetup,       // Not configured
    Disabled,       // Feature is switched off at the OS level
    Restricted,     // Access is restricted (e.g., parental controls)
    Denied,         // User denied permission
    Available       // Ready to use
}
```

## ISpeechToTextService Interface

Platform-native or cloud-backed speech recognition service. Registered as singleton. Uses a Start/Stop model with events to allow multiple subscribers.

```csharp
public interface ISpeechToTextService
{
    // Whether STT is supported on this platform
    bool IsSupported { get; }

    // Whether speech recognition is currently active
    bool IsListening { get; }

    // Request microphone and speech recognition permissions
    Task<AccessState> RequestAccess();

    // Start listening — throws InvalidOperationException if already listening
    Task Start(SpeechRecognitionOptions? options = null);

    // Stop listening — no-op if not listening
    Task Stop();

    // Fires for every recognition result (partial and final)
    event EventHandler<SpeechRecognitionResult> ResultReceived;

    // Fires when a keyword from SpeechRecognitionOptions.Keywords is detected in a final result
    event EventHandler<string> KeywordHeard;

    // Fires on recognition errors
    event EventHandler<SpeechRecognitionError> Error;
}
```

### Usage

```csharp
public class MyViewModel(ISpeechToTextService stt)
{
    async Task Listen()
    {
        stt.ResultReceived += (s, result) =>
            Console.WriteLine($"{result.Text} (final: {result.IsFinal})");

        stt.KeywordHeard += (s, keyword) =>
            Console.WriteLine($"Keyword: {keyword}");

        await stt.Start(new SpeechRecognitionOptions
        {
            Keywords = ["Yes", "No"]
        });

        // Later...
        await stt.Stop();
    }
}
```

## SpeechToTextExtensions (Extension Methods)

Convenience extension methods on `ISpeechToTextService` that handle Start/Stop/event wiring automatically.

```csharp
public static class SpeechToTextExtensions
{
    // Listen until silence — starts, waits for first final result, then stops.
    static Task<string?> ListenUntilSilence(
        this ISpeechToTextService service,
        SpeechRecognitionOptions? options = null,
        CancellationToken cancellationToken = default
    );

    // Wake word style — waits for a keyword to be heard,
    // then returns the next final statement after it.
    static Task<string?> StatementAfterKeyword(
        this ISpeechToTextService service,
        string[] keywords,
        SpeechRecognitionOptions? options = null,
        CancellationToken cancellationToken = default
    );

    // Waits until one of the specified keywords is detected.
    // Returns the matched keyword or null on timeout/cancellation.
    static Task<string?> WaitListenForKeywords(
        this ISpeechToTextService service,
        string[] keywords,
        TimeSpan? timeout = null,
        SpeechRecognitionOptions? options = null,
        CancellationToken cancellationToken = default
    );

    // Continuously yields keywords as they are detected.
    static IAsyncEnumerable<string> ListenForKeywords(
        this ISpeechToTextService service,
        string[] keywords,
        SpeechRecognitionOptions? options = null,
        CancellationToken cancellationToken = default
    );
}
```

### Usage

```csharp
// Simple dictation
var text = await stt.ListenUntilSilence();

// Wake word: "Hey Computer, do X" → returns "do X"
var command = await stt.StatementAfterKeyword(["Hey Computer"], cancellationToken: ct);

// Wait for keyword (with timeout)
var answer = await stt.WaitListenForKeywords(["Yes", "No"], timeout: TimeSpan.FromSeconds(30));

// Continuous keyword stream
await foreach (var kw in stt.ListenForKeywords(["Up", "Down", "Left", "Right"], cancellationToken: ct))
    Console.WriteLine($"Direction: {kw}");
```

## ITextToSpeechService Interface

Platform-native or cloud-backed text-to-speech service. Registered as singleton.

```csharp
public interface ITextToSpeechService
{
    // Whether TTS is supported on this platform
    bool IsSupported { get; }

    // Whether speech is currently playing
    bool IsSpeaking { get; }

    // True when this service can emit AudioLevelChanged during playback
    bool IsPlayerAnalysisSupported { get; }

    // Fires periodically while speaking with the current RMS level (0.0 - 1.0).
    // Suitable for driving a VU meter UI.
    event EventHandler<double>? AudioLevelChanged;

    // Get available voices, optionally filtered by culture
    Task<IReadOnlyList<VoiceInfo>> GetVoicesAsync(
        CultureInfo? culture = null,
        CancellationToken cancellationToken = default
    );

    // Speak text with optional configuration
    Task SpeakAsync(
        string text,
        TextToSpeechOptions? options = null,
        CancellationToken cancellationToken = default
    );

    // Stop current speech
    Task StopAsync();
}
```

`IsPlayerAnalysisSupported` matrix:

| Platform | Native TTS | Cloud TTS (Azure / OpenAI / ElevenLabs / custom) |
|---|---|---|
| iOS / macOS / Mac Catalyst | ✅ (AVAudioEngine + player-node tap) | ✅ (AVAudioPlayer metering) |
| Android | ✅ (`OnAudioAvailable` PCM RMS) | ✅ (Visualizer on audio session) |
| Windows | ❌ | ❌ |
| Browser | ❌ | ❌ |

### Usage

```csharp
public class MyViewModel(ITextToSpeechService tts)
{
    await tts.SpeakAsync("Hello!");
    await tts.SpeakAsync("Slow speech", new TextToSpeechOptions { SpeechRate = 0.5f });

    var voices = await tts.GetVoicesAsync(CultureInfo.GetCultureInfo("en-US"));
    await tts.SpeakAsync("With voice", new TextToSpeechOptions { Voice = voices.First() });
}
```

## IAudioSource Interface

Platform-native microphone audio capture. Registered as transient. Implements `IAsyncDisposable`.

```csharp
public interface IAudioSource : IAsyncDisposable
{
    // Optional runtime mic-permission request (Available on platforms without a runtime prompt)
    Task<AccessState> RequestAccess() => Task.FromResult(AccessState.Available);

    // Start capturing raw PCM audio (16kHz, 16-bit, mono). The optional processing argument
    // requests best-effort voice-processing effects (AEC / noise suppression / AGC).
    Task<Stream> StartCaptureAsync(
        AudioProcessingOptions? processing = null,
        CancellationToken cancellationToken = default
    );

    // Stop audio capture
    Task StopCaptureAsync();
}
```

## AudioProcessingOptions Record

Best-effort microphone voice-processing effects (in the `Shiny.Audio` namespace). Any effect a
platform or device can't honor is silently skipped.

```csharp
public record AudioProcessingOptions
{
    // Acoustic echo cancellation — subtracts the device's own speaker/TTS output from the mic
    // signal so it isn't re-captured while the mic is open (barge-in).
    bool EchoCancellation { get; init; }

    // Noise suppression — attenuates steady background noise.
    bool NoiseSuppression { get; init; }

    // Automatic gain control — normalizes capture level.
    bool AutomaticGainControl { get; init; }

    // True when at least one effect is requested.
    bool AnyEnabled { get; }

    static AudioProcessingOptions VoiceChat { get; }  // all three enabled
    static AudioProcessingOptions None { get; }       // raw capture
}
```

Platform mapping: **Apple** → single Voice-Processing I/O unit (any flag enables all three);
**Android** → `AcousticEchoCanceler` / `NoiseSuppressor` / `AutomaticGainControl` (+ `VoiceCommunication`
source when AEC requested); **Windows** → `Communications` capture category (no per-effect control);
**Browser** → `getUserMedia` constraints (WebRTC AEC3). Native on-device `ISpeechToTextService`
manages its own mic and ignores this.

## IAudioPlayer Interface

Platform-native audio playback. Registered as singleton. Implements `IAsyncDisposable`.

```csharp
public interface IAudioPlayer : IAsyncDisposable
{
    // Play audio (e.g. MP3) from a stream
    Task PlayAsync(Stream audioStream, CancellationToken cancellationToken = default);

    // Play from a remote http/https URL or a local file path — the platform resolves the
    // source natively (no platform-specific file URI needed). Remote sources stream on
    // Android/Windows/Browser and are buffered on Apple.
    Task PlayAsync(string source, CancellationToken cancellationToken = default);

    // Stop playback
    Task StopAsync();

    // Whether audio is currently playing
    bool IsPlaying { get; }

    // True when this player can emit AudioLevelChanged during playback
    bool IsPlayerAnalysisSupported { get; }

    // Fires periodically during playback with the current RMS level (0.0 - 1.0).
    event EventHandler<double>? AudioLevelChanged;

    // True when SETTING Volume is supported (Android, Windows, macOS*, Browser);
    // false on iOS / Mac Catalyst. Reading Volume works everywhere. Always guard a set with this.
    bool IsVolumeControlSupported { get; }

    // Device media volume, 0.0-1.0 (app media-element volume in the browser).
    // Getter works on all platforms. Setter throws NotSupportedException on iOS / Mac Catalyst.
    float Volume { get; set; }

    // Raised when the volume changes: hardware buttons, the OS volume UI, or a successful set.
    event EventHandler<float>? VolumeChanged;
}
```

### Volume

`Volume` is the **system media volume** (0.0-1.0) on device platforms — the level the hardware buttons
control, independent of any per-request TTS `Volume`. Reading works everywhere an `IAudioPlayer`
exists; **setting is platform-limited — always guard with `IsVolumeControlSupported`**:

```csharp
var level = player.Volume;                 // read anywhere

if (player.IsVolumeControlSupported)       // iOS / Mac Catalyst setter throws NotSupportedException
    player.Volume = 0.5f;

player.VolumeChanged += (_, v) => { /* new volume 0.0-1.0 */ };
```

| Platform | Read | Set | `VolumeChanged` | Backing API |
|---|---|---|---|---|
| Android | ✅ | ✅ | system settings observer | `AudioManager` `STREAM_MUSIC` (integer-stepped → set value is quantized) |
| Windows | ✅ | ✅ | endpoint callback | WASAPI `IAudioEndpointVolume` on the default render endpoint |
| macOS | ✅ | ✅* | CoreAudio property listener | HAL virtual main volume of the default output device |
| iOS / Mac Catalyst | ✅ | ❌ | KVO on `outputVolume` | `AVAudioSession.OutputVolume` (read-only; no supported set API) |
| Browser (WASM) | ✅ | ✅ | echoed on set | `HTMLAudioElement.volume` (app-local, **not** the OS volume; persists across plays) |

\* macOS reports `IsVolumeControlSupported = false` when the current default output device has no
settable virtual main volume (e.g. some HDMI / aggregate devices).

Marshal `VolumeChanged` to the UI thread before mutating bound properties — it fires from the audio /
observer thread.

## IAudio Interface (facade)

One-stop discovery entry point over the focused audio services. Registered as singleton by
`AddAudioServices()`. Each member resolves the underlying service on demand, preserving lifetimes.
`Monitor`/`Devices` throw `PlatformNotSupportedException` on platforms without an implementation
(Windows/Browser).

```csharp
public interface IAudio
{
    IAudioPlayer  Player  { get; }   // singleton
    IAudioSource  Source  { get; }   // fresh transient per access
    IAudioMonitor Monitor { get; }   // iOS/Mac Catalyst + Android
    IAudioDevices Devices { get; }   // iOS/Mac Catalyst + Android
}
```

## IAudioMonitor Interface

Live mic-to-output passthrough (PA). Registered as singleton (iOS/Mac Catalyst + Android). Implements
`IAsyncDisposable`.

```csharp
public interface IAudioMonitor : IAsyncDisposable
{
    Task<AccessState> RequestAccess();
    Task Start(AudioMonitorOptions? options = null, CancellationToken cancellationToken = default);
    Task Stop();
    bool IsMonitoring { get; }
    double Gain { get; set; }                         // 0.0 - 1.0, adjustable live
    Task SetInputDevice(AudioDevice? device);         // null = OS default
    Task SetOutputDevice(AudioDevice? device);        // best-effort (iOS: speaker override only)
    event EventHandler<double>? InputLevelChanged;    // 0.0 - 1.0 VU signal
}

public record AudioMonitorOptions
{
    public AudioProcessingOptions? Processing { get; init; }  // AEC/NS/AGC (feedback defense)
    public double Gain { get; init; } = 1.0;
    public AudioDevice? InputDevice { get; init; }
    public AudioDevice? OutputDevice { get; init; }
}
```

## IAudioDevices Interface

Audio route enumeration and current-route reporting. Registered as singleton (iOS/Mac Catalyst +
Android). Selection is applied via `IAudioMonitor.SetInputDevice`/`SetOutputDevice`.

```csharp
public interface IAudioDevices
{
    IReadOnlyList<AudioDevice> GetInputs();
    IReadOnlyList<AudioDevice> GetOutputs();
    AudioDevice? CurrentInput { get; }
    AudioDevice? CurrentOutput { get; }
    Task ShowOutputPicker();          // OS route picker where available; no-op otherwise
    event EventHandler? Changed;      // fires on route add/remove/switch
}

public enum AudioDeviceIo { Input, Output }

public enum AudioDeviceType
{
    Unknown, BuiltInMic, BuiltInSpeaker, BuiltInReceiver, WiredHeadset, WiredHeadphones,
    Bluetooth, BluetoothA2dp, Usb, CarAudio, Hdmi, AirPlay
}

public record AudioDevice(string Id, string Name, AudioDeviceIo Io, AudioDeviceType Type, bool IsCurrent);
```

## SpeechRecognitionResult Record

```csharp
public record SpeechRecognitionResult(
    string Text,           // Recognized speech text
    bool IsFinal,          // Whether this is the final result for a segment
    float? Confidence      // Optional confidence score (0.0 to 1.0)
);
```

## SpeechRecognitionError Record

```csharp
public record SpeechRecognitionError(
    string Message,            // Error description
    Exception? Exception       // Optional underlying exception
);
```

## SpeechRecognitionOptions Record

```csharp
public record SpeechRecognitionOptions
{
    // Language/culture for recognition (null = device default)
    CultureInfo? Culture { get; init; }

    // Timeout for silence detection (default: 2 seconds)
    TimeSpan SilenceTimeout { get; init; } = TimeSpan.FromSeconds(2);

    // Request on-device recognition when available
    bool PreferOnDevice { get; init; }

    // Keywords for keyword detection (null = no keyword detection)
    // When set, KeywordHeard event fires on case-insensitive whole-word matches
    string[]? Keywords { get; init; }

    // Voice-processing effects applied when a provider captures via IAudioSource (cloud
    // providers). Enable EchoCancellation to stop TTS bleeding into the mic. null = raw.
    // Native on-device recognizers ignore this.
    AudioProcessingOptions? AudioProcessing { get; init; }
}
```

## TextToSpeechOptions Record

```csharp
public record TextToSpeechOptions
{
    // Language/culture for synthesis (null = device default)
    CultureInfo? Culture { get; init; }

    // Specific voice to use (null = platform default)
    VoiceInfo? Voice { get; init; }

    // Speech rate multiplier (default: 1.0)
    float SpeechRate { get; init; } = 1.0f;

    // Pitch adjustment (default: 1.0)
    float Pitch { get; init; } = 1.0f;

    // Volume level (default: 1.0)
    float Volume { get; init; } = 1.0f;
}
```

## VoiceInfo Record

```csharp
public record VoiceInfo(
    string Id,             // Platform-specific voice identifier
    string Name,           // Human-readable voice name
    CultureInfo Culture    // Associated language/culture
);
```

## Cloud Provider Interfaces

### ISpeechToTextProvider

Pluggable interface for cloud STT backends. Implement to add custom providers. The `CloudSpeechToText` wrapper consumes this internally and raises events on `ISpeechToTextService`.

```csharp
public interface ISpeechToTextProvider
{
    IAsyncEnumerable<SpeechRecognitionResult> RecognizeAsync(
        Stream audioStream,
        SpeechRecognitionOptions? options = null,
        CancellationToken cancellationToken = default
    );

    // Non-fatal errors during continuous recognition (e.g. transient network failure
    // between chunked requests). CloudSpeechToText subscribes and forwards to its own
    // ISpeechToTextService.Error event — implementers raise this instead of throwing
    // out of the IAsyncEnumerable when they want the session to keep running.
    event EventHandler<SpeechRecognitionError>? Error;
}
```

### ITextToSpeechProvider

Pluggable interface for cloud TTS backends. Implement to add custom providers.

```csharp
public interface ITextToSpeechProvider
{
    Task<IReadOnlyList<VoiceInfo>> GetVoicesAsync(
        CultureInfo? culture = null,
        CancellationToken cancellationToken = default
    );

    Task<Stream> SynthesizeAsync(
        string text,
        TextToSpeechOptions? options = null,
        CancellationToken cancellationToken = default
    );
}
```

## Browser/WebAssembly Implementations

When running in a Blazor WebAssembly app, `AddSpeechServices()` auto-detects the browser via `OperatingSystem.IsBrowser()` and registers these implementations:

- **`BrowserSpeechToTextService`** — Uses the Web Speech API `SpeechRecognition` interface via `[JSImport]`/`[JSExport]` interop. Raises `ResultReceived`, `KeywordHeard`, and `Error` events.
- **`BrowserTextToSpeechService`** — Uses the Web Speech API `SpeechSynthesis` interface via `[JSImport]`/`[JSExport]` interop
- **`BrowserAudioPlayer`** — Converts streams to base64 data URLs and plays via HTML5 `Audio` element
- **`BrowserAudioSource`** — Captures raw PCM (16 kHz, 16-bit, mono) via the Web Audio API (`getUserMedia` + `ScriptProcessorNode`); honors `AudioProcessingOptions` through `getUserMedia` constraints

All browser implementations are annotated with `[SupportedOSPlatform("browser")]`.

The JS interop module (`shiny-audio.js`) ships as a static web asset inside the **`Shiny.Audio`** package (`_content/Shiny.Audio/shiny-audio.js`) and is imported on demand via `JSHost.ImportAsync`. **No `<script>` tag and no manual `wwwroot` copy are needed** — referencing the NuGet package is sufficient.

## DI Extension Methods

### Core Services (Shiny.Speech)

```csharp
public static class SpeechServiceCollectionExtensions
{
    // Register all core services (STT, TTS, AudioSource, AudioPlayer)
    // On Browser/WASM: auto-detected via OperatingSystem.IsBrowser()
    IServiceCollection AddSpeechServices(this IServiceCollection services);

    // Register ISpeechToTextService with platform-specific implementation
    // On Browser: registers BrowserSpeechToTextService
    IServiceCollection AddSpeechToText(this IServiceCollection services);

    // Register ITextToSpeechService with platform-specific implementation
    // On Browser: registers BrowserTextToSpeechService
    IServiceCollection AddTextToSpeech(this IServiceCollection services);

    // Register IAudioSource (transient)
    // On Browser: throws PlatformNotSupportedException
    IServiceCollection AddAudioSource(this IServiceCollection services);

    // Register IAudioPlayer (singleton)
    // On Browser: registers BrowserAudioPlayer
    IServiceCollection AddAudioPlayer(this IServiceCollection services);
}

// From Shiny.Audio (Shiny namespace). AddSpeechServices() calls AddAudioSource()/AddAudioPlayer()
// but NOT the monitor/devices — use AddAudioServices() to get the full set + the IAudio facade.
public static class AudioServiceCollectionExtensions
{
    // Registers IAudioSource + IAudioPlayer + IAudioMonitor + IAudioDevices + IAudio
    IServiceCollection AddAudioServices(this IServiceCollection services);

    IServiceCollection AddAudioMonitor(this IServiceCollection services);  // IAudioMonitor (iOS/MacCat + Android)
    IServiceCollection AddAudioDevices(this IServiceCollection services);  // IAudioDevices (iOS/MacCat + Android)
}
```

### Cloud Services (Shiny.Speech.Cloud)

```csharp
public static class SpeechCloudServiceCollectionExtensions
{
    // Register a cloud STT provider (replaces platform-native ISpeechToTextService)
    IServiceCollection AddCloudSpeechToText<TProvider>(this IServiceCollection services)
        where TProvider : class, ISpeechToTextProvider;

    IServiceCollection AddCloudSpeechToText(this IServiceCollection services, ISpeechToTextProvider provider);

    // Register a cloud TTS provider (replaces platform-native ITextToSpeechService)
    IServiceCollection AddCloudTextToSpeech<TProvider>(this IServiceCollection services)
        where TProvider : class, ITextToSpeechProvider;

    IServiceCollection AddCloudTextToSpeech(this IServiceCollection services, ITextToSpeechProvider provider);
}
```

### Azure AI Speech (Shiny.Speech.Azure)

```csharp
public static class AzureSpeechExtensions
{
    // Register Azure Speech with key and region
    IServiceCollection AddAzureSpeech(
        this IServiceCollection services,
        string subscriptionKey,
        string region,
        bool speechToText = true,
        bool textToSpeech = true
    );

    // Register Azure Speech with config object
    IServiceCollection AddAzureSpeech(
        this IServiceCollection services,
        AzureSpeechConfig config,
        bool speechToText = true,
        bool textToSpeech = true
    );
}
```

### AzureSpeechConfig

```csharp
public record AzureSpeechConfig
{
    required string SubscriptionKey { get; init; }
    required string Region { get; init; }
}
```

### ElevenLabs (Shiny.Speech.ElevenLabs)

```csharp
public static class ElevenLabsServiceCollectionExtensions
{
    // Combined: register STT (Scribe) + TTS — toggle either via flags
    IServiceCollection AddElevenLabsSpeech(
        this IServiceCollection services,
        string apiKey,
        bool speechToText = true,
        bool textToSpeech = true);
    IServiceCollection AddElevenLabsSpeech(
        this IServiceCollection services,
        ElevenLabsConfig config,
        bool speechToText = true,
        bool textToSpeech = true);

    // STT-only convenience
    IServiceCollection AddElevenLabsSpeechToText(this IServiceCollection services, string apiKey);
    IServiceCollection AddElevenLabsSpeechToText(this IServiceCollection services, ElevenLabsConfig config);

    // TTS-only convenience
    IServiceCollection AddElevenLabsTextToSpeech(this IServiceCollection services, string apiKey);
    IServiceCollection AddElevenLabsTextToSpeech(this IServiceCollection services, ElevenLabsConfig config);
}
```

### ElevenLabsConfig

```csharp
public record ElevenLabsConfig
{
    required string ApiKey { get; init; }
    string DefaultVoiceId { get; init; } = "21m00Tcm4TlvDq8ikWAM"; // Rachel
    string TextToSpeechModel { get; init; } = "eleven_multilingual_v2";
    string SpeechToTextModel { get; init; } = "scribe_v1";
}
```

> **ElevenLabs Scribe is request/response, not streaming.** `CloudSpeechToText` buffers the captured PCM until `Stop()` is called, wraps it in a WAV container, and posts a single multipart request to `/v1/speech-to-text`. One final `SpeechRecognitionResult` is yielded. For continuous partial results across long sessions, use Azure.

## Troubleshooting

### Speech recognition not working
- Call `RequestAccess()` first and check for `AccessState.Available`
- Ensure microphone permissions are declared in platform manifests
- Check `IsSupported` — some platforms/emulators don't support STT
- Make sure you're not calling `Start()` when already listening (it throws)

### No audio captured
- Ensure `AddAudioSource()` is registered (cloud providers auto-register this)
- Audio format is raw PCM (16kHz, 16-bit, mono) — not WAV or MP3
- Call `StopCaptureAsync()` when done to release the microphone

### Cloud provider not working
- Cloud providers automatically register `IAudioSource` and `IAudioPlayer` as needed
- Cloud providers replace the platform-native `ISpeechToTextService`/`ITextToSpeechService` registrations
- Verify API keys and region settings

### Browser/WASM speech not working
- `shiny-audio.js` ships in the `Shiny.Audio` package (`_content/Shiny.Audio/shiny-audio.js`) and loads automatically — do **not** add a `<script>` tag or copy it into `wwwroot`. If a stale `wwwroot/shiny-speech.js` exists from an older version (when consumers copied it manually), delete it
- Check browser support: `SpeechRecognition` is not supported in all browsers (Firefox lacks support as of 2026)
- `IAudioSource` **is** supported in the browser (raw PCM capture via the Web Audio API); `AudioProcessingOptions` maps to `getUserMedia` constraints (echo cancellation / noise suppression / auto gain)
- The browser will prompt for microphone permission automatically — no manifest entries needed

### TTS voice not found
- Use `GetVoicesAsync()` to list available voices for the target culture
- Voice availability varies by platform and cloud provider
- `VoiceInfo.Id` is platform-specific — don't hardcode across platforms

### ElevenLabs voices not loading
- API key must be valid and have available credits
- Voice listing requires network access
- Default voice ID (`21m00Tcm4TlvDq8ikWAM`) is the "Rachel" voice
