--
name: shiny-music
description: Generate code using Shiny.Music, a unified API for accessing the device music library on Android, iOS, and Mac Catalyst with permissions, metadata querying, filtering, playback, lyrics, album art, and file copy
auto_invoke: true
triggers:
  - music library
  - music player
  - device music
  - IMediaLibrary
  - IMusicPlayer
  - ILyricsProvider
  - LyricsResult
  - MusicMetadata
  - MusicFilter
  - GroupedCount
  - PlaylistInfo
  - media library
  - music permission
  - music playback
  - play music
  - AnalyzeLevelsAsync
  - AudioLevels
  - AudioSection
  - waveform
  - VU meter
  - song structure
  - instrumental gap
  - GetInstrumentalGaps
  - guitar solo
  - play from timestamp
  - VU meter
  - IVuMeter
  - VuLevel
  - CreateVuMeter
  - audio levels event
  - copy track
  - audio library
  - MediaStore audio
  - MPMediaQuery
  - music genre
  - genre query
  - music year
  - music decade
  - year query
  - decade query
  - filter tracks
  - playlist
  - playlists
  - GetGenresAsync
  - GetYearsAsync
  - GetDecadesAsync
  - GetTracksAsync
  - GetTrackByIdAsync
  - GetTracksByIdsAsync
  - GetPlaylistsAsync
  - GetPlaylistByIdAsync
  - GetPlaylistTracksAsync
  - GetAlbumArtPathAsync
  - GetLyricsAsync
  - album art
  - album artwork
  - lyrics
  - synced lyrics
  - LRC
  - ducking
  - audio ducking
  - Duck
  - DuckOptions
  - IsDucked
  - duck music
  - speak over music
  - volume
  - set volume
  - read volume
  - Volume
  - VolumeChanged
  - IsVolumeControlSupported
  - LRCLIB
  - Shiny.Music
  - music metadata
  - READ_MEDIA_AUDIO
  - NSAppleMusicUsageDescription
  - custom playlist
  - custom playlists
  - play count
  - play counts
  - CreatePlaylistAsync
  - RemovePlaylistAsync
  - AddTrackToPlaylistAsync
  - RemoveTrackFromPlaylistAsync
  - Shiny.Music.Extensions.AI
  - MusicAITools
  - AddMusicAITools
  - music AI tools
  - AIFunction music
  - Microsoft.Extensions.AI music
  - LLM music tools
  - chat client music
  - IChatClient music
  - pick a song for mood
  - play music with AI
  - SearchCatalogAsync
  - catalog search
  - Apple Music catalog
  - MusicCatalogSearchRequest
  - search Apple Music
  - stream catalog track
  - play catalog track
  - CatalogId
  - UseShiny
  - Shiny hosting
  - AndroidPlatform permission
  - Shiny.Core permission
---

# Shiny Music Skill

You are an expert in Shiny.Music, a .NET library that provides a unified API for accessing the device music library on Android, iOS, and Mac Catalyst. It supports permission management, querying track metadata, playing music files, fetching lyrics, retrieving album artwork, and copying tracks (where platform restrictions allow).

## When to Use This Skill

Invoke this skill when the user wants to:
- Access the device music library on Android, iOS, or Mac Catalyst
- Request permissions to read audio/music from the device
- Query or search music track metadata (title, artist, album, duration, year, explicit content, etc.)
- Get all distinct genres, years, or decades from the user's music library (with track counts)
- Browse playlists and get tracks within a playlist
- Filter tracks by genre, year, decade, and/or search text using `MusicFilter`
- Cross-query: get genres within a decade, years within a genre, etc.
- Play, pause, resume, stop, or seek within music tracks
- Check for an active streaming subscription via `HasStreamingSubscriptionAsync()`
- Fetch lyrics for a track (plain text or synced LRC format)
- Retrieve album artwork for a track
- Copy music files from the device library to app storage
- Understand DRM limitations on iOS (Apple Music subscription tracks)
- Configure Android manifest permissions or iOS Info.plist for music access
- Create, remove, and manage playlists via `IMediaLibrary` playlist CRUD methods
- Add or remove tracks from playlists
- Browse playlists and their tracks (including custom playlists)
- Expose the music library and player to an LLM agent as `Microsoft.Extensions.AI` tools (search, browse-by-mood, playback control, playlist management) via `Shiny.Music.Extensions.AI`

## Library Overview

- **Repository**: https://github.com/shinyorg/music
- **Namespace**: `Shiny.Music`
- **NuGet**: `Shiny.Music`
- **Targets**: `net10.0-android`, `net10.0-ios26.2`, `net10.0-maccatalyst26.2`
- **Docs**: https://shinylib.net/client/music

## Setup

### Dependency Injection (MAUI)

```csharp
using Shiny.Music;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShiny();   // REQUIRED (v4+): Android permission checks run on Shiny.Core's AndroidPlatform

        builder.Services.AddShinyMusic();

        return builder.Build();
    }
}
```

## Platform Configuration

### Android — AndroidManifest.xml

```xml
<!-- Android 13+ (API 33+) -->
<uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />

<!-- Android 12 and below (API < 33) -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                 android:maxSdkVersion="32" />
```

- **API 33+**: Uses the granular `READ_MEDIA_AUDIO` permission (audio files only).
- **API < 33**: Falls back to `READ_EXTERNAL_STORAGE`.
- Minimum supported API level: 24 (Android 7.0).

