# Camera Tool (`use_camera`)

Lets the assistant ask the user to point the camera at something. The tool logic lives in core
(`CameraToolInterceptor` over `ICameraCapture` / `ICameraConsent`); `Shiny.AiConversation.Maui.Camera` supplies
the MAUI capture (Shiny's `IMediaService` CameraView modal), an alert consent prompt and registration.
Requires `Microsoft.Maui.Controls` >= 10.0.100 (Shiny.Maui.Controls.Camera 2.x).

## Registration

```csharp
builder
    .UseMauiApp<App>()
    .UseShinyControls()
    .UseShinyCamera();                     // registers IMediaService

builder.Services.AddShinyAiConversation(o => o.UseFileAttachments(Path.Combine(FileSystem.AppDataDirectory, "ai_attachments")));
builder.Services.AddAiConversationCamera(c => c
    .Configure(o => o with { RequireConfirmation = true, MaxDimension = 1568 })
    // scan modes come from analyzer packages the app installs
    .AddScanner(CameraMode.Barcode, async (media, request, ct) => (await media.ScanBarcodeAsync())?.Value)
    .AddScanner(CameraMode.Text, (media, request, ct) => media.ScanTextStringAsync())
    // .SetConsent<MyConsent>()            // default: AlertCameraConsent
);
```

Permissions: `NSCameraUsageDescription` in iOS `Info.plist`; `android.permission.CAMERA` in `AndroidManifest.xml`.

## What the model sees

```text
use_camera(purpose, mode?: photo|text|barcode|document, facing?: back|front)
```

`mode` only lists modes the capture service supports (photo always, plus registered scanners) and
`CameraToolOptions.AllowedModes` permits. `purpose` is required and becomes the camera page title.

## CameraToolOptions

| Property | Default | Notes |
|---|---|---|
| `RequireConfirmation` | `true` | Ask via `ICameraConsent` before opening |
| `ImageRetention` | `DropAfterTurn` | For photos sent as follow-up messages (see `images.md`) |
| `MaxDimension` | `1568` | Longest edge of photos |
| `AllowedModes` | null | null = every supported mode |
| `Profiles` | `[]` | Limit the tool to sessions with these profiles |

## Flow

1. The tool is added per turn (`OnBeforeTurn`), so it closes over that turn's `TurnContext`
2. Consent declined → result `"The user declined to open the camera."`; cancel → `"The user cancelled the camera."`
3. Every `IAudioFocusParticipant` (the voice conversation registers itself) is paused while the camera is open, then resumed
4. Scan modes return text: `"barcode result:\n…"`
5. Photos (EXIF/location stripped):
   - `IChatClientProvider.SupportsImagesInToolResults == true` → returned inline as `[TextContent, DataContent]`;
     stored history keeps only `"[image/jpeg attachment]"`, so the model sees it for that turn only
   - otherwise (most providers) → the round ends (`Terminate`) and the photo is queued with
     `TurnContext.QueueFollowUp` as a `MessageSource.Camera` user message with an `ImagePart`; the model answers in
     a second round of the **same** turn, so voice speaks the answer normally

## Custom capture / consent

```csharp
public class MyConsent : ICameraConsent
{
    public Task<bool> Confirm(CameraRequest request, CancellationToken ct) => ...; // request.Purpose, .Mode, .Facing
}

public class MyCapture : ICameraCapture
{
    public IReadOnlyList<CameraMode> SupportedModes => [CameraMode.Photo];
    public Task<CameraCapture?> Capture(CameraRequest request, CancellationToken ct)
        => Task.FromResult<CameraCapture?>(new CameraCapture(bytes, "image/jpeg"));   // or new CameraCapture(Text: "...")
}
```
