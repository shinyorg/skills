---
name: shiny-appdevicebridge
description: Generate code using Shiny.AppDeviceBridge, device bridges on your app's own Shiny.Net.HttpServer that also host a web app (Blazor WebAssembly, React, Vue, any static build) inside a .NET MAUI app on Android, iOS, Mac Catalyst, Windows and the maui-labs macOS and Linux heads — served from a loopback HTTP server, updated over the air from a signed release server, and given device access through bridges with typed C# and TypeScript clients
auto_invoke: true
triggers:
  - Shiny.AppDeviceBridge
  - AppDeviceBridge
  - UseAppDeviceBridge
  - AddAppDeviceBridge
  - AppDeviceBridgeOptions
  - AppDeviceBridgeServer
  - AppDeviceBridgePolicies
  - AuthorizeBridges
  - AllowAnyCallerInDebug
  - BridgeCallers
  - IAppDeviceBridgeServerExtension
  - IWebAppBackgroundInvoker
  - UseWebAppHost
  - AddWebAppHost
  - Shiny.AppDeviceBridge.WebView
  - WebAppHost
  - WebAppHostView
  - WebAppHostPage
  - WebAppHostOptions
  - UseBaseline
  - AddWebAppReleases
  - MapWebAppReleases
  - IWebAppBridge
  - WebAppBridgeRoutes
  - WebAppBridgeResults
  - AddWebAppBridge
  - WebAppEventHub
  - WebAppInvoker
  - WebAppNativeCalls
  - WebAppEvents
  - AddWebAppHostClient
  - IBridgeTransport
  - BridgeException
  - BridgeClient
  - BridgeGet
  - BridgePost
  - BridgePut
  - BridgeDelete
  - BridgeEvent
  - BridgeBody
  - BridgeQuery
  - BridgeFile
  - ADB001
  - IHostBridge
  - ISettingsBridge
  - IFilesBridge
  - ILinksBridge
  - IAppBridge
  - ISensorsBridge
  - SensorsBridge
  - accelerometer
  - gyroscope
  - magnetometer
  - compass
  - barometer
  - IGpsBridge
  - IGeofencesBridge
  - IMotionBridge
  - IBluetoothLEBridge
  - IObdBridge
  - IWifiBridge
  - IDiscoveryBridge
  - IPushBridge
  - INotificationsBridge
  - ITransfersBridge
  - IHealthBridge
  - ISpeechBridge
  - IContactsBridge
  - ICalendarBridge
  - IPhotosBridge
  - IFoldersBridge
  - ITrayBridge
  - IQuickEntryBridge
  - ICameraBridge
  - AddCameraBridge
  - AddCameraBridgeClient
  - Shiny.AppDeviceBridge.Camera
  - CameraBridgeSession
  - CameraBridgeView
  - CameraBridgePage
  - ICameraBridgeController
  - ICameraCaptureStore
  - CameraSettingsInput
  - camera.status
  - remote viewfinder
  - device camera
  - IRpiCameraBridge
  - AddRpiCameraBridge
  - Shiny.AppDeviceBridge.RpiCamera
  - Raspberry Pi camera
  - libcamera
  - MJPEG stream
  - AddTrayIconBridge
  - AddQuickEntryBridge
  - Shiny.AppDeviceBridge.Desktop
  - quick entry
  - global hotkey
  - WebAppFileRoots
  - WebAppFileStore
  - WebAppFileRoot
  - AddPhotosBridge
  - AddFoldersBridge
  - photo picker
  - folder picker
  - background.js
  - ShinyHttpServerBuilder
  - AddShinyHttpServer
  - IsLocalConnection
  - IsTunneled
  - AppDeviceBridgeTunnel
  - AddAppDeviceBridgeTunnel
  - IAppDeviceBridgeTunnel
  - Shiny.AppDeviceBridge.Tunnel
  - TunnelState
  - QuickTunnelHost
  - public tunnel
  - pinggy
  - FolderRoots
  - FileRootsChanged
  - files.roots
  - MaxFileWriteBytes
  - WebAppPolicies
  - "@shinyorg/appdevicebridge"
  - hybrid web app
  - over-the-air web app updates