### Apple Platforms (iOS, Mac Catalyst) — Info.plist

```xml
<key>NSAppleMusicUsageDescription</key>
<string>This app needs access to your music library to browse and play your music.</string>
```

**This key is mandatory.** The app will crash on launch without it.

## Core API Reference

### IMediaLibrary

Provides access to the device music library including permissions, querying, and file operations.

#### RequestPermissionAsync

```csharp
Task<PermissionStatus> RequestPermissionAsync();
```

Prompts the user for music library access. On Android, requests `READ_MEDIA_AUDIO` (API 33+) or `READ_EXTERNAL_STORAGE` (older) via **Shiny.Core**'s `AndroidPlatform.RequestAccess` — this requires Shiny hosting (`.UseShiny()`), which is why `AddShinyMusic()` must be paired with `UseShiny()` on Android (**v4+**). On Apple platforms, calls `MPMediaLibrary.RequestAuthorization`.

Returns: `PermissionStatus` — `Granted`, `Denied`, `Restricted` (Apple platforms only), or `Unknown`.

#### CheckPermissionAsync

```csharp
Task<PermissionStatus> CheckPermissionAsync();
```

Checks the current permission status without prompting the user.

#### GetAllTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetAllTracksAsync();
```

Returns all music tracks on the device. Permission must be granted first. On Android, queries `MediaStore.Audio.Media` with `IsMusic != 0`. On Apple platforms, uses `MPMediaQuery.SongsQuery`. Only music is returned — no videos, ringtones, podcasts, or audiobooks.

#### SearchTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> SearchTracksAsync(string query);
```

Searches tracks by title, artist, or album. Case-insensitive partial string matching.

#### GetGenresAsync

```csharp
Task<IReadOnlyList<GroupedCount<string>>> GetGenresAsync(MusicFilter? filter = null);
```

Returns all distinct, non-null genre names from the user's music library with track counts, sorted alphabetically. When a `MusicFilter` is provided, only tracks matching the filter criteria are considered for grouping. Permission must be granted first.

#### GetYearsAsync

```csharp
Task<IReadOnlyList<GroupedCount<int>>> GetYearsAsync(MusicFilter? filter = null);
```

Returns all distinct, non-zero release years from the user's music library with track counts, sorted in ascending order. When a `MusicFilter` is provided, only tracks matching the filter criteria are considered. On Android, uses `MediaStore.Audio.Media.YEAR`; on Apple platforms, derives year from `MPMediaItem.ReleaseDate`.

#### GetDecadesAsync

```csharp
Task<IReadOnlyList<GroupedCount<int>>> GetDecadesAsync(MusicFilter? filter = null);
```

Returns all distinct decades with track counts, sorted in ascending order. Each decade is its starting year (e.g., 1990 for the 1990s). When a `MusicFilter` is provided, only tracks matching the filter criteria are considered.

#### GetTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetTracksAsync(MusicFilter filter);
```

Returns tracks matching the specified filter criteria. All non-null filter properties are combined with AND logic. On Android, genre filtering queries via `MediaStore.Audio.Genres.Members`; year/decade/search use SQL WHERE clauses. On Apple platforms, uses `MPMediaQuery.SongsQuery` with client-side LINQ filtering.

#### GetTrackByIdAsync

```csharp
Task<MusicMetadata?> GetTrackByIdAsync(string trackId);
```

Returns a single track by its platform-specific identifier (`MusicMetadata.Id`), or `null` if no track with that identifier exists. On Android, queries `MediaStore.Audio.Media` with an `Id = ?` selection. On Apple platforms, looks up the `MPMediaItem` with the matching persistent ID. Useful for restoring a previously-saved "now playing" track without loading the whole library.

#### GetTracksByIdsAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetTracksByIdsAsync(IEnumerable<string> trackIds);
```

Returns multiple tracks by identifier in a single query. Results are ordered to match the supplied `trackIds`; duplicate identifiers are collapsed and identifiers that do not resolve to a track are omitted. On Android, issues one `Id IN (...)` query; on Apple platforms, makes a single pass over `MPMediaQuery.SongsQuery`. Prefer this over calling `GetTrackByIdAsync` in a loop when hydrating a saved list of track IDs (e.g., a queue or favorites list).

#### GetPlaylistsAsync

```csharp
Task<IReadOnlyList<PlaylistInfo>> GetPlaylistsAsync();
```

Returns all playlists with their song counts, sorted alphabetically by name. On Android, merges MediaStore playlists with locally-stored custom playlists. On Apple platforms, reads system playlists from `MPMediaQuery.PlaylistsQuery` and merges with locally-stored custom playlists. Permission must be granted first.

#### GetPlaylistByIdAsync

```csharp
Task<PlaylistInfo?> GetPlaylistByIdAsync(string playlistId);
```

Returns a single playlist (with its song count) by identifier, or `null` if not found. Resolves both device playlists and locally-stored custom playlists (`custom:` prefixed IDs). On Android, reads device playlists from `MediaStore.Audio.Playlists`; on Apple platforms from `MPMediaQuery.PlaylistsQuery`. Use this to re-resolve a saved playlist's current name and count without enumerating every playlist via `GetPlaylistsAsync`.

#### GetPlaylistTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetPlaylistTracksAsync(string playlistId);
```

Returns all tracks in the specified playlist, in playlist order. The `playlistId` is the platform-specific identifier returned by `GetPlaylistsAsync`. On Android, queries `MediaStore.Audio.Playlists.Members` for platform playlists or the local JSON store for custom playlists. On Apple platforms, retrieves tracks from `MPMediaPlaylist` for system playlists or from the local JSON store for custom playlists.

#### GetAlbumArtPathAsync

```csharp
Task<string?> GetAlbumArtPathAsync(string trackId);
```

Returns a file path to the album artwork image for the specified track. On Android, returns the content URI for the album art from MediaStore. On Apple platforms, exports the `MPMediaItem.Artwork` image to a cached JPEG file and returns its path. Returns `null` if no artwork is available.

#### AnalyzeLevelsAsync

```csharp
Task<AudioLevels?> AnalyzeLevelsAsync(string trackId, TimeSpan? window = null);
```

Decodes a track to PCM **offline — without playing it** — and measures its amplitude. Use it to draw a waveform / VU meter, or to locate parts of a song (an intro, a chorus, a solo). Returns `null` for DRM-protected / streaming-only tracks that cannot be decoded to PCM (the same tracks `CopyTrackAsync` refuses) — **always null-check the result**.

- Both platforms copy the track to a **temporary file** and decode that (Android: `MediaExtractor` + `MediaCodec`; Apple: `AVAssetExportSession` → `AVAssetReader`), which lets analysis run **even while the same track is playing** (the live asset is otherwise held by the OS music player). Both down-mix to a single mono envelope at a fixed rate, so results are comparable across platforms.
- `window` is the analysis resolution (duration per RMS/peak entry); defaults to 500 ms.

`AudioLevels` contains:

- `Rms` / `Peak` — `IReadOnlyList<float>`, per-window levels normalized `0.0–1.0` against the track's loudest sample (entry `i` covers `[i*Window, (i+1)*Window)`).
- `Sections` — `IReadOnlyList<AudioSection>`: contiguous same-energy runs, each with `Start`, `Duration`, `End`, `AudioEnergy` (`Silent`/`Quiet`/`Moderate`/`Loud`), and `AverageLevel`. This is the coarse "song structure".

```csharp
var levels = await library.AnalyzeLevelsAsync(track.Id);
if (levels is not null)
{
    // the last loud stretch is often an outro solo
    var solo = levels.Sections.Where(s => s.Energy == AudioEnergy.Loud).MaxBy(s => s.Start);
    if (solo is not null)
    {
        await player.PlayAsync(track);
        player.Seek(solo.Start);
    }
}
```

#### CopyTrackAsync

```csharp
Task<bool> CopyTrackAsync(MusicMetadata track, string destinationPath);
```

Copies a music file to the specified path. Creates parent directories if needed. Returns `false` for DRM-protected tracks or on failure.

- **Android**: Reads from ContentResolver input stream. All local files can be copied. Original format preserved.
- **Apple platforms**: Exports via `AVAssetExportSession` in M4A format. DRM-protected Apple Music subscription tracks **cannot** be copied (`AssetURL` is null).

#### CreatePlaylistAsync

```csharp
Task<PlaylistInfo> CreatePlaylistAsync(string name);
```

Creates a new locally-stored custom playlist with the given name. On both platforms, playlists are persisted as JSON in local app data.

#### RemovePlaylistAsync

```csharp
Task RemovePlaylistAsync(string playlistId);
```

Removes a custom playlist by its identifier.

#### AddTrackToPlaylistAsync

```csharp
Task AddTrackToPlaylistAsync(string playlistId, MusicMetadata track);
```

Adds a track to an existing custom playlist. No-op if the track already exists in the playlist.

#### RemoveTrackFromPlaylistAsync

```csharp
Task RemoveTrackFromPlaylistAsync(string playlistId, string trackId);
```

Removes a track from an existing custom playlist.

### HasStreamingSubscriptionAsync

```csharp
Task<bool> HasStreamingSubscriptionAsync();
```

Checks whether the user has an active music streaming subscription that allows catalog playback. On Apple platforms, this queries MusicKit `MusicSubscription.GetCurrentAsync`. On Android, this always returns `false`.

### SearchCatalogAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> SearchCatalogAsync(string term, int limit = 25);
```

Searches the Apple Music streaming **catalog** (via MusicKit `MusicCatalogSearchRequest`) — unlike `SearchTracksAsync`, which searches only the user's *local* library, this returns catalog tracks that need not be in the library. Each result carries a `MusicMetadata.CatalogId` and is playable through `IMusicPlayer.PlayAsync` when the user has an active subscription. Catalog tracks are streaming-only: `ContentUri` is empty and they cannot be copied.

- **Apple platforms only** — throws `PlatformNotSupportedException` on Android.
- The first call triggers a MusicKit authorization prompt.
- `limit` is capped at 25 by Apple Music; defaults to 25.
- Returns an empty list when nothing matches, the user has no subscription/authorization, or the request fails (never throws on Apple).

```csharp
if (await library.HasStreamingSubscriptionAsync())
{
    var results = await library.SearchCatalogAsync("daft punk", limit: 10);
    if (results.Count > 0)
        await player.PlayAsync(results[0]); // streams by CatalogId
}
```

### IMusicPlayer

Controls playback of music files from the device library. Implements `IDisposable`.

