# Built-in Tools & Interceptors

| Feature | Registration | Default |
|---|---|---|
| `ask_user` (questions) | always on | on (see `questions.md`) |
| `memory` | `ConfigureMemory(...)` | on (see `memory.md`) |
| Current local time prompt | always on (`DefaultSessionInterceptor`) | on |
| Conversation search | `opts.AddConversationSearch(params string[] profiles)` | off |
| Session titles | `opts.AddSessionTitles()` | off |
| Long-term memory | `opts.AddLongTermMemory(int sessionCount = 3, params string[] profiles)` | off |
| Voice selection (`Shiny.AiConversation.Voice`) | `services.AddShinyAiConversationVoice(v => v.AddVoiceSelectionTools())` | off |
| Your tools | `opts.AddTool(tool, flags, params string[] profiles)` | - |

`profiles` limits a feature to sessions whose `AiSessionSettings.Profile` is in the list; none means every session.

## Conversation search

```csharp
opts.AddConversationSearch();                 // or AddConversationSearch("assistant")
```

Adds two silent tools:
- `search_conversations(query, scope = all|current|others, from?, to?, limit = 10)` - ranked snippets from
  other sessions and the **compacted** part of the current one (messages still in the prompt are excluded).
- `read_conversation(sessionId, aroundSequence, before = 5, after = 5)` - the messages around a hit.

Results are wrapped in `<conversation_excerpt>` blocks marked as data and capped at ~2K tokens. Sessions
with `ExcludeFromSearch = true` never appear. Search quality depends on the store: the SQLite store uses
full-text search; the in-memory store does word matching.

## Session titles

```csharp
opts.AddSessionTitles();
```

After the first completed exchange of an untitled session, asks the model for a 2-6 word title in the
background (the next turn never waits) and calls `session.SetTitle` - observe `TitleChanged` or read
`session.Title`. Set your own with `await session.SetTitle("...")`.

## Long-term memory

```csharp
opts.AddLongTermMemory(sessionCount: 3);
```

On load, adds a `<past_conversations>` system prompt with the compaction summaries of the most recently
active **other** sessions (skips `ExcludeFromSearch` sessions and sessions with no summary yet).

## Voice selection tools

From `Shiny.AiConversation.Voice`: `v.AddVoiceSelectionTools()` registers an `ISessionInterceptor` that adds
to every session:
- `get_available_voices(culture?)` (silent) - lists `ITextToSpeechService` voices
- `play_voice_sample(voiceId, sampleText?)` - speaks a sample with that voice
- `change_voice(voiceId)` (silent) - sets `IVoiceConversation.TextToSpeech` so later replies use it

They act on the `IVoiceConversation` directly (no service-locator lookup). See `voice.md`.

## Your own tools

```csharp
opts.AddTool(AIFunctionFactory.Create(GetWeather, "get_weather"));
opts.AddTool(AIFunctionFactory.Create(SaveRecipe, "save_recipe"), AiToolFlags.Silent, "cooking");
```

`AiToolFlags.Silent`: never triggers a spoken "one moment" and doesn't flip state to `Thinking`. For
tools that need the session or per-turn data, use an `ISessionInterceptor` (`interceptors.md`).