---

# Shiny.AppDeviceBridge

You are an expert in Shiny.AppDeviceBridge. Use this skill when the user hosts a web app inside a .NET MAUI app,
calls device features from that web app, updates it over the air, or writes a bridge of their own.

**Documentation:** https://shinylib.net/appdevicebridge

## The shape of it

- `Shiny.AppDeviceBridge` is the **bridge server**, on the app's own Shiny.Net.HttpServer: `http.AddAppDeviceBridge(o => …)`
  on the `ShinyHttpServerBuilder` from `services.AddShinyHttpServer(http => …)`. The server's address, port, TLS,
  limits, authentication and the app's own endpoints are configured on that same builder; `AppDeviceBridgeOptions`
  holds only the bridges' concerns (app id, mount points, allowed hosts, the bridge policy). `UseAppDeviceBridge` in
  MAUI calls `AddShinyHttpServer` for you. Every bridge route requires `AppDeviceBridgePolicies.Bridges`, which the
  bridges enforce themselves.
- **There is no private server.** Never generate `o.Server`, `o.ConfigureServer`, `o.AddAuthentication` or
  `o.AddAuthorization` on `AppDeviceBridgeOptions` — they do not exist. Use the builder.
- `Shiny.AppDeviceBridge.WebView` adds the **web app host** (`UseWebAppHost`): the web app served straight from a
  zip (the baseline or a signed download), shown in `WebAppHostView` / `WebAppHostPage`. The WebView trades a
  one-time launch token for an HttpOnly cookie, which the host adds to the bridge policy.
  Only the entry document gets `Cache-Control: no-cache`; the host sets no cache header on any other file.
- The server works without the WebView: bridges only, for callers the policy admits.
- **Bridges** are HTTP endpoints under `/_bridge/{name}` (the prefix is configurable) plus one Server-Sent
  Events stream. One package per bridge, one extension method each.
- **Every bridge has a typed client.** Never generate `fetch("/_bridge/…")` or JSON-object bodies in page code;
  use the bridge's client — C# for Blazor, TypeScript for everything else.

## The app (MAUI)

```csharp
builder
    .UseMauiApp<App>()
    .UseAppDeviceBridge(o => o.AppId = "field-app")         // bridges: AppId, BasePath, AllowedHosts, AuthorizeBridges
    .UseWebAppHost(o =>
    {
        o.UseBaseline(typeof(App).Assembly, "webapp.zip");      // offline, no update server needed
        // o.UpdateServer = new Uri("https://api.example.com/webapps");
        // o.PublicKey = "-----BEGIN PUBLIC KEY-----…";
    })
    .AddAppSupportBridge()
    .AddLocationBridges()
    .AddCalendarBridge()
    .AddPhotosBridge()
    .AddFoldersBridge();

// The server itself — address, auth, the app's own endpoints, non-MAUI registrations — on its builder, in any order:
builder.Services.AddShinyHttpServer(http =>
{
    http.Options.Address = IPAddress.Any;
    http.AddAuthentication().AddApiKey(k => k.AddKey(key, "kiosk"));
    http.Configure(server =>
    {
        server.UseAuthentication();
        server.UseAuthorization();
        server.MapGet("/api/orders", ctx => …).RequireAuthorization();
    });
}, autoStart: false);   // MAUI runs no hosted services; UseAppDeviceBridge starts the server

public class App : Application
{
    protected override Window CreateWindow(IActivationState? state) => new(new WebAppHostPage());
}
```

- `UseAppDeviceBridge` listens on loopback port 5780 unless the app sets a port of its own.
- Without MAUI: `services.AddShinyHttpServer(http => http.AddAppDeviceBridge(o => …).AddRpiCameraBridge())`.
- Bridge extensions register the Shiny service behind them. Do **not** also call `AddGps()`, `AddBluetoothLE()`
  and so on.
