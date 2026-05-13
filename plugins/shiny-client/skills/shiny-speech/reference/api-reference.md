# API Reference

## Installation

```bash
dotnet add package Shiny.Speech
dotnet add package Shiny.Speech.Azure            # Optional: Azure AI Speech
dotnet add package Shiny.Speech.ElevenLabs       # Optional: ElevenLabs TTS
```

## Namespace

```csharp
using Shiny.Speech;
```

## AccessState Enum

Permission/availability states for speech services.

```csharp
public enum AccessState
{
    Unknown,        // State has not been determined
    NotSupported,   // Feature is not supported on this platform
    Denied,         // User denied permission
    Restricted,     // Access is restricted (e.g., parental controls)
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
    // Start capturing raw PCM audio (16kHz, 16-bit, mono)
    Task<Stream> StartCaptureAsync(CancellationToken cancellationToken = default);

    // Stop audio capture
    Task StopCaptureAsync();
}
```

## IAudioPlayer Interface

Platform-native audio playback. Registered as singleton. Implements `IAsyncDisposable`.

```csharp
public interface IAudioPlayer : IAsyncDisposable
{
    // Play MP3 format audio from a stream
    Task PlayAsync(Stream audioStream, CancellationToken cancellationToken = default);

    // Stop playback
    Task StopAsync();

    // Whether audio is currently playing
    bool IsPlaying { get; }
}
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
- **`BrowserAudioSource`** — Throws `PlatformNotSupportedException` (raw PCM capture is not available in the browser; the Web Speech API handles audio internally)

All browser implementations are annotated with `[SupportedOSPlatform("browser")]`.

Blazor WASM apps must include the JS interop module in `index.html`:
```html
<script src="shiny-speech.js"></script>
```

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

### ElevenLabs TTS (Shiny.Speech.ElevenLabs)

```csharp
public static class ElevenLabsServiceCollectionExtensions
{
    // Register ElevenLabs TTS with API key
    IServiceCollection AddElevenLabsTextToSpeech(this IServiceCollection services, string apiKey);

    // Register ElevenLabs TTS with config object
    IServiceCollection AddElevenLabsTextToSpeech(this IServiceCollection services, ElevenLabsConfig config);
}
```

### ElevenLabsConfig

```csharp
public record ElevenLabsConfig
{
    required string ApiKey { get; init; }
    string DefaultVoiceId { get; init; } = "21m00Tcm4TlvDq8ikWAM"; // Rachel
    string ModelId { get; init; } = "eleven_multilingual_v2";
}
```

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
- Ensure `shiny-speech.js` is included in `index.html` via `<script src="shiny-speech.js"></script>`
- Check browser support: `SpeechRecognition` is not supported in all browsers (Firefox lacks support as of 2026)
- `IAudioSource` is not supported in the browser — use `ISpeechToTextService` directly (the Web Speech API handles audio internally)
- The browser will prompt for microphone permission automatically — no manifest entries needed

### TTS voice not found
- Use `GetVoicesAsync()` to list available voices for the target culture
- Voice availability varies by platform and cloud provider
- `VoiceInfo.Id` is platform-specific — don't hardcode across platforms

### ElevenLabs voices not loading
- API key must be valid and have available credits
- Voice listing requires network access
- Default voice ID (`21m00Tcm4TlvDq8ikWAM`) is the "Rachel" voice
