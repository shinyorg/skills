# Voice (`Shiny.AiConversation.Voice`)

`IVoiceConversation` talks to an `IAiSession` by voice: wake word, push-to-talk, follow-up answers,
interruption and sentence-by-sentence text-to-speech. The session owns the conversation; the voice
conversation owns the audio and only uses the session's public API (`Send`, `Interrupt`, events).

**NuGet**: `Shiny.AiConversation.Voice` · **Namespace**: `Shiny.AiConversation.Voice` (registration in `Shiny`)

## Registration

The voice package registers **no speech engines**. Register the Shiny.Speech providers you want yourself
(native, Azure, OpenAI, ElevenLabs, Typecast, Microsoft.Extensions.AI, Linux Whisper...). On Android prefer a
cloud speech-to-text provider; the native recognizer isn't good enough for conversation.

```csharp
using Shiny;

builder.Services.AddSpeechServices();                 // or your cloud STT/TTS provider registrations
builder.Services.AddAudioPlayer();                    // optional: sounds + gapless cloud-voice playback
builder.Services.AddShinyAiConversation(opts => opts.AddGithubCopilotChatClient());
builder.Services.AddShinyAiConversationVoice(v => v
    .AddVoiceSelectionTools()                         // optional: AI can list/sample/change its voice
    // .SetSoundProvider<MySounds>()                  // optional: replace the built-in cues
);
```

Registered (via `TryAdd`): `TimeProvider.System`, `DefaultSoundProvider` as `ISoundProvider`,
`VoiceConversation` as `IVoiceConversation` (singleton). Optional constructor dependencies: `IAudioPlayer`
(Shiny.Audio) and `IWakeWordDetector`.

## Lifecycle

```csharp
await voice.Attach(session, new VoiceOptions { WakeWord = null });   // one session at a time
await voice.PushToTalk();      // listen for one utterance now (with or without a wake word)
await voice.StopSpeaking();    // stop the reply, record what was heard, don't listen
await voice.Pause();           // release mic + speaker (e.g. camera open)
await voice.Resume();
await voice.Detach();          // stop everything and release the session (the caller still disposes it)
var access = await voice.RequestAccess();   // real AccessState from the STT engine
```

- `Attach` throws `InvalidOperationException` if a session is already attached (the mic is one resource).
- `PushToTalk` while the AI is thinking or speaking stops the reply (recording what was heard) and listens.
- `TextToSpeech` (property) is the live voice; it starts from `VoiceOptions.TextToSpeech` and the voice-selection tools change it.

## VoiceOptions

| Property | Default | Notes |
|---|---|---|
| `WakeWord` | `null` | Null = push-to-talk only. An always-open mic has battery, privacy and cloud-cost implications |
| `Interruption` | `QuietWords` | `Off` / `QuietWords` / `Full` (below) |
| `QuietWords` | cancel, quiet, shut up, stop, nevermind, never mind, hush | Must be the **whole** utterance ("stop" yes, "cancel this appointment" no). Ignored in `Off` |
| `InterruptionMinConfidence` | `0.5` | `Full` only, applied only once audio is playing; 0 = "not reported" and passes |
| `FollowUpTimeout` | 20 s | How long to wait for an answer after the AI asks; null waits forever. User-initiated listens always wait |
| `EchoSettleWindow` | 750 ms | `Off` only: recognition stays muted this long after speaking, then stale results are dropped |
| `Acknowledgement` | `Full` | `None` / `AudioBlip` / `LessWordy` / `Full` (below) |
| `ToolAcknowledgement` | `"One moment."` | Spoken when a non-silent tool runs before any reply text; null disables |
| `LessWordyInstruction` | "Be concise..." | Added to each voice turn via `AiInput.WithInstructions` in `LessWordy` |
| `SpeechToText` | `null` | `SpeechRecognitionOptions`; echo cancellation is forced on for `QuietWords` / `Full` |
| `TextToSpeech` | `null` | Initial `TextToSpeechOptions` (voice, rate, pitch...) |

## States (`VoiceState`)

`Off` (mic closed, waiting for push-to-talk) · `WaitingForWakeWord` · `Listening` · `FollowUp` (AI asked;
answering needs no wake word) · `Thinking` (waiting for the model) · `Speaking` · `Paused`.

Flow: wake word / push-to-talk → `Listening` → final utterance → `session.Send(AiInput.Voice(text))` →
`Thinking` → `Speaking` → if the turn ended `AwaitingUser` → `FollowUp` (times out) → back to
`WaitingForWakeWord` (or `Off`).

## Interruption modes

| Mode | While the AI thinks/speaks |
|---|---|
| `Off` | Recognition muted; settle window + drain afterwards. Nothing interrupts; quiet words are ignored |
| `QuietWords` (default) | Only an utterance that is entirely a quiet word stops the reply. Acts on the first matching partial |
| `Full` | Any final utterance stops the reply and becomes the next message (quiet words still just stop). Best with headphones |

Every interruption calls `turn.Interrupt(spokenText)` where `spokenText` is the sentences that **finished**
playing (a half-played sentence counts as not heard). The session then tells the model the reply was cut off.
`QuietWords` / `Full` turn on echo cancellation; on iOS voice processing can make playback quieter or change
the route — apps that care use `Off`.