- **Android**: Uses `Android.Media.MediaPlayer` with content URIs from MediaStore.
- **Apple platforms**: Uses `MPMusicPlayerController.ApplicationMusicPlayer`. For local tracks, looks up the `MPMediaItem` by persistent ID via `MPMediaQuery` and sets the queue. For streaming catalog tracks (those with a `CatalogId` from `SearchCatalogAsync`), enqueues by catalog id via `MPMusicPlayerStoreQueueDescriptor` — no library membership required, but an active subscription is.

#### PlayAsync

```csharp
Task PlayAsync(MusicMetadata track);
```

Stops any current track, loads the specified one, and begins playback. For local tracks, throws `InvalidOperationException` if the track is not found in the music library. Catalog tracks (with a `CatalogId`) are streamed by catalog id and do not require library membership (an active subscription is required).

- **Android**: Uses `Android.Media.MediaPlayer` with content URIs. Internally increments the play count in the local JSON store.
- **Apple platforms**: Uses `MPMusicPlayerController` — looks up the MPMediaItem by persistent ID, sets the queue, and starts playback.

#### Pause / Resume / Stop

```csharp
void Pause();   // No effect if not Playing
void Resume();  // No effect if not Paused
void Stop();    // Stops and releases the current track
```

#### Seek

```csharp
void Seek(TimeSpan position);
```

Seeks to the specified position. Android uses millisecond precision; Apple platforms use second precision.

#### CreateVuMeter

```csharp
IVuMeter CreateVuMeter(AudioLevels? implied = null, TimeSpan? interval = null);
```

Creates an event-based VU meter for the current playback. Call `Start()`, subscribe to `LevelChanged`, and `Dispose()` when done.

- **Android**: a **real audio-output tap** via `android.media.audiofx.Visualizer` when the app holds `RECORD_AUDIO` (`IsLive == true`). Add `<uses-permission android:name="android.permission.RECORD_AUDIO" />` and request it at runtime; without it, it falls back to the implied meter.
- **Apple**: the **implied** meter (`IsLive == false`) — levels are synthesized from `implied` (the `AnalyzeLevelsAsync` result) at the current playback position, since `MPMusicPlayerController` exposes no output tap. **Pass the analysis** or the meter emits silence.

`LevelChanged` may fire on a background thread — marshal to the UI before drawing. Each `VuLevel` has `Position`, `Rms`, `Peak` (0..1), and `Energy`.

```csharp
var levels = await _library.AnalyzeLevelsAsync(track.Id);
var meter = _player.CreateVuMeter(levels);
meter.LevelChanged += (_, level) =>
    MainThread.BeginInvokeOnMainThread(() => DrawVu(level.Rms, level.Peak));
meter.Start();
```

For a specific position without a running meter, use `audioLevels.SampleAt(position)` → `VuLevel`.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `State` | `PlaybackState` | Current state: `Stopped`, `Playing`, or `Paused` |
| `CurrentTrack` | `MusicMetadata?` | Currently loaded track, or `null` if stopped |
| `Position` | `TimeSpan` | Current playback position (`TimeSpan.Zero` if no track) |
| `Duration` | `TimeSpan` | Total duration of current track (`TimeSpan.Zero` if no track) |
| `IsDucked` | `bool` | Whether a duck scope is currently active |
| `Volume` | `float` | Device media volume, 0.0–1.0. **Getter works on all platforms.** Setter works on **Android** only; on **Apple platforms it throws `NotSupportedException`** (no supported OS API to set system volume) |
| `IsVolumeControlSupported` | `bool` | Whether setting `Volume` is supported: `true` on Android, `false` on Apple. Always check this before assigning `Volume` |

#### Events

| Event | Description |
|-------|-------------|
| `StateChanged` | Raised on state transitions (e.g., Playing -> Paused) |
| `PlaybackCompleted` | Raised when a track finishes naturally (not via `Stop()`) |
| `VolumeChanged` | `EventHandler<float>` — raised when the device media volume changes (hardware buttons, Control Center, or a successful `Volume` set); argument is the new volume 0.0–1.0 |

#### Duck (audio ducking)

```csharp
IAsyncDisposable Duck(DuckOptions? options = null);
```

Temporarily lowers the currently playing music so an announcement (e.g. text-to-speech) can be heard over top. Returns an `IAsyncDisposable` **scope** — full volume is restored when the scope is disposed. Wrap the announcement in an `await using` block:

```csharp
await using (player.Duck(new DuckOptions { Level = 0.2 }))
{
    await TextToSpeech.Default.SpeakAsync("Turn left in 200 meters");
}
// music ramps back to full volume here
```

- **Single active duck**: while a duck is already active, a second `Duck()` call returns a **no-op scope** — the existing duck is *not* superseded and keeps running until its own scope is disposed. This avoids overlapping restore/fades fighting over the volume. Dispose the active scope to restore full volume.
- Returns a **no-op scope** if nothing is currently playing (safe to call unconditionally).
- Check `player.IsDucked` to know whether a duck is active.
- **Android**: lowers this player's own track, honoring `DuckOptions.Level`, `FadeIn`, and `FadeOut` exactly.
- **Apple platforms**: activates `AVAudioSession` with `DuckOthers`. `Level` and the fade durations are **advisory only** — the OS controls duck depth and ramp. Announcement audio played through the app's audio session (an `AVAudioPlayer`, or `AVSpeechSynthesizer` with `UsesApplicationAudioSession = true`) plays at full volume over the ducked music; only other out-of-process audio is ducked.

