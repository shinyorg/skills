---
name: shiny-universal-hubs
description: Generate code using Shiny.UniversalHubs - Shiny.BluetoothLE.Hubs hubs served over Wi-Fi (HttpServer Switchboard + mDNS) as well as Bluetooth LE, with merged BLE/mDNS discovery, Wi-Fi-first connect with BLE fallback, resume, per hub start/stop, tunnels, file transfers and media channels (live audio/video packet pipes) on either transport
auto_invoke: true
triggers:
  - universal hubs
  - universalhubs
  - Shiny.UniversalHubs
  - hubs over wifi
  - ble hubs over wifi
  - hub over wifi and ble
  - wifi and bluetooth hub
  - wifi fallback ble
  - ble fallback
  - local network hub
  - mdns hub
  - device to device wifi
  - UniversalHub
  - UniversalHubOptions
  - AddUniversalServer
  - UniversalServerBuilder
  - AddUniversalHubs
  - UniversalHubHostBuilder
  - UniversalHubHostOptions
  - UniversalHubFileRequest
  - AuthorizeFile
  - AllowTunnel
  - IUniversalHubHost
  - StartHub
  - StopHub
  - GetRunningTransports
  - RunningTransports
  - HubTransports
  - HubConnection
  - AddUniversalHubClient
  - IUniversalHubClient
  - UniversalHubClientOptions
  - DiscoverAll
  - HubHostInfo
  - FromUrl
  - HubConnectOptions
  - Prefer
  - AutomaticReconnect
  - HttpRediscoverTimeout
  - UniversalHubProtocol
  - NSBonjourServices
  - mdns service type
  - media channel
  - media channels over wifi
  - live audio over hub
  - live video over hub
  - video call over hub
  - EnableMediaChannels
  - OpenMediaChannel
  - CanOpenMediaChannel
  - MediaChannelOpened
  - IBleHubMediaChannel
  - MediaTokenHeader
  - MediaPath
---

# Shiny.UniversalHubs Skill

You are an expert in Shiny.UniversalHubs, a library that serves **Shiny.BluetoothLE.Hubs** hubs over **Wi-Fi as well
as Bluetooth LE**. Write the hub once: clients on a plane connect over BLE, clients at home connect over Wi-Fi (more
range and bandwidth, no MTU), and both kinds share the same hub, groups and pushes.

- The **contract and hub are plain BluetoothLE Hubs**: a `[BleHubClient]` interface, a source generated proxy,
  `Clients`, `Groups`, `IHubContext<THub>`. Use the `shiny-ble-hubs` skill for contract rules (SBH diagnostics),
  serialization, `Clients` targets, groups and streaming. This library only carries the calls.
- The **Wi-Fi side** is a Shiny.Net.HttpServer Switchboard (SSE + POST, with resume), advertised over mDNS.
- The **client** merges BLE scanning and mDNS browsing into one list, connects over Wi-Fi first and falls back to BLE.

## When to Use This Skill

Invoke this skill when the user wants to:
- Serve a BluetoothLE Hubs hub over Wi-Fi / the local network as well as BLE
- Discover hub hosts over BLE and mDNS at once, and connect over whichever is reachable
- Know which transport a client is on, inside the hub or on the client
- Run an always-on hub beside one that runs on demand, on both transports
- Make a hub reachable from the internet through a tunnel, or connect to a host by URL / QR code
- Move files over HTTP on Wi-Fi and L2CAP on BLE behind the same calls
- Stream live audio or video (calls) between devices on a hub, over Wi-Fi or Bluetooth

Do NOT use this skill for BLE-only hubs (use `shiny-ble-hubs`), generic HTTP servers (use `shiny-httpserver`) or
generic mDNS discovery (use `shiny-discovery`).

## Library Overview

| NuGet | Purpose |
|---|---|
| `Shiny.UniversalHubs.Host` | `AddUniversalServer`, `UniversalHub<T>`, `HubConnection`, `IUniversalHubHost`, file routes, mDNS |
| `Shiny.UniversalHubs.Client` | `AddUniversalHubClient<T>`, `IUniversalHubClient<T>`, `HubHostInfo`, `HubConnectOptions` |
| `Shiny.UniversalHubs` | The shared Wi-Fi wire protocol (`HubTransports`, `UniversalHubProtocol`). Comes with both |

- **Namespace**: `Shiny.UniversalHubs` (plus `Shiny.BluetoothLE.Hubs` for the contract and hub types)
- **Platforms**: iOS, Android and Mac Catalyst register the BLE stacks for you. On plain `net10.0` (Linux, tests,
  macOS AppKit) register an `IBleHostingManager` / `IBleManager` yourself.
