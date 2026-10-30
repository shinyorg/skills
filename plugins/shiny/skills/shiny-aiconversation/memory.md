# Assistant Memory

The assistant can keep notes for itself that are never read aloud or shown in the chat. **Memory is on
by default.** Every session gets a silent `memory` tool and, each turn, a short `<memory_index>` of the
documents it can see (paths, descriptions and sizes, wrapped as data, not content).

## Scopes

| Scope | Key | Use |
|---|---|---|
| `MemoryScope.Session` | session id | Scratchpad for one conversation; deleted with the session |
| `MemoryScope.Profile` | `AiSessionSettings.Profile` | Shared by sessions with the same profile |
| `MemoryScope.Global` | null | Facts about the user every session should know |

The tool defaults to `global` for `create`; other commands look in session, then profile, then global.

## The `memory` tool (what the model sees)

`memory(command: view|create|replace|append|delete|rename, path, scope?, content?, description?, old_str?, new_str?, new_path?, offset?)`

- `view` of a directory (`/`) lists documents; `view` of a long document returns pages (`offset`).
- Limit violations come back as tool results starting with `Error:` (never failed turns).
- Calls/results are stored as `Audience.ModelOnly` parts and flagged `Silent` (`ToolCallStarted.Silent == true`).

## Turning it off / limits

```csharp
opts.ConfigureMemory(m => m with { Enabled = false });                 // off everywhere
opts.ConfigureMemory(m => m with { DisabledProfiles = ["private"] });  // off for some profiles
opts.ConfigureMemory(m => m with
{
    MaxDocumentBytes = 16 * 1024,
    MaxIndexEntries = 50,
    MaxIndexTokens = 1500,
    MaxScopeBytes = 256 * 1024,      // over it: write refused + MemoryPressureChanged
    MaxWritesPerTurn = 20
});
```

## Letting the user see and edit everything (required)

`IAiSessionManager.Memory` is the app-facing `IAiMemoryService`. Writes through it are marked
`MemoryAuthor.User` and appear in the index as "[edited by the user]".

```csharp
var notes = await manager.Memory.List();                                   // all scopes, newest first
var prefs = await manager.Memory.Get(MemoryScope.Global, null, "/prefs.md");
await manager.Memory.Save(MemoryScope.Global, null, "/prefs.md", "Vegetarian. Metric units.", "Preferences");
await manager.Memory.Delete(MemoryScope.Global, null, "/prefs.md");
await manager.Memory.Clear();                                              // "forget everything"
```

`manager.Delete(sessionId)` also clears that session's `Session`-scoped memory.

## Events

```csharp
case MemoryChanged m:          ShowChip($"Remembered: {m.Document.Path}"); break;   // assistant writes only
case MemoryPressureChanged p:  WarnMemoryFull(p.Scope, p.UsedBytes, p.LimitBytes); break;
```

## Stores

Default `InMemoryAiMemoryStore`; `opts.SetSqliteDocDbStores(path)` persists memory with sessions; custom
via `opts.SetMemoryStore<T>()`. `IAiMemoryStore.Save(doc, expectedVersion)`: `expectedVersion == null`
creates (throw `MemoryConflictException` if the path exists), otherwise the stored version must match
(else `MemoryConflictException`); the caller sets the new version. Store keys are (scope, scopeKey, path).

Never put binary content in memory documents, and never use memory for instructions.
