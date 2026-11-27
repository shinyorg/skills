---
name: shiny-universal-hubs
description: Generate code using Shiny.UniversalHubs - SignalR style hubs between nearby devices over any transport (Bluetooth LE, HTTP/WebSocket with mDNS and tunnels, Apple MultipeerConnectivity, Google Nearby Connections, or your own), with a source generated client proxy, typed pushes, groups, streaming, per hub transports, serializers and BLE services, transport priority and runtime enable/disable, resume, file transfers, media channels (live audio/video packet pipes) and dynamic hubs
auto_invoke: true
triggers:
  - universal hubs
  - universalhubs
  - Shiny.UniversalHubs
  - Shiny.UniversalHubs.BluetoothLE
  - Shiny.UniversalHubs.Http
  - Shiny.UniversalHubs.Multipeer
  - Shiny.UniversalHubs.Nearby
  - hubs over transports
  - hub over ble
  - hub over wifi
  - signalr over ble
  - signalr between devices
  - device to device hub
  - local multiplayer
  - nearby multiplayer
  - offline hub
  - hub without network
  - UniversalHub
  - UniversalHubContract
  - UniversalHubContractAttribute
  - IHubContext
  - HubConnection
  - HubCallerContext
  - HubConnectedClient
  - AddUniversalServer
  - UniversalServerBuilder
  - UniversalHubBuilder
  - UniversalServerOptions
  - AddHub
  - UseBluetoothLE
  - UseHttp
  - UseMultipeer
  - UseNearby
  - UseTransport
  - AddTransport
  - IUniversalHubHost
  - StartHub
  - StopHub
  - RunningTransports
  - GetRunningTransports
  - RunningChanged
  - IHubTransport
  - IHubBinding
  - IHubEndpoint
  - HubStreamInvocation
  - IHubPeerChannel
  - IHubClientTransport
  - HubClientContext
  - HubHostSighting
  - HubEndpoint
  - IDynamicHubTransport
  - BleHostTransport
  - HttpHostTransport
  - MultipeerHostTransport
  - NearbyHostTransport
  - BleClientTransport
  - HttpClientTransport
  - MultipeerClientTransport
  - NearbyClientTransport
  - BleHubEndpoint
  - HttpHubEndpoint
  - MultipeerHubEndpoint
  - NearbyHubEndpoint
  - HttpHubHost
  - FromUrl
  - AddUniversalHubClient
  - IUniversalHubClient
  - IHubProxy
  - IHubConnection
  - UniversalHubClientOptions
  - Priority
  - HubConnectOptions
  - Prefer
  - Allow
  - Only
  - Solo
  - SetEnabled
  - DiscoverAll
  - HubHostInfo
  - scanFor
  - advertise
  - 16-bit uuid
  - ble advertisement overflow
  - IHubSerializer
  - JsonHubSerializer
  - serializer mismatch
  - HubException
  - HubRemoteException
  - HubDisconnectedException
  - HubDisconnect
  - HubDisconnectReason
  - HubClientStatus
  - Reconnecting
  - ResumeWindow
  - AutomaticReconnect
  - EnableFileTransfers
  - AuthorizeFile
  - UploadFile
  - DownloadFile
  - HubFileTransferResult
  - HubTransferProgress
  - EnableMediaChannels
  - OpenMediaChannel
  - IHubMediaChannel
  - MediaChannelOpened
  - BluetoothLETransportOptions
  - HttpTransportOptions
  - HttpClientTransportOptions
  - AllowTunnel
  - AddQuickTunnel
  - NSBonjourServices
  - mdns service type
  - multipeer
  - multipeerconnectivity
  - nearby connections
  - NearbyProtocol
  - AllowDynamicHubs
  - IDynamicHubs
  - DynamicHubDefinition
  - IDynamicHubHandler
  - AddDynamicHubClients
  - IDynamicHubClients
  - SUH001
  - SUH002
  - SUH003
  - SUH004
  - SUH005
  - SUH006
  - SUH007
  - SUH008
  - SUH009
  - MaxMessageSize
---

# Shiny.UniversalHubs

SignalR-style hubs between nearby devices, over any transport. A hub is written once and served over the transports it
picks; every client shares its groups and pushes whichever way it connected. Read `reference/api-reference.md` for the
full API.

## When to use

- Real-time messaging between devices that are near each other or on one network: local multiplayer games, chat,
  classroom / meeting tools, companion apps, device control - with or without a network.
- The user mentions Shiny.UniversalHubs, any of its packages, hubs over BLE / Wi-Fi / Multipeer / Nearby, or one of the
  triggers above.
