---
name: shiny-music
description: Generate code using Shiny.Music, a unified API for accessing the device music library on Android, iOS, and Mac Catalyst with permissions, metadata querying, filtering, playback, lyrics, album art, and file copy
auto_invoke: true
when_to_use: Use when the user needs device music-library access, playback, metadata queries, lyrics, album art, playlists, or track-copy workflows across Android, iOS, or Mac Catalyst.
triggers:
- "Access the device music library on Android, iOS, or Mac Catalyst"
- "Request permissions to read audio/music from the device"
- "Query or search music track metadata (title, artist, album, duration, year, explicit content, etc.)"
- "Get all distinct genres, years, or decades from the user's music library (with track counts)"
- "Browse playlists and get tracks within a playlist"
- "Filter tracks by genre, year, decade, and/or search text using `MusicFilter`"
- "Cross-query: get genres within a decade, years within a genre, etc."
- "Play, pause, resume, stop, or seek within music tracks"
- "Check for an active streaming subscription via `HasStreamingSubscriptionAsync()`"
- "Fetch lyrics for a track (plain text or synced LRC format)"
- "Retrieve album artwork for a track"
- "Copy music files from the device library to app storage"
- "Understand DRM limitations on iOS (Apple Music subscription tracks)"
- "Configure Android manifest permissions or iOS Info.plist for music access"
- "Create, remove, and manage playlists via `IMediaLibrary` playlist CRUD methods"
- "Add or remove tracks from playlists"
- "Browse playlists and their tracks (including custom playlists)"
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
        builder.UseMauiApp<App>();

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

Prompts the user for music library access. On Android, requests `READ_MEDIA_AUDIO` (API 33+) or `READ_EXTERNAL_STORAGE` (older). On Apple platforms, calls `MPMediaLibrary.RequestAuthorization`.

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

#### GetPlaylistsAsync

```csharp
Task<IReadOnlyList<PlaylistInfo>> GetPlaylistsAsync();
```

Returns all playlists with their song counts, sorted alphabetically by name. On Android, merges MediaStore playlists with locally-stored custom playlists. On Apple platforms, reads system playlists from `MPMediaQuery.PlaylistsQuery` and merges with locally-stored custom playlists. Permission must be granted first.

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

### IMusicPlayer

Controls playback of music files from the device library. Implements `IDisposable`.

- **Android**: Uses `Android.Media.MediaPlayer` with content URIs from MediaStore.
- **Apple platforms**: Uses `MPMusicPlayerController.ApplicationMusicPlayer`. Looks up the `MPMediaItem` by persistent ID via `MPMediaQuery` and sets the player queue.

#### PlayAsync

```csharp
Task PlayAsync(MusicMetadata track);
```

Stops any current track, loads the specified one, and begins playback. Throws `InvalidOperationException` if the track is not found in the music library.

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

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `State` | `PlaybackState` | Current state: `Stopped`, `Playing`, or `Paused` |
| `CurrentTrack` | `MusicMetadata?` | Currently loaded track, or `null` if stopped |
| `Position` | `TimeSpan` | Current playback position (`TimeSpan.Zero` if no track) |
| `Duration` | `TimeSpan` | Total duration of current track (`TimeSpan.Zero` if no track) |

#### Events

| Event | Description |
|-------|-------------|
| `StateChanged` | Raised on state transitions (e.g., Playing -> Paused) |
| `PlaybackCompleted` | Raised when a track finishes naturally (not via `Stop()`) |

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
    int PlayCount = 0
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
2. **Register as singletons** — both `IMediaLibrary` and `IMusicPlayer` should be singletons in DI.
3. **Dispose the player** — `IMusicPlayer` implements `IDisposable`; call `Dispose()` or let the DI container handle it.
4. **Test on physical devices** — simulators/emulators have no music content.
5. **Handle `Restricted` on Apple platforms** — distinct from `Denied`; means system policy blocks access.
6. **Copy format on Apple platforms is M4A** — regardless of original encoding, `AVAssetExportSession` outputs M4A.
7. **Use `HasStreamingSubscriptionAsync()`** — check before presenting streaming playback UI to the user.
8. **Use `MusicFilter` for combined queries** — filter tracks by genre + year/decade in a single call rather than filtering in memory.
9. **Use grouping methods with filters for cross-queries** — e.g., `GetGenresAsync(new MusicFilter { Decade = 1990 })` to find genres represented in the 90s.
10. **Use `GetPlaylistsAsync` and `GetPlaylistTracksAsync`** — browse playlists and retrieve their contents in playlist order.
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
