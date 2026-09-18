---
name: shiny-appdevicebridge
description: Generate code using Shiny.AppDeviceBridge, device bridges on your app's own Shiny.Net.HttpServer that also host a web app (Blazor WebAssembly, React, Vue, any static build) inside a .NET MAUI app on Android, iOS, Mac Catalyst, Windows and the maui-labs macOS and Linux heads — served from a loopback HTTP server, updated over the air from a signed release server, and given device access through bridges with typed C# and TypeScript clients
auto_invoke: true
triggers:
  - Shiny.AppDeviceBridge
  - AppDeviceBridge
  - UseAppDeviceBridge
  - AddAppDeviceBridge
  - AppDeviceBridgeBuilder
  - MauiAppDeviceBridgeBuilder
  - AddBridge
  - AppDeviceBridgeOptions
  - AppDeviceBridgeServer
  - AppDeviceBridgePolicies
  - AuthorizeBridges
  - AllowAnyCallerInDebug
  - BridgeCallers
  - IAppDeviceBridgeServerExtension
  - IWebAppMainThread
  - IWebAppBackgroundInvoker
  - Shiny.AppDeviceBridge.WebView
  - WebAppHost
  - WebAppHostView
  - WebAppHostPage
  - WebAppHostOptions
  - UseBaseline
  - ServeWebAppRemotely
  - ServeWebAppLocally
  - OnPrepareResponse
  - ContentTypeOverrides
  - SelectVariant
  - client variants
  - mobile and desktop client
  - AddWebAppReleases
  - MapWebAppReleases
  - IWebAppBridge
  - WebAppBridgeRoutes
  - WebAppBridgeResults
  - WebAppEventHub
  - WebAppEventSource
  - AddAppSupportLinux
  - UPowerBattery
  - Shiny.AppDeviceBridge.AppSupport.Linux
  - WebAppEventStream
  - MapEvent
  - EventStreamHeartbeat
  - bridge.stream
  - bridge.error
  - not_listening
  - WebAppInvoker
  - UseTrafficMonitor
  - AddTrafficRecorder
  - TrafficRecorder
  - TrafficRecorderOptions
  - TrafficMonitorPage
  - ShowTrafficMonitorAsync
  - TrafficText
  - Shiny.AppDeviceBridge.Simulator
  - shiny-bridge-sim
  - bridge simulator
  - simulate bridges
  - GPX trail
  - traffic monitor
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
  - RpiCameraStreamer
  - StreamToAsync
  - RpiCameraStreamSettings
  - RpiCameraStreamStatistics
  - RpiCameraStreamEnd
  - RpiCameraFrameStream
  - RpiCameraFrameHeader
  - RpiCameraStreamFrame
  - ReadFramesAsync
  - camera over L2CAP
  - camera over Bluetooth LE
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
  - AddTunnel
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
  - UseDelegate
  - WebAppNotificationDelegate
  - "@shinyorg/appdevicebridge"
  - hybrid web app
  - over-the-air web app updates
---

# Shiny.AppDeviceBridge

You are an expert in Shiny.AppDeviceBridge. Use this skill when the user hosts a web app inside a .NET MAUI app,
calls device features from that web app, updates it over the air, or writes a bridge of their own.

**Documentation:** https://shinylib.net/appdevicebridge

## The shape of it

- `Shiny.AppDeviceBridge` is the **bridge server**, on the app's own Shiny.Net.HttpServer:
  `http.AddAppDeviceBridge(bridge => …)` on the `ShinyHttpServerBuilder` from `services.AddShinyHttpServer(http => …)`.
  The server's address, port, TLS, limits, authentication and the app's own endpoints are configured on that same
  builder; `AppDeviceBridgeOptions` holds only the bridges' concerns (app id, mount points, allowed hosts, the bridge
  policy), set with `bridge.Configure(o => …)`. `UseAppDeviceBridge` in MAUI calls `AddShinyHttpServer` for you.