#### DuckOptions

```csharp
public record DuckOptions
{
    public double Level { get; init; } = 0.2;                        // target volume 0.0–1.0 while ducked
    public TimeSpan FadeIn { get; init; } = TimeSpan.FromMilliseconds(200);   // ramp-down when ducking starts
    public TimeSpan FadeOut { get; init; } = TimeSpan.FromMilliseconds(200);  // ramp-up when the scope is disposed
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Level` | `double` | `0.2` | Target volume (0.0–1.0) while ducked. Android: applied exactly. Apple: ignored (OS-controlled) |
| `FadeIn` | `TimeSpan` | `200ms` | Ramp-down duration when ducking starts. Android: honored. Apple: ignored |
| `FadeOut` | `TimeSpan` | `200ms` | Ramp-up duration when the duck scope is disposed. Android: honored. Apple: ignored |

#### Volume

```csharp
float Volume { get; set; }         // device media volume, 0.0–1.0
bool IsVolumeControlSupported { get; }
event EventHandler<float>? VolumeChanged;
```

`Volume` is the **device-wide system media volume** (0.0–1.0), independent of any active duck. Reading works on all platforms; **setting is platform-limited** — always guard with `IsVolumeControlSupported`:

```csharp
// Read anywhere
var level = player.Volume;

// Set only where supported (Android). On Apple the setter throws NotSupportedException.
if (player.IsVolumeControlSupported)
    player.Volume = 0.5f;

// Observe changes (hardware buttons, Control Center, or a successful set)
player.VolumeChanged += (_, v) => Console.WriteLine($"Volume is now {v:P0}");
```

- **Android**: `Volume` reads/writes the system `STREAM_MUSIC` level via `AudioManager` (settable). This is separate from ducking, which attenuates only this player's own output. `IsVolumeControlSupported` is `true`. The stream is integer-stepped, so a set value is quantized to the nearest step and reading it back may differ slightly. Setting silently (no system volume UI) still raises `VolumeChanged`.
- **Apple platforms**: `Volume` reads `AVAudioSession.OutputVolume` (reliable). **The setter throws `NotSupportedException`** — iOS/Mac Catalyst expose no supported API to change the system volume (`MPMusicPlayerController.Volume` was deprecated in iOS 7 and is a no-op). `IsVolumeControlSupported` is `false`. Let the user adjust volume via the hardware buttons or an `MPVolumeView`. `VolumeChanged` fires from KVO on the audio session's output volume.

### ILyricsProvider

Provides lyrics for music tracks.

#### GetLyricsAsync

```csharp
Task<LyricsResult?> GetLyricsAsync(MusicMetadata track);
```

Returns lyrics for the specified track, or `null` if no lyrics are available. The result may contain plain text lyrics, synchronized lyrics in LRC format, or both.