- Hosting is **foreground only**, the same as BLE hosting.

## Code Generation Instructions

### 1. The contract (shared by host and client)

A normal BluetoothLE Hubs contract - see `shiny-ble-hubs` for the rules:

```csharp
using Shiny.BluetoothLE.Hubs;

[BleHubClient]
public interface IGameHub
{
    Task<MoveResult> MakeMove(int cell);
    event Action<GameState> StateChanged;
}

[JsonSerializable(typeof(MoveResult))]
[JsonSerializable(typeof(GameState))]
[JsonSerializable(typeof(int))]
public partial class GameJsonContext : JsonSerializerContext;

Shiny.Json.AddContext(GameJsonContext.Default);
```

Values travel as JSON over Wi-Fi and are re-framed, never re-serialized, so a custom `IBleHubSerializer` **must produce
JSON** (the default does).

### 2. The hub (host)

Derive from `UniversalHub<TContract>` (it is a `BleHub<TContract>`). Its lifecycle hooks get a `HubConnection`:

```csharp
using Shiny.UniversalHubs;

public class GameHub(GameEngine engine) : UniversalHub<IGameHub>
{
    public override Task OnConnectedAsync(HubConnection connection)
    {
        // connection.Id: the peripheral UUID over BLE, sb:{lineId} over Wi-Fi - same id as Context.ConnectionId
        // connection.Transport: HubTransports.Ble or HubTransports.Http
        // connection.Client: Name, AppVersion, Properties (from the handshake)
        // connection.Properties: your own per-connection state bag, same on both transports
        // connection.Ble: the IPeripheral (null over HTTP); connection.Http: the Switchboard CallerContext (null over BLE)
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(HubConnection connection, HubDisconnect disconnect)
    {
        engine.Leave(connection.Id);
        return this.Clients.All.StateChanged(engine.Snapshot());
    }

    public async Task<MoveResult> MakeMove(int cell)
    {
        // this.Connection is the caller, with its transport
        await this.Clients.All.StateChanged(engine.Snapshot());
        return new MoveResult(true, null);
    }
}
```

- Override the `HubConnection` overloads. The parameterless `OnConnectedAsync()` and `OnDisconnectedAsync(HubDisconnect)`
  are sealed on `UniversalHub<T>`.
- `connection.Http` is null in `OnDisconnectedAsync`, because the line's requests are over.
- `HubConnection.GetTransport(id)` maps any connection id to its transport.
- A plain `BleHub<T>` still works over both transports, but can't tell them apart.

### 3. Host registration