- A platform without an implementation answers `501`; `IHostBridge.GetInfoAsync()` lists every bridge with
  `IsSupported`.
- Platform setup is the underlying library's: usage descriptions, manifest permissions, entitlements. Loopback
  needs cleartext to `127.0.0.1` on Android and `NSAllowsLocalNetworking` on Apple platforms.

## Bridge packages

| Package | Registration | Client package / interface |
| --- | --- | --- |
| built in | (always) | `Shiny.AppDeviceBridge.Client`: `IHostBridge`, `ISettingsBridge`, `IFilesBridge`, `ILinksBridge` |
| `.AppSupport` | `AddAppSupportBridge()` | `IAppBridge` — info, orientation, browser, maps, store, launch at login, share, haptics, connectivity, battery, screen, clipboard; `ISensorsBridge` — start a sensor with a speed and `MinIntervalMs`, readings only as events (`OnCompassAsync`, …), all stopped when the page's event stream closes |
| `.Locations` | `AddGpsBridge()`, `AddGeofenceBridge()`, `AddMotionActivityBridge()` | `IGpsBridge`, `IGeofencesBridge`, `IMotionBridge` |
| `.BluetoothLE` | `AddBluetoothLEBridge()` | `IBluetoothLEBridge` |
| `.Obd` | `AddObdBridge()` | `IObdBridge` |
| `.Wifi` | `AddWifiBridge(hotspot)` | `IWifiBridge` |
| `.Discovery` | `AddDiscoveryBridge(protocols)` | `IDiscoveryBridge` |
| `.Push` | `AddPushBridge()` | `IPushBridge` |
| `.Notifications` | `AddNotificationsBridge()` | `INotificationsBridge` |
| `.HttpTransfers` | `AddHttpTransfersBridge()` | `ITransfersBridge` |
| `.AppLinks` | `AddAppLinksBridge(o => …)` | `ILinksBridge` (built in) |
| `.Health` | `AddHealthBridge()` | `IHealthBridge` |
| `.Speech` | `AddSpeechBridge()` | `ISpeechBridge` |
| `.Contacts` | `AddContactsBridge()` | `IContactsBridge` |
| `.Calendar` | `AddCalendarBridge()` | `ICalendarBridge` |
| `.Photos` | `AddPhotosBridge()` | `IPhotosBridge` |
| `.Camera` | `AddCameraBridge(o => …)` | `ICameraBridge` — this device's camera driven from a page anywhere; viewfinder MJPEG at `camera/preview` for an `<img>` (not Linux) |
| `.Folders` | `AddFoldersBridge()` — also registers `FolderRoots` for folders the app adds by path | `IFoldersBridge` |
| `.Desktop` | `AddTrayIconBridge()`, `AddQuickEntryBridge(o => o.HotKey = "Ctrl+Alt+Space")` | `Shiny.AppDeviceBridge.Desktop.Client`: `ITrayBridge`, `IQuickEntryBridge` (desktop only; `501` on mobile) |
| `.RpiCamera` | `http.AddRpiCameraBridge(o => …)` (the server's builder, for headless Pis) | `IRpiCameraBridge` — snapshots, captures into a file root, controls; live MJPEG at `rpicamera/stream` for an `<img>` (Linux + native shim only) |
| `.Jobs` | `AddWebAppJob(name, configure)` | native call `job:{name}` with `JobRun` |
| `.Tunnel` | `http.AddAppDeviceBridgeTunnel(o => o.Host = QuickTunnelHost.Pinggy)` | none — the app opens and closes it (`AppDeviceBridgeTunnel.StartAsync(token)` / `StopAsync()`) from its own UI or endpoints |

Client packages are `Shiny.AppDeviceBridge.{Bridge}.Client`, registered with `Add{Name}BridgeClient()` — the
name from the interface: `IAppBridge` → `AddAppBridgeClient()`, `ITransfersBridge` → `AddTransfersBridgeClient()`,
`ITrayBridge` → `AddTrayBridgeClient()`, `IQuickEntryBridge` → `AddQuickEntryBridgeClient()`. The desktop bridges share
`Shiny.AppDeviceBridge.Desktop.Client`.

## Quick entry

A prompt window over other applications. The page configures it and answers submissions; the answer also goes to
`background.js` when no page is open, so register the handler as a native call, not only an event:

```csharp
await quickEntry.SetPromptAsync(new QuickEntryPromptInput(Placeholder: "Ask…", Suggestions: [new("Sync now", Value: "sync")]));

await nativeCalls.HandleAsync("quickentry.submitted", QuickEntryJsonContext.Default.QuickEntrySubmission, async s =>
{
    await quickEntry.SetPromptAsync(new QuickEntryPromptInput(IsBusy: true));
    await quickEntry.SetPromptAsync(new QuickEntryPromptInput(IsBusy: false, Response: await AnswerAsync(s.Text)));
});
```

Null properties on `QuickEntryPromptInput` / `QuickEntryOptionsInput` leave values unchanged; `Response: ""` clears the
response and `HotKey: ""` removes the hotkey.

## A Blazor page

```csharp
// Program.cs
builder.Services
    .AddWebAppHostClient()            // transport + IHostBridge, ISettingsBridge, IFilesBridge, ILinksBridge
    .AddCalendarBridgeClient()
    .AddPhotosBridgeClient();
```

```razor
@inject ICalendarBridge Calendar
@inject IPhotosBridge Photos
@inject IFilesBridge Files
@implements IAsyncDisposable

@code {
    IAsyncDisposable? subscription;

    protected override async Task OnInitializedAsync()
    {
        var access = await Calendar.RequestAccessAsync(new CalendarAccessRequest());
        var page = await Calendar.GetEventsAsync(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(7), limit: 20);
    }

    async Task PickAsync()
    {
        foreach (var photo in await Photos.PickAsync(new PhotoPickRequest(Limit: 3)))
        {
            var bytes = await Files.ReadBytesAsync(photo.File.Root, photo.File.Path);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (subscription is not null)
            await subscription.DisposeAsync();
    }
}
```

Rules:

1. **Catch `BridgeException`**, never `HttpRequestException`: it carries `StatusCode`, the bridge's `Code` and
   `IsNotSupported` (501). Treat 501 as "hide the feature on this platform".
2. **Events are subscriptions.** `await using var sub = await Gps.OnReadingAsync(r => …)` — dispose it when the
   component goes away. Handlers run off the renderer; call `InvokeAsync(StateHasChanged)`.
3. **Settings take your own types** through `ISettingsBridge.GetAsync<T>(scope, key, typeInfo, default)` and
   `SetAsync<T>(…)` with a source-generated `JsonTypeInfo<T>`. Never reflection-based `JsonSerializer` calls.
4. **Files move by `BridgeFile { Root, Path }`.** Bridges that produce files (photos, exports) return one; read it
   through `IFilesBridge`. Bridges that take a file (share, notification image, tray icon) take one.
5. **Native calls** — background work handed to the page — are typed:
   `nativeCalls.HandleAsync("gps", LocationsJsonContext.Default.GpsReading, reading => …)`. The same names run in
   `background.js` when no page is open; that script is JavaScript (Jint), with `fetch` to `/_bridge`.

## A JavaScript / TypeScript page

```ts
import { BridgeError, CalendarBridge, FilesBridge, FoldersBridge, GpsBridge } from "@shinyorg/appdevicebridge";

const calendar = new CalendarBridge();
try {
    await calendar.createEvent({ title: "Standup", start: new Date(), end: new Date(Date.now() + 1_800_000) });
} catch (e) {
    if (e instanceof BridgeError && e.isNotSupported) hideCalendar();
}

const folder = await new FoldersBridge().pick({ root: "documents" });   // null when cancelled
if (folder) await new FilesBridge().writeText(folder.root, "notes.txt", "hello");

const stop = new GpsBridge().onReading(reading => console.log(reading.latitude));
stop();    // unsubscribes
```

- Method names are the C# names in camelCase without `Async`. Required parameters are positional; optional
  ones, and `signal`, go in the trailing options object.
- Events return an unsubscribe function. Enums are string unions (`"ReadWrite"`), dates accept `Date` or ISO
  strings, `byte[]` results come back as `Blob`.
- The clients discover the bridge prefix from `_host/config`, so a moved `BridgePrefix` or `BasePath` needs no
  page change.

## Folders and photos

- `IFoldersBridge.PickAsync(new FolderPickRequest("documents"))` shows the platform folder picker and makes the
  folder a **file root** named `documents`, remembered across launches (security-scoped bookmark on Apple, a
  persisted SAF grant on Android, the path on Windows and Linux). `ForgetAsync(root)` releases it. Null means
  cancelled. The app's own roots (`data`, `cache`) cannot be replaced.
- On Android a picked folder has no path: the files bridge works, but share, transfers and notification images
  refuse it.
- `IPhotosBridge.PickAsync` needs no permission. `GetLibraryAsync`/`GetThumbnailAsync`/`ExportAsync` need
  `RequestAccessAsync()` and `NSPhotoLibraryUsageDescription` / `READ_MEDIA_IMAGES`. The library is 501 on Linux.

## Device camera

Three different cameras — pick the right one:

- **The camera of the machine showing the page** → `getUserMedia` in the page, with `AllowWebPermissions(WebAppWebPermissions.Camera)`.
- **This device's camera, driven from a page elsewhere** (a phone on a mount, a laptop as the remote) → `.Camera`: `builder.AddCameraBridge()`, `ICameraBridge` in the page.
- **A Raspberry Pi camera module on a headless Pi** → `.RpiCamera`.

```csharp
await camera.OpenAsync();                                   // device shows CameraBridgePage unless the app handles OpenRequested
await using var sub = await camera.OnStatusAsync(s => …);   // wait for Live
var photo = await camera.TakePhotoAsync();                  // photo.File is a BridgeFile → IFilesBridge
await camera.UpdateSettingsAsync(new CameraSettingsInput(VideoMode: true, Zoom: 2));
```

- Commands need a camera screen open: handle `BridgeException` with `Code == "camera_not_open"` (409) as an instruction,
  `camera_state` (422) as "not now". Never poll `OpenAsync` in a loop.
- The viewfinder is `<img src="_bridge/camera/preview?t={ticks}">` — always a changing query.
- An app with its own camera screen: `CameraBridgeView` on the page, `o.PresentWhenOpened = false`, navigate from
  `CameraBridgeSession.OpenRequested` and set `e.Handled = true`. Custom filing: register `ICameraCaptureStore` first.

## Writing a bridge with a typed client

1. **Contracts project** (`net10.0`, references `Shiny.AppDeviceBridge.Client`): records plus a
   `[BridgeClient]` interface and its `JsonSerializerContext`.

```csharp
[BridgeClient("orders", typeof(OrdersJson))]
public interface IOrdersBridge
{
    [BridgeGet("{id}")] Task<Order> GetAsync(string id, CancellationToken cancellationToken = default);
    [BridgeGet] Task<OrderPage> ListAsync(int offset = 0, int limit = 50, CancellationToken cancellationToken = default);
    [BridgePost] Task<Order> CreateAsync(NewOrder order, CancellationToken cancellationToken = default);
    [BridgeDelete("{id}")] Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    [BridgeEvent("orders.changed")] Task<IAsyncDisposable> OnChangedAsync(Func<Order, Task> handler);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(OrderPage))]
[JsonSerializable(typeof(NewOrder))]
public partial class OrdersJson : JsonSerializerContext;
```

   Binding: a parameter named like a route token fills it; a complex type on POST/PUT (or `[BridgeBody]`) is the
   body; simple types are query parameters (`[BridgeQuery("name")]` renames). `Task<Stream>`/`Task<byte[]>` read raw.
   Anything else — a complex type on GET/DELETE, an unmatched token, two bodies — is build error **ADB001**.
2. **Native bridge** (MAUI project, references the contracts):

```csharp
public sealed class OrdersBridge(IOrderStore store, WebAppEventHub events) : IWebAppBridge
{
    public string Name => "orders";
    public bool IsSupported => true;

    public void Map(WebAppBridgeRoutes routes) => routes
        .MapGet("/{id}", async ctx => await (await store.FindAsync(ctx.Request.RouteValues["id"]!) is { } order
            ? WebAppBridgeResults.Json(ctx, order, OrdersJson.Default.Order)
            : WebAppBridgeResults.NotFound(ctx, "No such order.")));
}

builder.Services.AddWebAppBridge<OrdersBridge>();   // from a MauiAppBuilder extension
http.AddWebAppBridge<OrdersBridge>();               // a bridge with no MAUI dependency
```

   Answer failures with `WebAppBridgeResults.Error(ctx, status, code, message)`, `NotSupported` (501),
   `BadRequest`, `NotFound`. Publish events with `events.Publish(name, payload, typeInfo)`.
3. Map platform enums onto contract enums with `BridgeEnum.Convert<TFrom, TTo>` (by name) rather than casting.

## File roots at runtime

- **Folders the app maps** (a share, a folder from its own dialog) use `FolderRoots` from `.Folders`:
  `folders.Add(root, absolutePath, displayName)` keeps it as a root across launches on every platform,
  `folders.Forget(root)` removes it, `folders.All` lists them with `Available`. Failures are `WebAppFileException`
  (`bad_request`, `configured_root`, `folder_unavailable`). Never let the page supply the path.
- The page hears changes as `files.roots` — `IFilesBridge.OnRootsChangedAsync` / `FilesBridge.onRootsChanged` — and
  should re-list roots then.
- Lower level: `WebAppFileRoots` holds every root, `Add(WebAppFileStore)` / `Remove(name)`, raises `Changed`;
  `TryResolve(root, path, out fullPath)` for bridges that need a disk path. `WebAppFileRoot` needs an absolute path.
  Subclass `WebAppFileStore` for storage that is not a directory; throw `WebAppFileException` for failures.
- `MaxFileWriteBytes` raises the server's request body limit whichever bridges are registered, so the app's own
  upload endpoints get it too.

## Security — do not loosen

- Bridges are device access. The default policy admits callers on this device only (plus the WebView's session
  with the WebView host). **Debug builds admit any caller not arriving through a tunnel** (`AllowAnyCallerInDebug`,
  on by default) — say so when a user binds `http.Options.Address` past loopback.
- To open bridges to others, generate `o.AuthorizeBridges(p => …)` with a real credential from
  `http.AddAuthentication()...`, keeping `BridgeCallers.IsOnDevice(ctx.HttpContext)` for the device. Never a policy
  that allows everyone in release.
- **Tunnels deliver from loopback.** To ask whether a request came from this device use
  `BridgeCallers.IsLocalConnection(ctx)` or `IsOnDevice(ctx)` — never `IsLocal(ctx.Connection.RemoteIpAddress)` alone,
  which a tunnel caller passes. Tunneled requests must carry the tunnel's own host.
- The app's own endpoints are the app's: `http.AddAuthentication()`, `http.AddAuthorization(...)` (every call
  applies), `server.UseAuthentication(); server.UseAuthorization();` in `http.Configure`. Nothing is authenticated by
  default unless the app sets a fallback policy. `WebAppPolicies.Session` accepts only the WebView. Bridge policy and
  endpoint policies are separate, and the bridges enforce theirs without the app's pipeline.
- Update downloads are ECDSA P-256 signed; keep `PublicKey` compiled into the app.

## Trim and AOT

Everything is trim/AOT-clean. Generated code must use source-generated `JsonTypeInfo`, never reflection-based
serialization or `JsonObject` bodies.