- **Default implementation**: Uses the [LRCLIB](https://lrclib.net) service to fetch lyrics by artist name, track title, and duration. No API key required.

### LyricsResult

```csharp
public record LyricsResult(string? PlainLyrics, string? SyncedLyrics);
```

| Property | Type | Description |
|----------|------|-------------|
| `PlainLyrics` | `string?` | Plain text (unsynchronized) lyrics, or `null` if unavailable |
| `SyncedLyrics` | `string?` | Synchronized lyrics in LRC format with timestamps, or `null` if unavailable |

#### LRC Format

Synced lyrics use the standard LRC format with timestamps:

```
[00:12.00]First line of lyrics
[00:17.50]Second line of lyrics
[00:23.10]Third line of lyrics
```

Each line is prefixed with `[mm:ss.xx]` indicating when the line should be displayed during playback.

#### Instrumental gaps from synced lyrics (`LyricsExtensions`)

`LyricsExtensions` (namespace `Shiny.Music`) turns synced lyrics into the **instrumental (no-vocal) sections** of a track — intro, breaks, solos, outro — with **no audio decode**, so it works even for DRM tracks where `AnalyzeLevelsAsync` returns `null`.

```csharp
IReadOnlyList<InstrumentalGap> GetInstrumentalGaps(
    this LyricsResult? lyrics,
    TimeSpan? trackDuration = null,   // enables the trailing (outro) gap
    TimeSpan? minimumGap = null);     // default 8s — excludes ordinary pauses between lines

IReadOnlyList<LrcLine> ParseSyncedLyrics(this LyricsResult? lyrics);  // (Timestamp, Text) lines
```

`InstrumentalGap` has `Start`, `Duration`, and `End`. Requires `SyncedLyrics` (returns empty for plain-only lyrics). Combine with `AnalyzeLevelsAsync`: the lyric gaps give precise boundaries, the audio `Sections` tell you which gap is the loud solo versus the quiet intro.

```csharp
// "Start playing the guitar solo"
var lyrics = await _lyricsProvider.GetLyricsAsync(track);
var gaps = lyrics.GetInstrumentalGaps(track.Duration);
var solo = gaps.MaxBy(g => g.Duration);   // the longest instrumental stretch
if (solo is not null)
{
    await _player.PlayAsync(track);
    _player.Seek(solo.Start);
}
```

### MusicMetadata

```csharp
public record MusicMetadata(
    string Id,
    string? Title,
    string? Artist,
    string? Album,
    string? Genre,
    TimeSpan Duration,
    string? AlbumArtUri,
    bool? IsExplicit,
    string ContentUri,
    string? StoreId = null,
    int? Year = null,
    int PlayCount = 0,
    string? CatalogId = null
);
```

| Property | Description |
|----------|-------------|
| `Id` | Platform-specific unique ID. Android: MediaStore row ID. Apple platforms: MPMediaItem persistent ID. |
| `Title` | Track title, or `null` if not available. |
| `Artist` | Artist or performer, or `null` if not available. |
| `Album` | Album name, or `null` if not available. |
| `Genre` | Genre, or `null` if unavailable. |
| `Duration` | Playback duration. |
| `AlbumArtUri` | Album art URI. Android: MediaStore content URI. Apple platforms: `null` (use `GetAlbumArtPathAsync` for cached artwork). |
| `IsExplicit` | Whether the track is marked as explicit content. Apple platforms only via `MPMediaItem.IsExplicitItem`; always `null` on Android. |
| `ContentUri` | URI for playback/copy. Android: `content://` URI. Apple platforms: `ipod-library://` asset URL from `MPMediaItem.AssetURL` (empty string for DRM-protected tracks). |
| `StoreId` | Track persistent ID used for `MPMusicPlayerController` playback. Apple platforms only; `null` on Android. |
| `Year` | Release year of the track, or `null` if not available. Android: `MediaStore.Audio.Media.YEAR`; Apple platforms: derived from `MPMediaItem.ReleaseDate`. |
| `PlayCount` | Number of times the track has been played. Apple platforms: from `MPMediaItem.PlayCount`. Android: from locally stored JSON play counts (incremented automatically by the player). |
| `CatalogId` | Apple Music catalog identifier, set on tracks returned by `SearchCatalogAsync`. When present, the track is a streaming catalog item: `PlayAsync` enqueues it by this id via `MPMusicPlayerStoreQueueDescriptor` (subscription required) and `ContentUri` is empty (not copyable). `null` for local tracks and always `null` on Android. |

### PlaylistInfo

```csharp
public record PlaylistInfo(string Id, string Name, int SongCount);
```

| Property | Description |
|----------|-------------|
| `Id` | Platform-specific unique identifier. Android: MediaStore playlist row ID or `custom:` prefixed ID for custom playlists. Apple platforms: MPMediaPlaylist persistent ID or `custom:` prefixed ID for custom playlists. |
| `Name` | The display name of the playlist. |
| `SongCount` | The number of tracks in the playlist. |

### MusicFilter

Defines optional criteria for filtering music tracks. All specified properties are combined with AND logic. Used with `GetTracksAsync`, `GetGenresAsync`, `GetYearsAsync`, and `GetDecadesAsync`.

```csharp
public class MusicFilter
{
    public string? Genre { get; init; }
    public int? Year { get; init; }
    public int? Decade { get; init; }
    public string? SearchQuery { get; init; }
}
```

| Property | Description |
|----------|-------------|
| `Genre` | Filter by genre name (case-insensitive match). |
| `Year` | Filter by exact release year. Takes precedence over `Decade` if both are set. |
| `Decade` | Filter by decade start year (e.g., 1990 for the 1990s). Ignored if `Year` is also set. |
| `SearchQuery` | Text search across title, artist, and album (case-insensitive, contains match). |

### GroupedCount&lt;T&gt;

Returned by `GetGenresAsync`, `GetYearsAsync`, and `GetDecadesAsync`.

```csharp
public record GroupedCount<T>(T Value, int Count);
```

| Property | Description |
|----------|-------------|
| `Value` | The grouped value (`string` for genres, `int` for years/decades). |
| `Count` | The number of tracks that belong to this group. |

### PermissionStatus

| Value | Description |
|-------|-------------|
| `Unknown` | User has not been prompted yet |
| `Denied` | User denied access |
| `Granted` | User granted access |
| `Restricted` | Apple platforms only — blocked by system policy (parental controls, MDM) |

### PlaybackState

| Value | Description |
|-------|-------------|
| `Stopped` | No track playing; player is idle |
| `Playing` | A track is actively playing |
| `Paused` | Playback is paused and can be resumed |

## DRM and ContentUri

On Apple platforms, `ContentUri` is populated from `MPMediaItem.AssetURL` which provides an `ipod-library://` URL for locally-synced tracks. DRM-protected Apple Music subscription tracks have no `AssetURL` — their `ContentUri` will be empty. However, all tracks can still be played via `MPMusicPlayerController` using the `StoreId` (persistent ID).

| Track Source | ContentUri | Playable | Copyable |
|---|---|---|---|
| Apple platforms — local/purchased tracks | `ipod-library://` URL | yes | yes |
| Apple platforms — DRM subscription tracks | empty | yes (via MPMusicPlayerController) | no |
| Android local files | `content://` URI | yes | yes |

## Streaming Subscription Check

Use `HasStreamingSubscriptionAsync()` to determine if the user has an active Apple Music subscription (on Apple platforms):

```csharp
var canStream = await _library.HasStreamingSubscriptionAsync();
if (canStream)
{
    // User has an active Apple Music subscription
}
```

On Android, this always returns `false`.

## Code Generation Best Practices

1. **Always request permission first** — call `RequestPermissionAsync()` before any query or playback operation.
2. **Call `UseShiny()` on Android (v4+)** — Android permission checks run on Shiny.Core's `AndroidPlatform`, so the app must be under Shiny hosting (`.UseShiny()` in MAUI, or `ShinyAndroidApplication` + `ShinyAndroidActivity` natively). Without it, `IMediaLibrary` cannot resolve `AndroidPlatform`. Apple platforms have no such requirement.
3. **Register as singletons** — both `IMediaLibrary` and `IMusicPlayer` should be singletons in DI.
3. **Dispose the player** — `IMusicPlayer` implements `IDisposable`; call `Dispose()` or let the DI container handle it.
4. **Test on physical devices** — simulators/emulators have no music content.
5. **Handle `Restricted` on Apple platforms** — distinct from `Denied`; means system policy blocks access.
6. **Copy format on Apple platforms is M4A** — regardless of original encoding, `AVAssetExportSession` outputs M4A.
7. **Use `HasStreamingSubscriptionAsync()`** — check before presenting streaming playback UI to the user.
8. **Use `MusicFilter` for combined queries** — filter tracks by genre + year/decade in a single call rather than filtering in memory.
9. **Use grouping methods with filters for cross-queries** — e.g., `GetGenresAsync(new MusicFilter { Decade = 1990 })` to find genres represented in the 90s.
10. **Use `GetPlaylistsAsync` and `GetPlaylistTracksAsync`** — browse playlists and retrieve their contents in playlist order.
14. **Rehydrate saved IDs with the by-id getters** — when restoring persisted state (now-playing track, a saved queue, a favorited playlist), use `GetTrackByIdAsync` / `GetPlaylistByIdAsync` rather than scanning `GetAllTracksAsync`/`GetPlaylistsAsync`. For a list of track IDs, use `GetTracksByIdsAsync` (single query) instead of a loop of `GetTrackByIdAsync`. All three return `null`/omit entries for IDs that no longer resolve — always null-check.
11. **Use `GetAlbumArtPathAsync`** — retrieve album artwork as a cached file path for display in the UI.
12. **Use `ILyricsProvider.GetLyricsAsync`** — fetch lyrics for a track. Check `SyncedLyrics` for timed LRC format, fall back to `PlainLyrics` for plain text.
13. **Playlist CRUD is on `IMediaLibrary`** — use `CreatePlaylistAsync`, `RemovePlaylistAsync`, `AddTrackToPlaylistAsync`, `RemoveTrackFromPlaylistAsync`. On both platforms these manage locally-stored custom playlists.
17. **Play counts** — on Apple platforms, `PlayCount` comes from `MPMediaItem.PlayCount` (system-tracked). On Android, play counts are incremented internally when `PlayAsync` is called and stored locally.

## Playlist Management Examples

```csharp
// Create a playlist
var playlist = await _library.CreatePlaylistAsync("My Favorites");

// Add a track to the playlist
await _library.AddTrackToPlaylistAsync(playlist.Id, track);

// Browse all playlists
var playlists = await _library.GetPlaylistsAsync();
foreach (var p in playlists)
    Console.WriteLine($"{p.Name} ({p.SongCount} songs)");

// Get tracks in a playlist
var tracks = await _library.GetPlaylistTracksAsync(playlist.Id);

// Remove a track from a playlist
await _library.RemoveTrackFromPlaylistAsync(playlist.Id, track.Id);

// Remove a playlist
await _library.RemovePlaylistAsync(playlist.Id);
```

## Lyrics Examples

```csharp
// Fetch lyrics for a track
var lyrics = await _lyricsProvider.GetLyricsAsync(track);
if (lyrics != null)
{
    if (!string.IsNullOrEmpty(lyrics.SyncedLyrics))
    {
        // Parse LRC format for synced display
        // Format: [mm:ss.xx]Line of lyrics
        foreach (var line in lyrics.SyncedLyrics.Split('\n'))
            Console.WriteLine(line);
    }
    else if (!string.IsNullOrEmpty(lyrics.PlainLyrics))
    {
        // Display plain text lyrics
        Console.WriteLine(lyrics.PlainLyrics);
    }
}
```

## Album Art Examples

```csharp
// Get album artwork path
var artPath = await _library.GetAlbumArtPathAsync(track.Id);
if (artPath != null)
{
    // Use as image source in MAUI
    var imageSource = ImageSource.FromFile(artPath);
}
```

## Filtering Examples

```csharp
// All Rock tracks
var rockTracks = await library.GetTracksAsync(new MusicFilter { Genre = "Rock" });

// All tracks from the 1990s
var nineties = await library.GetTracksAsync(new MusicFilter { Decade = 1990 });

// Rock tracks from 1995
var rock95 = await library.GetTracksAsync(new MusicFilter { Genre = "Rock", Year = 1995 });

// Genres in the 2000s (with counts)
var genres2000s = await library.GetGenresAsync(new MusicFilter { Decade = 2000 });

// Years for Jazz (with counts)
var jazzYears = await library.GetYearsAsync(new MusicFilter { Genre = "Jazz" });

// Decades for Pop (with counts)
var popDecades = await library.GetDecadesAsync(new MusicFilter { Genre = "Pop" });

// Combined: genres matching "rock" search in the 1980s
var rock80s = await library.GetGenresAsync(new MusicFilter { Decade = 1980, SearchQuery = "rock" });

// Browse all playlists
var playlists = await library.GetPlaylistsAsync();
foreach (var p in playlists)
    Console.WriteLine($"{p.Name} ({p.SongCount} songs)");

// Get tracks in a playlist
var playlistTracks = await library.GetPlaylistTracksAsync(playlists[0].Id);
```

## AI Tools (Microsoft.Extensions.AI)

The **separate** `Shiny.Music.Extensions.AI` NuGet package exposes the music library and player as
[`Microsoft.Extensions.AI`](https://learn.microsoft.com/dotnet/ai/) `AIFunction` tools, so an LLM chat
agent can search/browse the library, pick a track for a mood, control playback, and manage playlists.
It is AOT-compatible (hand-authored JSON schemas, no reflection) and depends only on
`Microsoft.Extensions.AI.Abstractions`.

- **NuGet**: `Shiny.Music.Extensions.AI` (in addition to `Shiny.Music`)
- **Namespace**: `Shiny.Music.Extensions.AI`
- **Entry points**: `AddMusicAITools(...)` (DI) and the resolvable `MusicAITools` bundle (`.Tools`)

### Registration — opt-in areas

Each `Add…` opts a group of tools in; anything not added is invisible to the LLM. `AddMusicAITools`
requires `AddShinyMusic()` first (it resolves `IMediaLibrary`/`IMusicPlayer`/`ILyricsProvider`).

```csharp
using Shiny.Music.Extensions.AI;

builder.Services.AddShinyMusic();
builder.Services.AddMusicAITools(tools => tools
    .AddLibrary()             // search_tracks, browse_tracks, list_music_categories,
                              // list_playlists, get_playlist_tracks, get_lyrics
    .AddPlayback()            // play_track, control_playback, get_now_playing
    .AddPlaylistManagement()  // create_playlist, modify_playlist, delete_playlist
);
// or: builder.Services.AddMusicAITools(tools => tools.AddAll());  // the three areas above

// AddCatalog() (search_catalog) is Apple-only and NOT in AddAll(). Guard the opt-in
// with a compiler flag so the tool is only offered where it works:
builder.Services.AddMusicAITools(tools =>
{
    tools.AddLibrary().AddPlayback().AddPlaylistManagement();
#if IOS || MACCATALYST
    tools.AddCatalog();       // search_catalog — Apple Music streaming catalog
#endif
});
```

### Using the tools with a chat client

```csharp
var tools = sp.GetRequiredService<MusicAITools>().Tools;
var response = await chatClient.GetResponseAsync(
    messages,
    new ChatOptions { Tools = [.. tools] }
);
```

### Generated tools

| Tool | Area | Arguments | Purpose |
|---|---|---|---|
| `search_tracks` | Library | `query` (required), `limit` | Free-text search over title/artist/album |
| `browse_tracks` | Library | `genre`, `year`, `decade`, `query`, `limit` | Filter the library — the "pick a song for the mood" path |
| `list_music_categories` | Library | `kind` (`genres`\|`years`\|`decades`, required), `genre` | Distinct categories with track counts |
| `list_playlists` | Library | — | All playlists with ids and song counts |
| `get_playlist_tracks` | Library | `playlist_id` (required), `limit` | Tracks in a playlist |
| `analyze_song_structure` | Library | `track_id` (required) | Instrumental gaps (from synced lyrics, DRM-safe) + audio-energy sections (offline scan, null for DRM) in seconds — to start playback at a solo/chorus. No audio is played |
| `get_lyrics` | Library | `track_id` (required) | Plain and/or synced lyrics (needs `ILyricsProvider`) |
| `search_catalog` | Catalog *(Apple-only)* | `query` (required), `limit` | Search the Apple Music streaming catalog (not just the local library); returns ids usable with `play_track` |
| `play_track` | Playback | `track_id` (required), `start_seconds` | Load and play a track by id (local or catalog); optionally start partway in (e.g. at a solo from `analyze_song_structure`) |
| `control_playback` | Playback | `action` (`pause`\|`resume`\|`stop`\|`seek`, required), `position_seconds` | Transport control |
| `get_now_playing` | Playback | — | Current state, track, position, duration |
| `create_playlist` | Playlist mgmt | `name` (required) | Create a custom playlist |
| `modify_playlist` | Playlist mgmt | `action` (`add_track`\|`remove_track`), `playlist_id`, `track_id` (all required) | Add/remove a track |
| `delete_playlist` | Playlist mgmt | `playlist_id` (required) | Delete a custom playlist |

### Notes

- Every tool returns a structured JSON object; failures come back as `{ "error": "..." }` rather than throwing.
- Track ids flow between tools: `search_tracks`/`browse_tracks`/`get_playlist_tracks`/`search_catalog` return
  `id`s that `play_track`, `get_lyrics`, and `modify_playlist` consume. Catalog results carry a `catalogId`
  and aren't in the local library — `play_track` remembers them from the search so it can stream them by id.
- `AddCatalog()` (the `search_catalog` tool) is **Apple-only** and is **not** included in `AddAll()`. Guard the
  opt-in with `#if IOS || MACCATALYST` so it isn't offered on Android (where the underlying call throws).
- The tools **assume music-library permission is already granted** — they never trigger the permission
  UI (which needs a foreground activity). Call `IMediaLibrary.RequestPermissionAsync()` from the app first.
- `AddPlayback()` throws at registration if no `IMusicPlayer` is available (i.e. `AddShinyMusic()` was not
  called on a platform target).
