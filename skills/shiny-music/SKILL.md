--
name: shiny-music
description: Generate code using Shiny.Music, a unified API for accessing the device music library on Android and iOS with permissions, metadata querying, playback, and file copy
auto_invoke: true
triggers:
  - music library
  - music player
  - device music
  - IMediaLibrary
  - IMusicPlayer
  - MusicMetadata
  - media library
  - music permission
  - music playback
  - play music
  - copy track
  - audio library
  - MediaStore audio
  - MPMediaQuery
  - Shiny.Music
  - music metadata
  - READ_MEDIA_AUDIO
  - NSAppleMusicUsageDescription
---

# Shiny Music Skill

You are an expert in Shiny.Music, a .NET library that provides a unified API for accessing the device music library on Android and iOS. It supports permission management, querying track metadata, playing music files, and copying tracks (where platform restrictions allow).

## When to Use This Skill

Invoke this skill when the user wants to:
- Access the device music library on Android or iOS
- Request permissions to read audio/music from the device
- Query or search music track metadata (title, artist, album, duration, etc.)
- Play, pause, resume, stop, or seek within music tracks
- Copy music files from the device library to app storage
- Understand DRM limitations on iOS (Apple Music subscription tracks)
- Configure Android manifest permissions or iOS Info.plist for music access

## Library Overview

- **Repository**: https://github.com/shinyorg/music
- **Namespace**: `Shiny.Music`
- **NuGet**: `Shiny.Music`
- **Targets**: `net10.0-android`, `net10.0-ios`
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

### iOS — Info.plist

```xml
<key>NSAppleMusicUsageDescription</key>
<string>This app needs access to your music library to browse and play your music.</string>
```

**This key is mandatory.** The app will crash on launch without it. No special entitlements are required.

## Core API Reference

### IMediaLibrary

Provides access to the device music library including permissions, querying, and file operations.

#### RequestPermissionAsync

```csharp
Task<PermissionStatus> RequestPermissionAsync();
```

Prompts the user for music library access. On Android, requests `READ_MEDIA_AUDIO` (API 33+) or `READ_EXTERNAL_STORAGE` (older). On iOS, calls `MPMediaLibrary.RequestAuthorization`.

Returns: `PermissionStatus` — `Granted`, `Denied`, `Restricted` (iOS only), or `Unknown`.

#### CheckPermissionAsync

```csharp
Task<PermissionStatus> CheckPermissionAsync();
```

Checks the current permission status without prompting the user.

#### GetAllTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> GetAllTracksAsync();
```

Returns all music tracks on the device. Permission must be granted first. On Android, queries `MediaStore.Audio.Media` with `IsMusic != 0`. On iOS, uses `MPMediaQuery` filtered to `MPMediaType.Music`. Only music is returned — no videos, ringtones, podcasts, or audiobooks.

#### SearchTracksAsync

```csharp
Task<IReadOnlyList<MusicMetadata>> SearchTracksAsync(string query);
```

Searches tracks by title, artist, or album. Case-insensitive partial string matching.

#### CopyTrackAsync

```csharp
Task<bool> CopyTrackAsync(MusicMetadata track, string destinationPath);
```

Copies a music file to the specified path. Creates parent directories if needed. Returns `false` for DRM-protected tracks or on failure.

- **Android**: Reads from ContentResolver input stream. All local files can be copied. Original format preserved.
- **iOS**: Exports via `AVAssetExportSession` in M4A format. DRM-protected Apple Music subscription tracks **cannot** be copied (`AssetURL` is null).

### IMusicPlayer

Controls playback of music files from the device library. Implements `IDisposable`.

#### PlayAsync

```csharp
Task PlayAsync(MusicMetadata track);
```

Stops any current track, loads the specified one, and begins playback. Throws `InvalidOperationException` if `ContentUri` is empty or the platform player fails.

- **Android**: Uses `Android.Media.MediaPlayer` with content URIs.
- **iOS**: Uses `AVFoundation.AVAudioPlayer` with `ipod-library://` asset URLs.

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

Seeks to the specified position. Android uses millisecond precision; iOS uses second precision.

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
| `StateChanged` | Raised on state transitions (e.g., Playing → Paused) |
| `PlaybackCompleted` | Raised when a track finishes naturally (not via `Stop()`) |

### MusicMetadata

```csharp
public record MusicMetadata(
    string Id,
    string Title,
    string Artist,
    string Album,
    string? Genre,
    TimeSpan Duration,
    string? AlbumArtUri,
    string ContentUri
);
```

| Property | Description |
|----------|-------------|
| `Id` | Platform-specific unique ID. Android: MediaStore row ID. iOS: MPMediaItem persistent ID. |
| `Title` | Track title. |
| `Artist` | Artist or performer. |
| `Album` | Album name. |
| `Genre` | Genre, or `null` if unavailable. |
| `Duration` | Playback duration. |
| `AlbumArtUri` | Album art URI (Android only via MediaStore; `null` on iOS). |
| `ContentUri` | URI for playback/copy. Android: `content://` URI. iOS: `ipod-library://` asset URL. **Empty string for DRM-protected Apple Music tracks** — these cannot be played or copied. |

### PermissionStatus

| Value | Description |
|-------|-------------|
| `Unknown` | User has not been prompted yet |
| `Denied` | User denied access |
| `Granted` | User granted access |
| `Restricted` | iOS only — blocked by system policy (parental controls, MDM) |

### PlaybackState

| Value | Description |
|-------|-------------|
| `Stopped` | No track playing; player is idle |
| `Playing` | A track is actively playing |
| `Paused` | Playback is paused and can be resumed |

## DRM and ContentUri

On iOS, Apple Music subscription tracks are DRM-protected. For these tracks:
- `MPMediaItem.AssetURL` is `null`
- `MusicMetadata.ContentUri` will be `string.Empty`
- `PlayAsync` will throw (no valid URL)
- `CopyTrackAsync` will return `false`

Always check `string.IsNullOrEmpty(track.ContentUri)` before attempting playback or copy on iOS.

| Track Source | ContentUri | Playable | Copyable |
|---|---|---|---|
| iTunes purchases (DRM-free) | ✅ populated | ✅ | ✅ |
| Locally synced from computer | ✅ populated | ✅ | ✅ |
| Apple Music subscription | ❌ empty | ❌ | ❌ |
| iTunes Match (cloud) | ⚠️ only if downloaded | ⚠️ | ⚠️ |
| Android local files | ✅ always populated | ✅ | ✅ |

## Code Generation Best Practices

1. **Always request permission first** — call `RequestPermissionAsync()` before any query or playback operation.
2. **Check `ContentUri` before playing on iOS** — empty means DRM-protected, will throw.
3. **Register as singletons** — both `IMediaLibrary` and `IMusicPlayer` should be singletons in DI.
4. **Dispose the player** — `IMusicPlayer` implements `IDisposable`; call `Dispose()` or let the DI container handle it.
5. **Test on physical devices** — simulators/emulators have no music content.
6. **Handle `Restricted` on iOS** — distinct from `Denied`; means system policy blocks access.
7. **Copy format on iOS is M4A** — regardless of original encoding, `AVAssetExportSession` outputs M4A.