One call, once. It registers BLE hosting (blehubs' `AddBleHubServer`) and the HttpServer + Switchboard + mDNS:

```csharp
builder.Services.AddUniversalServer(server => server
    .ServiceUuid(ServiceUuid)                                              // the one BLE service - clients use the same
    .Ble(o =>                                                              // BleHubHostOptions
    {
        o.LocalName = "TTT";                                               // also the mDNS instance name; keep short and distinct
        o.EnableFileTransfers(filesDir, ft => ft.MaxUploadSize = 512 * 1024);   // L2CAP and HTTP both use it
    })
    .Protocol(o => o.RequestTimeout = TimeSpan.FromSeconds(10))           // optional, blehubs limits
    .Http(o =>                                                             // UniversalHubHostOptions
    {
        o.AuthorizeFile = req => req.FileName.StartsWith("avatar-");      // Wi-Fi file gate (L2CAP's Authorize doesn't apply)
        o.HostId = settings.InstallId;                                    // persist for a stable id across restarts
    })
    .HttpServer(http => http.AddQuickTunnel())                             // optional - the HttpServer itself (port, TLS, tunnels)
    .AddHub<GameHub>(GameCharacteristicUuid, "_mygame._tcp", o =>
    {
        o.MaxClients = 6;                                                  // counted across both transports
        o.ValidateClient = info => String.IsNullOrWhiteSpace(info.Name) ? "A name is required" : null;
    })
    .AddHub<ChatHub>(ChatCharacteristicUuid, "_mychat._tcp", o => o.AllowTunnel = true)
);
```

- **Each hub** has a full 128-bit **characteristic UUID** (also its HTTP path) and an **mDNS service type**
  (`_name._tcp`, the name 1-15 letters, digits or `-`; anything else throws at registration). Clients use the same
  two. Hubs sharing a type share one advertisement.
- **One BLE service UUID** for every hub - a BLE scan can't tell which hubs run, mDNS can.
- **`server.Endpoint`** is the HTTP base path (`/hubs`). Hubs at `{Endpoint}/{characteristicUuid}`, files at
  `{Endpoint}/files`. mDNS advertises it.
- The HttpServer binds every interface on a free port (mDNS publishes it), unless it was already registered with
  `AddShinyHttpServer`.
- **Call `AddUniversalServer` once**, with at least one hub. A second call, no hubs or the same hub twice throws. Don't
  also call `AddBleHubServer` for the same hubs.
- It doesn't start anything - `IUniversalHubHost` does.

Lower-level equivalent (each hub must be in both):

```csharp
builder.Services.AddBleHubServer(ble => ble.AddHub<GameHub>(GameCharacteristicUuid));
builder.Services.AddShinyHttpServer(http =>
{
    http.Options.Address = IPAddress.Any;
    http.Options.Port = 0;
    http.AddUniversalHubs().AddHub<GameHub>("_mygame._tcp", allowTunnel: false);
}, autoStart: false);
```

### 4. Starting and stopping

```csharp
public class Lobby(IUniversalHubHost host)
{
    public Task<HubTransports> OpenAll() => host.Start(HubTransports.All);   // returns what actually started
    public Task CloseAll() => host.Stop("The host ended the game");          // clients get ServerShutdown

    public Task<HubTransports> OpenGame() => host.StartHub<GameHub>();      // one hub, both transports
    public Task CloseGame() => host.StopHub<GameHub>("Game over");          // the other hubs carry on
    public HubTransports ChatOn => host.GetRunningTransports<ChatHub>();
}
```

- A transport that can't start (Bluetooth off, no network) is skipped if another starts; `Start` throws when none could.
- The HTTP server starts with the first hub hosted over HTTP and stops with the last. mDNS lists only running hubs.
- `RunningTransports`, `ListenUrls`, and `FileTransferred` (HTTP files; L2CAP raises `IBleHubHost.FileTransferred`).
- `IHubContext<THub>` (pushes, `Disconnect`, `ConnectedClients`, groups) covers both transports' clients.

### 5. Client

```csharp
builder.Services.AddUniversalHubClient<IGameHub>(GameCharacteristicUuid, "_mygame._tcp", o => o.ServiceUuid = ServiceUuid);

public class JoinViewModel(IUniversalHubClient<IGameHub> client)
{
    IDisposable? scan;

    public void StartScan() => this.scan = client.DiscoverAll().Subscribe(host =>
    {
        // one HubHostInfo per host, re-raised when either side changes
        // host.Key (stable), host.Name, host.HostId, host.Rssi, host.Transports (None = gone)
    });

    public async Task Join(HubHostInfo host)
    {
        this.scan?.Dispose();
        client.Hub.StateChanged += state => mainThread.BeginInvokeOnMainThread(() => this.Apply(state));
        client.Disconnected += (_, disconnect) => { /* switch on disconnect.Reason */ };

        await client.Connect(host, new HubConnectOptions("Allan") { Prefer = HubTransports.Http });
        var transport = client.Transport;   // Http or Ble
        await client.Hub.MakeMove(4);
    }
}
```

- `IUniversalHubClient<T>` **is** an `IBleHubClient<T>`: `Hub`, `Status`, `Disconnected`, `StatusChanged`, `Rename`,
  `UploadFile` / `DownloadFile` all work on either transport. `Discover()` / `Connect(BleHubHostInfo)` are BLE only.
- **`Prefer`** (Http by default) is tried first when the host is reachable both ways; the other is the fallback.
  **`Allowed`** limits which may be used. A **refusal** (full, `ValidateClient`, hub not running) is **not retried**
  on the other transport. The transport never changes mid-session.
- **Resume**: a short Wi-Fi drop keeps the connection id and groups and replays missed pushes; neither side sees a
  disconnect. `UniversalHubClientOptions.AutomaticReconnect = false` turns it off.
- **Discovery merges by host name** (Apple can't advertise a host id over BLE), so hosts need distinct `LocalName`s.
  A transport that can't scan is skipped.
- **By address** (tunnel URL, typed address, QR code), HTTP only:
  `await client.Connect(HubHostInfo.FromUrl(new Uri("https://abc.a.pinggy.link")));` - pass `endpoint:` if the host
  changed `/hubs`.
- Hub events arrive on a background thread - marshal to the UI thread.

### 6. Tunnels

A tunnel exposes the whole HttpServer, so **each hub opts in** with `AllowTunnel = true`. Through a tunnel, other hubs
answer 404 (their LAN and BLE clients are unaffected). File routes are open through a tunnel when any hub allows it,
still gated by the per-client file token. Put authentication in front of anything shared on the internet.

### 7. Disconnect reasons

The same `HubDisconnectReason` on both transports, host and client:

| Reason | BLE | HTTP |
|---|---|---|
| `ClientDisconnect` | client called `Disconnect()` | client closed its line |
| `ClientTimeout` | unsubscribe without goodbye, or the sweep | line dropped and the resume window ran out |
| `ServerDisconnect` | `Context.Abort` / `IHubContext.Disconnect` | same |
| `ServerShutdown` | `IUniversalHubHost.Stop` / `StopHub` / `IHubContext.Stop` | same |
| `ConnectionFailed` | link failed | line faulted |

### 8. Platform setup

iOS `Info.plist` (host and client):

```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>Bluetooth is used to talk to nearby devices</string>
<key>NSLocalNetworkUsageDescription</key>
<string>The local network is used to talk to nearby devices</string>
<key>NSBonjourServices</key>
<array>
    <string>_mygame._tcp</string>   <!-- every hub's mDNS service type -->
</array>
<key>NSAppTransportSecurity</key>   <!-- clients: plain HTTP on the LAN -->
<dict>
    <key>NSAllowsLocalNetworking</key>
    <true/>
</dict>
```

Android `AndroidManifest.xml`: the BLE permissions from `shiny-ble-hubs`, plus `INTERNET`, `ACCESS_NETWORK_STATE`,
`ACCESS_WIFI_STATE`, `CHANGE_WIFI_MULTICAST_STATE`, and `android:usesCleartextTraffic="true"` on `<application>` for
clients.

### 9. Media channels (live audio and video)

blehubs' media channel, an ordered, two-way packet pipe beside a hub connection, works over Wi-Fi too. Over BLE it's
L2CAP; over Wi-Fi this library carries it as a WebSocket at `{BasePath}/media`. Use it for calls and other live media.
**Don't push media through hub methods**: they're far too slow and chatty, especially over BLE.

```csharp
// host
builder.Services.AddUniversalServer(server => server
    .Ble(o => o.EnableMediaChannels())                 // off by default; maxPacketSize defaults to 128 KB
    .AddHub<ChatHub>(ChatUuid, "_mychat._tcp"));

hubContext.MediaChannelOpened += (_, e) => _ = Serve(e.Client.Id, e.Channel);   // IHubContext<ChatHub>
var channel = hubContext.GetMediaChannel(connectionId);

// client (IUniversalHubClient<IChatHub>), on either transport
if (client.CanOpenMediaChannel)
{
    var channel = await client.OpenMediaChannel();     // returns the open one if there is one
    await channel.Send(packet);                        // whole, in order; > MaxPacketSize throws ArgumentException
    await foreach (var p in channel.Receive()) { }     // one reader; ends when the channel closes
}
```

- The channel closes when the client disconnects, the hub stops or either side disposes it. `Completion` completes then.
- Pace it yourself: for video, send latest-wins (drop stale frames) and keep audio ahead. BLE carries a few KB/s of
  hub calls but much more over L2CAP; still, size video for BLE (about 160 px at 3 fps) and use compressed audio.
- Building a call: capture and play voice with Shiny.Audio's `IAudioDuplex` (`shiny-speech` skill, echo cancelled),
  film with the Shiny Controls `CameraView` + `VideoEncoder` and show the other side with `RemoteVideoView`
  (`shiny-controls` skill). The repo's `samples/Sample/Calls` is a complete FaceTime-style example: signalling, host
  relay between clients, H.264/JPEG negotiation, ADPCM over BLE.
- Permissions for calls: iOS/Mac `NSMicrophoneUsageDescription`, `NSCameraUsageDescription`; Android `RECORD_AUDIO`,
  `CAMERA`, `MODIFY_AUDIO_SETTINGS`.

## Best Practices

1. **Set your own BLE `ServiceUuid` and mDNS service types**, so other apps' hosts stay out of your scans.
2. **Give hosts short, distinct names** (`LocalName`). Discovery merges BLE and mDNS by name.
3. **Persist `HostId`** if clients should recognize a host across restarts.
4. **Keep BLE as the fallback.** Plane, hotel and public Wi-Fi usually isolate clients, blocking mDNS and
   device-to-device traffic.
5. **Register a BLE hosting stack even for Wi-Fi-only hosting**; Wi-Fi hosting still works with Bluetooth off.
6. **About six hubs** fit in one mDNS TXT record per host.
7. **Never store state on the hub instance** - it is recreated per call. Use singletons or `connection.Properties`.
8. **Test on two physical devices**, both directions, with Wi-Fi on and off.
9. **Live media over Bluetooth is voice-first.** Expect a small, slow picture; video is much better on Wi-Fi.

## Reference Files

- `reference/api-reference.md` - full public API surface