- **Shiny.BluetoothLE.Hubs is gone** - it is `Shiny.UniversalHubs` + `Shiny.UniversalHubs.BluetoothLE`. Translate old code
  with the upgrade table at the end.

## Packages

| Package | Adds |
|---|---|
| `Shiny.UniversalHubs` | hubs, host, client, transport seams, protocols, source generator (net11.0) |
| `Shiny.UniversalHubs.BluetoothLE` | `UseBluetoothLE` - GATT characteristics, L2CAP files and media; registers platform BLE stacks |
| `Shiny.UniversalHubs.Http` | `UseHttp` - a WebSocket per hub connection on Shiny.Net.HttpServer, mDNS, tunnels |
| `Shiny.UniversalHubs.Multipeer` | `UseMultipeer` - Apple only (iOS, Mac Catalyst, macOS) |
| `Shiny.UniversalHubs.Nearby` | `UseNearby` - Android, iOS, macOS |

Projects that declare contracts or hubs get the generator from the `Shiny.UniversalHubs` package.

## Contract and hub

```csharp
[UniversalHubContract]
public interface IGameHub
{
    Task<bool> Move(int cell);                                   // Task or Task<T>, optional trailing CancellationToken
    IAsyncEnumerable<Score> Scores(CancellationToken ct);        // streaming
    event Action<GameState> StateChanged;                        // host -> client push (Action / Action<...>)
}

public class GameHub(IGameStore store) : UniversalHub<IGameHub>   // new instance + DI scope per call
{
    public async Task<bool> Move(int cell)
    {
        await this.Clients.All.StateChanged(await store.Apply(this.Context.ConnectionId, cell));
        return true;
    }

    public async IAsyncEnumerable<Score> Scores([EnumeratorCancellation] CancellationToken ct) { yield break; }

    public override Task OnConnectedAsync(HubConnection connection) => Task.CompletedTask;
    public override Task OnDisconnectedAsync(HubConnection connection, HubDisconnect disconnect) => Task.CompletedTask;
}
```

- `this.Context` (`HubCallerContext`: `ConnectionId`, `Client`, `Items`, `ConnectionAborted`, `Abort()`), `this.Clients`
  (`All`, `Others`, `Caller`, `Client(id)`, `Group(name)`, ...), `this.Groups`, `this.Connection`.
- Keep state in singletons or `Context.Items` - hub instances don't live between calls.
- Outside a hub: inject `IHubContext<GameHub>` (`Clients`, `Groups`, `ConnectedClients`, `Disconnect`, `Start`, `Stop`,
  `ClientConnected/Disconnected`, `MediaChannelOpened`).
- Method and event names travel on the wire - renaming one breaks deployed clients.

## Host

```csharp
services.AddUniversalServer(server => server
    .Configure(o => { o.LocalName = "Host"; o.EnableFileTransfers(dir); o.EnableMediaChannels(); })
    .Multipeer("mygame-mc")
    .Nearby("com.mycompany.mygame")
    .AddHub<GameHub>("game", hub => hub
        .MaxClients(6)
        .ValidateClient(info => info.Name == null ? "Name required" : null)
        .UseHttp("_mygame._tcp")
        .UseMultipeer()
        .UseNearby()
        .UseBluetoothLE("180D", "6e400002-b5a3-f393-e0a9-e50e24dcca9e")));

await host.Start();                                   // IUniversalHubHost: every hub on its transports
await host.Stop<BleHostTransport>();                  // one transport off
await host.StartHub<GameHub, HttpHostTransport>();
```

Rules:
- `AddUniversalServer` once per app; every hub inside it. Hub names: 1-64 of letters, digits, `.`, `_`, `-`; unique.
- A hub needs at least one transport; a transport set up must be used by a hub (unless dynamic hubs are allowed).
- A transport that can't start is skipped while another starts; one the platform lacks reports `IsSupported = false`.
- In a hub: `connection.Transport is BleHostTransport`, `connection.Peripheral()`, `connection.HttpContext()`,
  `connection.MultipeerPeer()`, `connection.NearbyPeer()`.

### BLE specifics

- Each hub is a characteristic in a service it picks; hubs may share one.
- `UseBluetoothLE(service, characteristic, advertise: true)`. Every advertised service (running or not) must fit one
  31-byte advertisement - registration throws otherwise. One 128-bit UUID fits; two don't. Prefer 16/32-bit UUIDs
  (`"180D"`, `"ABCD1234"`), a shared service, or `advertise: false`.