- **Bridges register on the bridge builder, never on `MauiAppBuilder` or `ShinyHttpServerBuilder`.**
  `AppDeviceBridgeBuilder` has `Http`, `Services`, `Options`, `Configure(Action<AppDeviceBridgeOptions>)` and
  `AddBridge<TBridge>()`; creating one registers the bridge server with the built-in host, settings, files and invoke
  bridges. In MAUI, both `UseAppDeviceBridge` overloads — `(bridge => …, webApp => …)` with a web app, `(bridge => …)` without — hand out a
  `MauiAppDeviceBridgeBuilder` (adds `Maui`, the `MauiAppBuilder`) and do the host work once: `UseShiny()` (guarded, so an
  app that already calls it is fine) and the app's dispatcher registered as `IWebAppMainThread`. **Never generate
  `UseShiny()` for the bridges.** Calling `UseAppDeviceBridge` again adds to the same server.
- **Two kinds of bridge package.**
  - **No MAUI** — BluetoothLE, Obd, Discovery, Wifi, HttpTransfers, Jobs (plain `net10.0`), Locations
    (GPS/geofences/motion), Notifications, Push, Speech, Calendar, Contacts, Health, RpiCamera, Tunnel. They reference
    only `Shiny.AppDeviceBridge`; their extensions are generic (`TBuilder AddGpsBridge<TBuilder>(this TBuilder bridge)
    where TBuilder : AppDeviceBridgeBuilder`) and return the builder they were given, so they chain on either builder and
    run headless (on macOS they register Shiny's core services themselves).
  - **MAUI** — AppSupport, AppSupport.Linux, AppLinks, Camera, Photos, Folders, Desktop (tray, quick entry). They reference
    `Shiny.AppDeviceBridge.Maui`, extend `MauiAppDeviceBridgeBuilder`, and do their own MAUI registration through
    `bridge.Maui` (camera control, tray icon, controls, Essentials, lifecycle events). The app calls none of that.
- `AllowWebPermissions` and `UseTrafficMonitor` stay on `MauiAppBuilder`; `AddTrafficRecorder` stays on
  `ShinyHttpServerBuilder`.
- Every bridge route requires `AppDeviceBridgePolicies.Bridges`, which the bridges enforce themselves.
- **There is no private server.** Never generate `o.Server`, `o.ConfigureServer`, `o.AddAuthentication` or
  `o.AddAuthorization` on `AppDeviceBridgeOptions` — they do not exist. Use the builder.
- `Shiny.AppDeviceBridge.WebView` adds the **web app host** — the `UseAppDeviceBridge(bridge => …, webApp => …)`
  overload (bridge delegate first, both required; `http.AddAppDeviceBridge(bridge => …, webApp => …)` without MAUI).
  One delegate registers the bridges alone; a second configures the web app. It serves the web app straight from a
  zip (the baseline or a signed download), shown in `WebAppHostView` / `WebAppHostPage`. The WebView trades a
  one-time launch token for an HttpOnly cookie, which the host adds to the bridge policy.
  Only the entry document gets `Cache-Control: no-cache`; the host sets no cache header on any other file.
  `WebAppHostOptions.OnPrepareResponse` runs after that for every file (an app's own cache policy for LAN/tunnel
  callers), and `ContentTypeOverrides` maps extensions (with the dot) to content types.
- Who gets the pages: the WebView (launch session) always; other machines and tunnels only with
  `ServeWebAppRemotely = true`; a browser on the same device only with `ServeWebAppLocally = true`. Neither option
  opens the bridges — that is `AuthorizeBridges`. Never generate a custom middleware to let a local browser in.
- **Two clients (phone and desktop) at the same URLs**: `webApp.Variants("mobile", "desktop")` plus
  `webApp.SelectVariant = ctx => …` (cookie, `Sec-CH-UA-Mobile`, `User-Agent`), with one zip holding `mobile/` and
  `desktop/`. Never generate a second mount point, a redirect to `/desktop`, or two web app hosts for this. The
  first variant is the default; a null/unknown/throwing selector falls back to it.
- The server works without the WebView: bridges only, for callers the policy admits.
- **Moving the server at runtime** (a LAN switch, a port setting) is `await http.StopAsync(); http.Options.Address = …;
  http.Options.Port = …; await http.StartAsync();` on the app's `HttpServer`. `Origin`, the WebView session and an open
  tunnel follow on their own; never re-register the bridges or rebuild the host to change the port.
- **Bridges** are HTTP endpoints under `/_bridge/{name}` (the prefix is configurable) plus one Server-Sent
  Events stream. One package per bridge, one extension method each on the bridge builder. Host, settings, files and
  invoke are built in; AppSupport (`app/…`, `/_bridge/sensors`) is its own package, added with `AddAppSupportBridge()`.
- **The event stream carries only named topics.** `GET /_bridge/events?topics=a,b` opens it; the first event,
  `bridge.stream`, carries the id for `PUT /_bridge/events/{id}` `{ "topics": [...] }`. The host runs each topic's
  native source only while a stream names it, and unhooks it when the page drops it, disconnects, or the source throws
  (`bridge.error { event, message }`). The typed clients manage topics — never hand-write `new EventSource(...)` in a
  page that has them.
- **Subscribe before starting a session.** BLE scans and characteristic notifications, Health listeners, the OBD
  monitor, Discovery browses and dictation answer `409 not_listening` without a listener for their result event, and
  stop by themselves once it has none; each sensor stops once its event has none. `await` the subscription (it
  resolves once the host is delivering), then start.
- **Every bridge has a typed client.** Never generate `fetch("/_bridge/…")` or JSON-object bodies in page code;
  use the bridge's client — C# for Blazor, TypeScript for everything else.

## The app (MAUI)

```csharp
builder
    .UseMauiApp<App>()
    .UseAppDeviceBridge(
        bridge => bridge
            .Configure(o => o.AppId = "field-app")   // AppId, BasePath, AllowedHosts, AuthorizeBridges
            .AddAppSupportBridge()
            .AddLocationBridges()
            .AddCalendarBridge()
            .AddPhotosBridge()
            .AddFoldersBridge(),
        webApp =>
        {
            webApp.UseBaseline(typeof(App).Assembly, "webapp.zip");   // offline, no update server needed
            // webApp.UpdateServer = new Uri("https://api.example.com/webapps");
            // webApp.PublicKey = "-----BEGIN PUBLIC KEY-----…";
        }
    );

// No web app, bridges only: the other overload, with just the bridge delegate.
// builder.UseAppDeviceBridge(bridge => bridge.Configure(o => o.AppId = "kiosk").AddGpsBridge());

// A head adding its own bridges (desktop only, say) adds to the same server:
// builder.UseAppDeviceBridge(bridge => bridge.AddTrayIconBridge());

// The server itself — address, auth, the app's own endpoints — on its builder, in any order:
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
- Without MAUI: `services.AddShinyHttpServer(http => http.AddAppDeviceBridge(bridge => bridge.Configure(o => …).AddRpiCameraBridge().AddTunnel().AddBridge<ClipboardBridge>()))`.
  A non-MAUI web host is the same overload on the server's builder: `http.AddAppDeviceBridge(bridge => …, webApp => …)`.
- Bridge extensions register the Shiny service behind them. Do **not** also call `AddGps()`, `AddBluetoothLE()`
  and so on.
- A platform without an implementation answers `501`; `IHostBridge.GetInfoAsync()` lists every bridge with
  `IsSupported`.
- Platform setup is the underlying library's: usage descriptions, manifest permissions, entitlements. Loopback
  needs cleartext to `127.0.0.1` on Android and `NSAllowsLocalNetworking` on Apple platforms.

## Bridge packages

| Package | Registration (on the bridge builder) | Client package / interface |
| --- | --- | --- |
| built in | (always) | `Shiny.AppDeviceBridge.Client`: `IHostBridge`, `ISettingsBridge`, `IFilesBridge`, `ILinksBridge` |
| `.AppSupport` | `AddAppSupportBridge()` | `IAppBridge` — info, orientation, browser, maps, store, launch at login, share, haptics, connectivity, battery, screen, clipboard; `ISensorsBridge` — start a sensor with a speed and `MinIntervalMs`, readings only as events (`OnCompassAsync`, …), each stopped once nothing listens to its event |
| `.AppSupport.Linux` | `AddAppSupportLinux()` on the GTK4 head, after `AddAppSupportBridge()` | battery and energy saver from UPower and power-profiles-daemon, with change events. The maui-labs GTK4 battery never raises them, so a Linux head without this gets no `app.battery` events |
| `.Locations` | `AddGpsBridge()`, `AddGeofenceBridge()`, `AddMotionActivityBridge()` | `IGpsBridge`, `IGeofencesBridge`, `IMotionBridge` |
| `.BluetoothLE` | `AddBluetoothLEBridge()` | `IBluetoothLEBridge` |
| `.Obd` | `AddObdBridge()` | `IObdBridge` |
| `.Wifi` | `AddWifiBridge(hotspot)` | `IWifiBridge` |
| `.Discovery` | `AddDiscoveryBridge(protocols)` | `IDiscoveryBridge` |
| `.Push` | `AddPushBridge()` | `IPushBridge` |
| `.Notifications` | `AddNotificationsBridge()`; a custom delegate: `AddNotificationsBridge(o => o.UseDelegate<MyNotificationDelegate>())` (subclass `WebAppNotificationDelegate`) | `INotificationsBridge` |
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
| `.RpiCamera` | `AddRpiCameraBridge(o => …)` (either builder — a headless Pi's too) | `IRpiCameraBridge` — snapshots, captures into a file root, controls; live MJPEG at `rpicamera/stream` for an `<img>`; `ICameraService.StreamToAsync(stream)` for a pipe with no HTTP, read with `ReadFramesAsync` (Linux + native shim only) |
| `.Jobs` | `AddWebAppJob(name, configure)` | native call `job:{name}` with `JobRun` |
| `.Tunnel` | `AddTunnel(o => o.Host = QuickTunnelHost.Pinggy)` (either builder) | none — the app opens and closes it (`AppDeviceBridgeTunnel.StartAsync(token)` / `StopAsync()`) from its own UI or endpoints |

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

## Traffic monitor

A debug window onto every request the server answers — files, bridge calls (including 401/403/421 refusals), the app's
own endpoints. Register it in a debug build only, and open it from something of the app's own:

```csharp
#if DEBUG
builder.UseTrafficMonitor(o => o.RedactRequestBody = ctx => ctx.Request.Path == "/api/login");
#endif

traffic.Clicked += async (_, _) => await page.HostView.ShowTrafficMonitorAsync();   // or TrafficMonitorPage.ShowAsync(Navigation)
```

In memory only (newest 300, text bodies up to 128 KB). `Authorization`, `Proxy-Authorization`, `Cookie`, `Set-Cookie`
and the `token`/`access_token` query parameters are redacted by default — that is what keeps the WebView's launch token
and session cookie out of it; do not clear those sets in shipped code. Without MAUI: `http.AddTrafficRecorder()` and read
`TrafficRecorder.Snapshot()` / `Changed`; `TrafficText` (`Status`, `Headers`, `Body`, `Filter`, `Describe`) formats an
exchange the way the pages do. When overlaying a button on `WebAppHostPage`, set `page.Content = null` before
putting `HostView` in a new layout — replacing the content un-parents the old one and the view's `Navigation` goes dead.

## Simulator (testing a page without a device)

`shiny-bridge-sim` (.NET tool `Shiny.AppDeviceBridge.Simulator`) serves every bridge from its `[BridgeClient]` interface,
answering with values set in its TUI, a scenario file or a trail. Recommend it for testing a page's behavior against
specific device states (offline, permission denied, `501` on a platform, a GPS walk) — no page changes:

```bash
shiny-bridge-sim --dev-server http://localhost:5288            # or --app <published wwwroot>; page at http://127.0.0.1:5299/
shiny-bridge-sim --scenario setup.json --trail walk.gpx --play walk --speed 4 --headless   # CI
```

- A route answers `value` (JSON validated against the contract; a file for byte routes), `null` (204) or `error`
  (status + `code` + `message`, e.g. `403 access_denied`), with optional `DelayMs`. A bridge can be switched off (all `501`).
- Values may hold `"$now"`, `"$now-5m"`, `"$now+2h"`, `"$uuid"`, filled in when sent.
- Trail JSON: `{ "name": "...", "loop": false, "steps": [ { "delayMs": 0, "event": "wifi.changed", "payload": {...} },
  { "delayMs": 5000, "bridge": "wifi", "route": "GET current", "mode": "null" }, { "bridge": "ble", "supported": false } ] }`.
  Route keys are `"<METHOD> <pattern>"` from the interface (`GET current`, `DELETE regions/{identifier}`, `GET` for the root).
- `.gpx` loads as `gps.reading` events plus `GET gps/current`/`GET gps/last` values at the recorded pace.
- A new bridge must be added to `BridgeCatalog` and the simulator's csproj; `SimulatorCatalogTests` fail until it is.

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
   component goes away; the last disposal for an event stops its native source. Await it before starting what feeds
   it (a scan, a listener). Handlers run off the renderer; call `InvokeAsync(StateHasChanged)`.
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

const stop = await new GpsBridge().onReading(reading => console.log(reading.latitude));
stop();    // unsubscribes
```

- Method names are the C# names in camelCase without `Async`. Required parameters are positional; optional
  ones, and `signal`, go in the trailing options object.
- Events return a promise of an unsubscribe function; await it before starting what feeds the event. Enums are string unions (`"ReadWrite"`), dates accept `Date` or ISO
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
- **This device's camera, driven from a page elsewhere** (a phone on a mount, a laptop as the remote) → `.Camera`: `bridge.AddCameraBridge()`, `ICameraBridge` in the page.
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

## Pi camera into a Stream (L2CAP)

A page watches a Pi camera with `<img src="_bridge/rpicamera/stream">`. A pipe with no HTTP around it, a Bluetooth LE
L2CAP channel above all, uses `ICameraService.StreamToAsync` (namespace `Shiny.AppDeviceBridge.RpiCamera`) on the device and
`ReadFramesAsync` (namespace `Shiny.AppDeviceBridge.RpiCamera.Client`) on the viewer. It is not a bridge route, so there is no `501`:
off Linux, or without the native shim, it throws `CameraUnavailableException`.

```csharp
// Pi (Shiny.BluetoothLE.Hosting 5.6.5+): ICameraService comes from AddRpiCameraBridge(); send ticket.Psm + ticket.Token to the phone
var ticket = await broker.Reserve("camera", TimeSpan.FromSeconds(30), (channel, ct) =>
    camera.StreamToAsync(channel, new RpiCameraStreamSettings { MaxFps = 8 }, cancellationToken: ct));

// Phone (Shiny.BluetoothLE 5.6.5+)
await using var channel = await peripheral.OpenL2CapTicketChannel(psm, token);
await foreach (var frame in channel.ReadFramesAsync(ct))   // RpiCameraStreamFrame: Jpeg, Sequence, Width, Height, TimestampMs
    Show(frame.Jpeg);
```

- Call `StreamToAsync` once the viewer has connected (in the broker handler), never ahead of it: it opens the camera
  session on start and releases it on return.
- `RpiCameraStreamSettings` defaults: 640×480, `Quality` 60, `MaxFps` 10, `MaxDuration` 5 minutes. Keep them low for
  BLE; frames over `MaxFps` are dropped before encoding and counted in `RpiCameraStreamStatistics.FramesDropped`.
- It returns `RpiCameraStreamEnd.CameraStopped` or `MaxDurationReached`; cancellation throws `OperationCanceledException`,
  a viewer that left throws `IOException`. It never disposes the destination.
- `ReadFramesAsync` ends cleanly when the stream closes between frames; mid-frame is `EndOfStreamException`, a bad
  header `InvalidDataException`. A gap in `Sequence` is dropped frames, not an error.
- Wire format (`RpiCameraFrameStream`): 28-byte little-endian header `[length u32][sequence u32][width u32][height u32][fourcc u32 "MJPG"][timestampMs i64]`
  then the JPEG; `WriteFrameAsync`, `WriteHeader`, `TryReadHeader`, `MaxFrameBytes` (8 MB).
- It holds the camera outside the bridge: `GET rpicamera` doesn't list it and `DELETE rpicamera/streams` doesn't stop it.

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
public sealed class OrdersBridge(IOrderStore store) : IWebAppBridge
{
    public string Name => "orders";
    public bool IsSupported => true;

    public void Map(WebAppBridgeRoutes routes) => routes
        // Hooked per page stream that names the event; unhooked in finally when it lets go, disconnects or throws.
        .MapEvent("orders.changed", ct => WebAppEventStream.FromEvent<Order>(emit =>
        {
            EventHandler<Order> handler = (_, order) => emit(order);
            store.Changed += handler;
            return () => store.Changed -= handler;
        }, ct), OrdersJson.Default.Order)
        .MapGet("/{id}", async ctx => await (await store.FindAsync(ctx.Request.RouteValues["id"]!) is { } order
            ? WebAppBridgeResults.Json(ctx, order, OrdersJson.Default.Order)
            : WebAppBridgeResults.NotFound(ctx, "No such order.")));
}

bridge.AddBridge<OrdersBridge>();   // on the bridge builder, MAUI or headless

// Or an extension of your own, like the packages. No MAUI needed: generic over the builder, returning the one it was given.
public static TBuilder AddOrdersBridge<TBuilder>(this TBuilder bridge) where TBuilder : AppDeviceBridgeBuilder
{
    bridge.AddBridge<OrdersBridge>();
    return bridge;
}
// Needs MAUI registration (a control, lifecycle events, Essentials): extend MauiAppDeviceBridgeBuilder and use bridge.Maui.
```

   Answer failures with `WebAppBridgeResults.Error(ctx, status, code, message)`, `NotSupported` (501),
   `BadRequest`, `NotFound`.
   **Native UI (a permission prompt, a picker) goes through `IWebAppMainThread`** — resolve it and call
   `InvokeAsync(...)`; the MAUI host routes it to the dispatcher, headless it runs inline. Never
   `Application.Current.Dispatcher` or `MainThread` in a bridge. An `IAppDeviceBridgeServerExtension` can override
   `Initialize(IServiceProvider)`, called once when the container builds the server, before any request.
   **Events are `IAsyncEnumerable<T>` mapped with `routes.MapEvent(name, ct => …, typeInfo)`** — never a hub-wide
   publish, never a hook in the constructor or `Dispose`. A native .NET event: `WebAppEventStream.FromEvent<T>(emit => {
   hook; return unhook; }, ct)` (an overload takes an async hook for main-thread-only APIs). Values your own code raises
   (a Shiny delegate, a session loop): a `WebAppEventSource<T>` — `Publish` is a no-op with no listener,
   `ListenAsync(stopped, ct)` reports the listeners remaining so a session can stop at zero — or
   `routes.Events.Source(name, typeInfo)` for a delegate registered outside the bridge. A session the page starts
   should answer `409 not_listening` while `source.HasListeners` is false.
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
- To open bridges to others, generate `bridge.Configure(o => o.AuthorizeBridges(p => …))` with a real credential from
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