## Acknowledgement modes

| Mode | Sounds | Text-to-speech |
|---|---|---|
| `None` | no | no (text reply only) |
| `AudioBlip` | yes (incl. `Respond` on first text) | no |
| `LessWordy` | yes | yes, model asked to be brief |
| `Full` | yes | yes |

Sounds (`AiAction`): `Ok` (listening), `Think` (sent), `Respond` (AudioBlip), `Cancel` (quiet word), `Error`.
`DefaultSoundProvider` plays embedded WAVs through `IAudioPlayer` (no-op without one). Sound failures are
logged, never fatal.

## Speech output

- Replies are spoken sentence by sentence as they stream (decimals, list numbers, abbreviations and
  initials don't split; fenced code is dropped; markdown is stripped).
- With `ITextToSpeechService.CanSynthesizeToStream` **and** an `IAudioPlayer`, the next sentence is
  synthesized while the current one plays (gapless cloud voices). Otherwise `SpeakAsync` per sentence.
- Only reply text is spoken; tool calls/results (`ModelOnly`) never are. Silent tools (memory, search,
  `ask_user`) never trigger `ToolAcknowledgement`.

## Events (`GetEvents`, hot broadcast; first item is `VoiceStateChanged`)

`VoiceStateChanged(State)` · `InterimTranscript(Text)` (coalescable) · `FinalTranscript(Text)` ·
`SpeakingSentence(Text)` · `SpeechInterrupted(Reason, HeardText, SpokenText)` (`InterruptionReason`: QuietWord / Answer / NewSpeech / PushToTalk / Stop) · `ResponseLatency(Latency)`
(end of utterance → first audio) · `AudioLevel(Input, Output)` (coalescable) · `VoiceError(Error)`.

## Wake word cost: `IWakeWordDetector`

Without a detector the STT engine matches the wake word, so a cloud recognizer streams (and bills) all room
audio. Register an on-device spotter and the recognizer stays closed until the wake word:

```csharp
public interface IWakeWordDetector
{
    IAsyncEnumerable<string> Listen(IReadOnlyList<string> wakeWords, CancellationToken ct);
}
builder.Services.AddSingleton<IWakeWordDetector, MyPorcupineDetector>();
```

None ships in the box; without one, prefer push-to-talk on Android with cloud STT.

## Voice selection tools

`v.AddVoiceSelectionTools()` adds, to every session: `get_available_voices(culture?)` (silent),
`play_voice_sample(voiceId, sampleText?)`, `change_voice(voiceId)` (silent; sets `IVoiceConversation.TextToSpeech`).

## Example view model

```csharp
public class VoiceViewModel(IAiSessionManager manager, IVoiceConversation voice) : IAsyncDisposable
{
    IAiSession? session;
    CancellationTokenSource? cts;

    public async Task Start()
    {
        if (await voice.RequestAccess() != AccessState.Available)
            return;

        // share this one instance with the chat page too
        this.session = await manager.GetLatest() ?? await manager.Start();
        await voice.Attach(this.session, new VoiceOptions
        {
            WakeWord = "hey shiny",                       // or null for push-to-talk only
            Interruption = InterruptionMode.QuietWords,
            FollowUpTimeout = TimeSpan.FromSeconds(15)
        });

        this.cts = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            await foreach (var e in voice.GetEvents(this.cts.Token))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    switch (e)
                    {
                        case VoiceStateChanged s: this.State = s.State; break;
                        case InterimTranscript t: this.Heard = t.Text; break;
                        case SpeakingSentence s: this.Caption = s.Text; break;
                        case VoiceError err: this.Error = err.Error.Message; break;
                    }
                });
            }
        });
    }

    public Task Talk() => voice.PushToTalk();
    public Task Stop() => voice.StopSpeaking();

    public async ValueTask DisposeAsync()
    {
        this.cts?.Cancel();
        await voice.Detach();
        if (this.session != null)
            await this.session.DisposeAsync();
    }

    public VoiceState State { get; set; }
    public string? Heard { get; set; }
    public string? Caption { get; set; }
    public string? Error { get; set; }
}
```

## Answering while the AI speaks

`VoiceOptions.AnswerInterruptions` (default `true`, needs `QuietWords` or `Full` interruption): while the AI is
speaking a question that has choices, a **final** result that is **entirely** one of them stops the reply and sends
`AiInput.Answer(question, choiceIds)` (voice-sourced, so the reply stays spoken-style). Accepted: the choice label
("large", "the large", "I'll take medium please"), ordinals ("the second one", "option three", "the last one"),
yes/no synonyms ("yeah", "nope") for questions with Yes/No choices, and several labels ("cheese and olives") when
`AllowMultiple`. The question is known as soon as the `ask_user` call streams in. The AI reading its own options
("small, medium or large?") never matches; questions without choices can't be answered this way (use `Full`).
The same matcher turns a follow-up answer into a structured choice. `ask_user` asks the model for explicit
Yes/No choices on yes/no questions. `SpeechInterrupted.Reason == InterruptionReason.Answer` reports it.