- A stopped hub's service leaves the advertisement unless another running hub shares it.
- Unadvertised hubs: the client sets `scanFor:` (another advertised service of the host) or uses another transport.
- `server.BluetoothLE(o => ...)`: `ClientSweepInterval`, `SecureFileTransfers`, `SecureMediaChannels`, `FileTransfer`,
  `DynamicServiceUuid`. On plain .NET / macOS AppKit register `IBleManager` / `IBleHostingManager` yourself.

### HTTP specifics

- `UseHttp(mdnsServiceType, allowTunnel)`; `server.Http(o => BasePath / Advertise / ServiceType / ManageHttpServer /
  DynamicServiceTypes)`; `server.HttpServer(http => ...)` for the server itself (e.g. `AddQuickTunnel()`).
- WebSocket at `{BasePath}/{hub name}`; the peer-link session inside carries calls, pushes, resume, files and media.
- Through a tunnel, a hub without `allowTunnel` is 404. Authenticate with `connection.HttpContext()?.User`.
- Clients reach a known address with `HttpHubHost.FromUrl(uri)`.

### Custom transports

Implement `IHubTransport` (`DisplayName`, `IsSupported`, `IsRunning`, `Start`, `Stop`, optional `HostRenamed`) and add it
with `hub.UseTransport<MyTransport>(settings)` (read back via `hub.GetSettings<T>()`); DI constructs it. It talks to each
hub through `IHubBinding.Endpoint` (`IHubEndpoint`) with an `IHubPeerChannel` per client (implement `Transport` and
`GetFeature` so `HubConnection` can describe the client). Client side: `IHubClientTransport` (`Discover` returns
`HubHostSighting`s with your `HubEndpoint` subclass; `Connect` calls `HubClientContext.Connect(events => link)` with an
`IHubConnectionTransport`), added with `o.UseTransport(sp => new MyClientTransport(...))`.

## Client

```csharp
services.AddUniversalHubClient<IGameHub>("game", o => o
    .UseHttp("_mygame._tcp")
    .UseMultipeer("mygame-mc")
    .UseNearby("com.mycompany.mygame")
    .UseBluetoothLE("180D", "6e400002-b5a3-f393-e0a9-e50e24dcca9e")
    .Priority<NearbyClientTransport, HttpClientTransport>());      // optional; default = Use… order

using var scan = client.DiscoverAll().Subscribe(host => { /* host.Transports, host.Endpoint<BleHubEndpoint>()?.Rssi */ });
await client.Connect(host, new HubConnectOptions("Allan").Prefer<BleClientTransport>());
await client.Hub.Move(4);
await client.Disable<BleClientTransport>();       // Enable / Solo / IsEnabled / EnabledChanged
```

- Connect tries the host's transports in priority order; refusals (full, ValidateClient, serializer mismatch) are not
  retried. `Allow<T>()` restricts, `Only<T>()` = just that one.
- `client.Transport` is the transport in use; `Status`, `StatusChanged`, `Disconnected`, `Rename`, `UploadFile`,
  `DownloadFile`, `OpenMediaChannel` work on every transport.
- HTTP client options: `UseHttp(type, h => { h.ConnectTimeout; h.RediscoverTimeout; h.MessageHandlerFactory; h.MaxMessageSize; })`.

## Serializers

`IHubSerializer` (`Name`, `Serialize<T>`, `Deserialize<T>`); default `JsonHubSerializer` ("json", Shiny's AOT JSON - add
your `JsonSerializerContext` with `Json.AddContext(...)`). Per hub: `hub.Serializer(x)`; per client: `o.Serializer(x)`.
Names must match or the handshake refuses. Dynamic hubs are JSON.

## Resume, disconnects, files, media

- Dropped links resume as the same connection within `UniversalServerOptions.ResumeWindow` (30 s), missed pushes
  replayed (`ResumeBufferSize` 128). Client status `Reconnecting` meanwhile; `AutomaticReconnect` on the client options.
- `HubDisconnect(Reason, Message)`: `ClientDisconnect`, `ClientTimeout`, `ServerDisconnect`, `ServerShutdown`,
  `ConnectionFailed`.
- Files: `o.EnableFileTransfers(dir, ft => ...)` + `o.AuthorizeFile` (all transports); client `UploadFile`,
  `UploadStream`, `DownloadFile` -> `HubFileTransferResult`, progress `HubTransferProgress`.
- Media channels: `o.EnableMediaChannels(maxPacketSize)`; `client.OpenMediaChannel()` -> `IHubMediaChannel`
  (`Send`, `Receive`, `Completion`); host `IHubContext.MediaChannelOpened`.

## Dynamic hubs

```csharp
server.AllowDynamicHubs(o => o.MaxHubs = 6).Http(o => o.DynamicServiceTypes.Add("_mygame._tcp"));
var hub = dynamicHubs.Add(new DynamicHubDefinition("trivia").UseHttp("_mygame._tcp").UseBluetoothLE(characteristic), handler);
await hub.Start();
// clients without a contract
services.AddDynamicHubClients(o => o.UseHttp());
var client = dynamicClients.Create("trivia", o => o.UseHttp("_mygame._tcp"));
await client.Hub.Invoke("Answer", [json]);
```

## Generator diagnostics

| Id | Meaning |
|---|---|
| SUH001 | The hub doesn't implement a contract method |
| SUH002 | Unsupported hub method return type (use Task, Task<T>, IAsyncEnumerable<T>) |
| SUH003 | Hub methods can't be overloaded |
| SUH004 | Unsupported hub event (use Action / Action<...>) |
| SUH005 | The hub's contract isn't marked [UniversalHubContract] |
| SUH006 | Unsupported hub method signature |
| SUH007 | A contract declares a property or indexer (only methods and Action events) |
| SUH008 | Hub contracts can't be generic |
| SUH009 | A contract member name collides with the generated proxy (Hub, OnPush, ...) or is both a method and an event |

## Platform setup

- iOS / macOS Info.plist: `NSBluetoothAlwaysUsageDescription`, `NSLocalNetworkUsageDescription`, `NSBonjourServices` with
  each hub's mDNS type, `_{mc}._tcp` + `_{mc}._udp` for Multipeer, `NearbyProtocol.BonjourServices(serviceId)` for
  Nearby; clients add `NSAllowsLocalNetworking`.
- Android: BLE permissions, `INTERNET`, `ACCESS_NETWORK_STATE`, `ACCESS_WIFI_STATE`, `CHANGE_WIFI_MULTICAST_STATE`,
  `usesCleartextTraffic`; Nearby adds its own and needs Google Play services.

## Upgrading from Shiny.BluetoothLE.Hubs / older UniversalHubs

| Old | New |
|---|---|
| `Shiny.BluetoothLE.Hubs(.Host/.Client)`, `Shiny.UniversalHubs.Host/.Client` | `Shiny.UniversalHubs` + transport packages |
| `using Shiny.BluetoothLE.Hubs;` | `using Shiny.UniversalHubs;` |
| `[BleHubClient]`, `BleHub<T>` | `[UniversalHubContract]`, `UniversalHub<T>` |
| `AddBleHubServer`, `AddHub<T>(characteristic, mdnsType, o => ...)` | `AddUniversalServer(s => s.AddHub<T>("name", h => h.UseBluetoothLE(svc, chr).UseHttp(type)))` |
| `AddBleHubClient<T>`, `AddUniversalHubClient<T>(characteristic, mdnsType, ...)` | `AddUniversalHubClient<T>("name", o => o.UseBluetoothLE(svc, chr).UseHttp(type))` |
| `IBleHubClient<T>`, `IBleHubConnection`, `IBleHubSerializer`, `BleHubException`, `BleHubRemoteException`, `IBleHubMediaChannel`, `BleHubClientStatus` | `IUniversalHubClient<T>`, `IHubConnection`, `IHubSerializer`, `HubException`, `HubRemoteException`, `IHubMediaChannel`, `HubClientStatus` |
| `HubTransports` flags, `Prefer`/`Allowed` | transport types: `Start<T>()`, `Prefer<T>()`, `Allow<T>()`, `Only<T>()` |
| `HubHostInfo.Ble/.Http/.Multipeer/.Nearby`, `.Rssi`, `HubHostInfo.FromUrl` | `Endpoint<BleHubEndpoint>()`, `IsSeenOver<T>()`, `HttpHubHost.FromUrl` |
| `connection.Ble/.Http/.Multipeer/.Nearby` | `connection.Peripheral()`, `.HttpContext()`, `.MultipeerPeer()`, `.NearbyPeer()` |
| one BLE service UUID for all hubs | per-hub service, `advertise:`, 16/32-bit UUIDs |
| `DynamicHubDefinition(name, characteristic, serviceType)` | `new DynamicHubDefinition(name).UseHttp(type).UseBluetoothLE(chr)` |
| `SBH0xx` | `SUH0xx` |
