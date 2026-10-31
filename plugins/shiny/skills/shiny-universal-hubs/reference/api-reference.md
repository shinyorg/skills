# Shiny.UniversalHubs API Reference

Hub contracts, `BleHub<T>`, `IHubContext<THub>`, `IBleHubClient<T>`, `HubDisconnect` and the file transfer types come
from Shiny.BluetoothLE.Hubs - see the `shiny-ble-hubs` skill's API reference.

## Shared (`Shiny.UniversalHubs`)

```csharp
[Flags]
public enum HubTransports
{
    None = 0,
    Ble = 1,
    Http = 2,
    Multipeer = 4,      // Shiny.UniversalHubs.Multipeer - does nothing until that package is set up
    Nearby = 8,         // Shiny.UniversalHubs.Nearby - likewise
    All = Ble | Http | Multipeer | Nearby
}

public static class UniversalHubProtocol
{
    public const int Version = 2;                           // the handshake refuses other versions
    public const string ServiceType = "_shinyhub._tcp";    // lower-level default mDNS type
    public const string DefaultBasePath = "/hubs";
    public const string HandshakeMethod = "hub.handshake";
    public const string InvokeMethod = "hub.invoke";
    public const string StreamMethod = "hub.stream";
    public const string PushMethod = "hub.push";
    public const string FileTokenHeader = "X-Hub-File-Token";
    public const string MediaTokenHeader = "X-Hub-Media-Token";   // media channel WebSocket upgrade
    public const string TxtProtocol = "proto", TxtHostId = "hostId", TxtName = "name", TxtPath = "path", TxtHubs = "hubs";

    public static string HubPath(string basePath, string characteristicUuid);   // {basePath}/{characteristicUuid}
    public static string FilesPath(string basePath);                            // {basePath}/files
    public static string MediaPath(string basePath);                            // {basePath}/media (WebSocket)
    public static void ValidateServiceType(string serviceType);                 // throws unless _name._tcp (name: 1-15 letters, digits or -)
}
```

## Host (`Shiny.UniversalHubs.Host`)

```csharp
public static class UniversalServerExtensions
{
    // once, with at least one hub (or AllowDynamicHubs); registers AddBleHubServer + AddShinyHttpServer(autoStart: false)
    public static IServiceCollection AddUniversalServer(this IServiceCollection services, Action<UniversalServerBuilder> configure);
}

public sealed class UniversalServerBuilder
{
    public string Endpoint { get; set; }                                                    // "/hubs"
    public UniversalServerBuilder ServiceUuid(string serviceUuid);                          // the one BLE service
    public UniversalServerBuilder Ble(Action<BleHubHostOptions> configure);                 // LocalName, EnableFileTransfers, sweep
    public UniversalServerBuilder Protocol(Action<BleHubProtocolOptions> configure);        // blehubs limits
    public UniversalServerBuilder Http(Action<UniversalHubHostOptions> configure);
    public UniversalServerBuilder HttpServer(Action<ShinyHttpServerBuilder> configure);     // port, TLS, middleware, tunnels
    public UniversalServerBuilder AddHub<THub>(string characteristicUuid, string mdnsServiceType, Action<UniversalHubOptions>? configure = null) where THub : class;
    public UniversalServerBuilder AllowDynamicHubs(Action<DynamicHubOptions>? configure = null);   // IDynamicHubs; compiled hubs optional
}

public sealed class UniversalHubOptions : BleHubOptions   // MaxClients, ValidateClient (both transports)
{
    public bool AllowTunnel { get; set; }                 // false - tunnelled requests get 404
}

public class UniversalHubHostOptions
{
    public string BasePath { get; set; }                  // "/hubs" (set from Endpoint by AddUniversalServer)
    public bool Advertise { get; set; }                   // true - mDNS while Wi-Fi hosting runs
    public string ServiceType { get; set; }               // default type for lower-level AddHub without one
    public string HostId { get; set; }                    // new Guid each start - persist for stability
    public bool ManageHttpServer { get; set; }            // true - start/stop the HttpServer with hosting
    public Func<UniversalHubFileRequest, bool>? AuthorizeFile { get; set; }   // false = 403
}

public sealed record UniversalHubFileRequest(L2CapTransferType Type, string FileName, long? Size, BleHubConnectedClient Client);

// lower level
public static class UniversalHubHostExtensions
{
    public static UniversalHubHostBuilder AddUniversalHubs(this ShinyHttpServerBuilder builder, Action<UniversalHubHostOptions>? configure = null);
}
public sealed class UniversalHubHostBuilder
{
    // the hub must also be added to AddBleHubServer
    public UniversalHubHostBuilder AddHub<THub>(string? mdnsServiceType = null, bool allowTunnel = false) where THub : class;
}

public interface IUniversalHubHost
{
    HubTransports RunningTransports { get; }
    IReadOnlyList<string> ListenUrls { get; }
    Task<HubTransports> Start(HubTransports transports = HubTransports.All, CancellationToken cancellationToken = default);
    Task Stop(string? reason = null);                                     // ServerShutdown
    Task<HubTransports> StartHub<THub>(HubTransports transports = HubTransports.All, CancellationToken cancellationToken = default) where THub : class;
    Task StopHub<THub>(string? reason = null) where THub : class;
    HubTransports GetRunningTransports<THub>() where THub : class;
    event EventHandler<BleHubFileTransferredEventArgs>? FileTransferred;  // HTTP files only
}

public abstract class UniversalHub<TContract> : BleHub<TContract> where TContract : class
{
    public HubConnection Connection { get; }                              // the caller
    public virtual Task OnConnectedAsync(HubConnection connection);
    public virtual Task OnDisconnectedAsync(HubConnection connection, HubDisconnect disconnect);
    // OnConnectedAsync() / OnDisconnectedAsync(HubDisconnect) / OnDisconnectedAsync(string?) are sealed
}

public sealed record HubConnection(string Id, HubTransports Transport, BleHubConnectedClient Client)
{
    public IDictionary<string, object> Properties { get; }               // per-connection state (Client.Items)
    public Shiny.BluetoothLE.Hosting.IPeripheral? Ble { get; }           // null over HTTP
    public CallerContext? Http { get; }                                   // null over BLE and in OnDisconnectedAsync
    public MultipeerPeer? Multipeer { get; }                              // only over Multipeer
    public NearbyPeer? Nearby { get; }                                    // only over Nearby
    public static HubTransports GetTransport(string connectionId);       // sb: = Http, mc: = Multipeer, nb: = Nearby, else Ble
}
```

### Dynamic hubs (defined while the app runs)

Values are the bytes the caller's `IBleHubSerializer` wrote (JSON by default). Served over BLE, Wi-Fi and Multipeer at
`{BasePath}/{characteristicUuid}` like a compiled hub; no wire change.

```csharp
public sealed class DynamicHubOptions
{
    public IList<string> ServiceTypes { get; }            // empty = only UniversalHubHostOptions.ServiceType
    public int MaxHubs { get; set; }                      // 6 - what an mDNS TXT record fits
}

public sealed record DynamicHubDefinition(string Name, string CharacteristicUuid, string ServiceType)
{
    public int? MaxClients { get; init; }                 // 8, across every transport; null = no limit
    public bool AllowTunnel { get; init; }
}

public interface IDynamicHubs
{
    IReadOnlyList<IDynamicHub> Hubs { get; }
    IDynamicHub? Find(string name);
    IDynamicHub Add(DynamicHubDefinition definition, IDynamicHubHandler handler);   // throws: name/characteristic taken, type not allowed, MaxHubs
    Task Remove(string name, string? reason = null);      // stops it everywhere, clients get ServerShutdown
}

public interface IDynamicHub
{
    DynamicHubDefinition Definition { get; }
    HubTransports RunningTransports { get; }
    Task<HubTransports> Start(HubTransports transports = HubTransports.All, CancellationToken cancellationToken = default);
    Task Stop(string? reason = null);
    IReadOnlyList<HubConnection> Connections { get; }
    IGroupManager Groups { get; }
    IHubClients<DynamicHubPush> Clients { get; }
    Task Disconnect(string connectionId, string? reason = null);
}

public readonly struct DynamicHubPush
{
    public Task Send(string eventName, IReadOnlyList<ReadOnlyMemory<byte>> arguments, CancellationToken cancellationToken = default);
}

public enum DynamicHubMethodKind { NotFound, Invoke, Stream }

public interface IDynamicHubHandler
{
    DynamicHubMethodKind GetMethodKind(string method);
    Task<byte[]?> Invoke(DynamicHubCall call, CancellationToken cancellationToken);      // null = no result; throw to fail
    IAsyncEnumerable<byte[]> Stream(DynamicHubCall call, CancellationToken cancellationToken);
    Task OnConnectedAsync(HubConnection connection);                                    // throw to refuse
    Task OnDisconnectedAsync(HubConnection connection, HubDisconnect disconnect);
    Task OnRenamedAsync(HubConnection connection, string? previousName) => Task.CompletedTask;   // throw to refuse
}

public sealed class DynamicHubCall
{
    public HubConnection Connection { get; }
    public string Method { get; }
    public IReadOnlyList<ReadOnlyMemory<byte>> Arguments { get; }   // exactly as the caller serialized them
    public CancellationToken ConnectionAborted { get; }
    public void Abort(string? reason = null);                       // after this call's reply
}
```

## Client (`Shiny.UniversalHubs.Client`)

```csharp
public static class UniversalHubClientExtensions
{
    public static IServiceCollection AddUniversalHubClient<TContract>(this IServiceCollection services, string characteristicUuid, string mdnsServiceType, Action<UniversalHubClientOptions>? configure = null) where TContract : class;
    public static IServiceCollection AddUniversalHubClient<TContract>(this IServiceCollection services, string characteristicUuid, Action<UniversalHubClientOptions>? configure = null) where TContract : class;
}

public class UniversalHubClientOptions
{
    public string ServiceUuid { get; set; }                     // BleHubProtocolOptions.DefaultServiceUuid - match the host
    public string ServiceType { get; set; }                     // the mDNS type to browse - match the hub
    public TimeSpan HttpConnectTimeout { get; set; }            // 6s per address
    public TimeSpan HttpRediscoverTimeout { get; set; }         // 3s - re-resolve a moved host over mDNS when no address answers; zero = off
    public bool AutomaticReconnect { get; set; }                // true - resume short Wi-Fi drops
    public Func<HttpMessageHandler>? HttpMessageHandlerFactory { get; set; }
}

public interface IUniversalHubClient<out TContract> : IBleHubClient<TContract> where TContract : class
{
    HubTransports Transport { get; }                            // None while disconnected
    IObservable<HubHostInfo> DiscoverAll(HubTransports transports = HubTransports.All);
    Task Connect(HubHostInfo host, HubConnectOptions? options = null, CancellationToken cancellationToken = default);
}

public sealed record HubHostInfo(string Key, string Name, string? HostId, BleHubHostInfo? Ble, DiscoveredHttpServer? Http)
{
    public MultipeerHost? Multipeer { get; init; }              // set while seen over MultipeerConnectivity
    public NearbyHost? Nearby { get; init; }                    // set while seen over Nearby Connections
    public HubTransports Transports { get; }                    // None = the host has gone
    public int? Rssi { get; }
    public static HubHostInfo FromUrl(Uri serverUrl, string endpoint = "/hubs");   // HTTP only
}

public sealed record HubConnectOptions(string? Name = null, string? AppVersion = null, Dictionary<string, string>? Properties = null)
{
    public HubTransports Prefer { get; init; }                  // Http - tried first; then Http, Multipeer, Nearby, Ble
    public HubTransports Allowed { get; init; }                 // All
}

public sealed record MultipeerHost(string DisplayName, IReadOnlyDictionary<string, string> Info);   // Info: proto, hostId, name, hubs
public sealed record NearbyHost(string EndpointName);
```

### Dynamic hub clients (no compiled contract)

```csharp
public static class DynamicHubClientExtensions
{
    // once; configure runs again for each client (ServiceType is set per client); add-on ServiceActions run once
    public static IServiceCollection AddDynamicHubClients(this IServiceCollection services, Action<UniversalHubClientOptions>? configure = null);
}

public interface IDynamicHubClients
{
    IUniversalHubClient<IDynamicHubCalls> Create(string characteristicUuid, string mdnsServiceType);   // new, disconnected; dispose when done
}

public interface IDynamicHubCalls                                // works against compiled and dynamic hubs
{
    Task<ReadOnlyMemory<byte>> Invoke(string method, IReadOnlyList<ReadOnlyMemory<byte>> arguments, CancellationToken cancellationToken = default);   // empty = no result
    IAsyncEnumerable<ReadOnlyMemory<byte>> Stream(string method, IReadOnlyList<ReadOnlyMemory<byte>> arguments, CancellationToken cancellationToken = default);
    event Action<string, IReadOnlyList<ReadOnlyMemory<byte>>>? Pushed;   // name, serialized arguments; in order
}
```


## Media channels

The API is blehubs' (`IBleHubMediaChannel`, `BleHubHostOptions.EnableMediaChannels`, `IHubContext<THub>.MediaChannelOpened`
/ `GetMediaChannel`, `IBleHubConnection.CanOpenMediaChannel` / `OpenMediaChannel`) - see the `shiny-ble-hubs` skill.
Over Wi-Fi:

```csharp
public sealed record HubHandshakeReply(
    bool Accepted, string? Reason, string? HostName, string? HostId, string? FileToken,
    string? MediaToken = null,              // send as X-Hub-Media-Token on the WebSocket upgrade; null = no media channels
    int? MediaMaxPacketSize = null);
// GET {BasePath}/media  (WebSocket; one binary message = one packet)  401 token, 404 no media channels, 400 not an upgrade
```


## Multipeer (`Shiny.UniversalHubs.Multipeer`)

```csharp
public static class MultipeerExtensions
{
    // host - once per AddUniversalServer
    public static UniversalServerBuilder Multipeer(this UniversalServerBuilder server, string serviceType, Action<MultipeerHostOptions>? configure = null);
    // client - inside AddUniversalHubClient's configure, once per contract
    public static UniversalHubClientOptions Multipeer(this UniversalHubClientOptions options, string serviceType, Action<MultipeerClientOptions>? configure = null);
}

public sealed class MultipeerHostOptions
{
    public string ServiceType { get; }
    public string? DisplayName { get; set; }        // default: BleHubHostOptions.LocalName; at most 63 UTF-8 bytes
}

public sealed class MultipeerClientOptions
{
    public string ServiceType { get; }
    public TimeSpan InviteTimeout { get; set; }     // 10s, then the next transport is tried
    public string? DisplayName { get; set; }        // this device's radio name - the host sees it as MultipeerPeer.DisplayName; "Hub client"
}

// on the host (Shiny.UniversalHubs.Host): HubConnection.Multipeer
public sealed record MultipeerPeer(string DisplayName);

public static class MultipeerProtocol
{
    public static void ValidateServiceType(string serviceType);         // 1-15 of a-z 0-9 -, a letter, no leading/trailing/double -
    public static IReadOnlyList<string> BonjourServices(string serviceType);   // ["_{type}._tcp", "_{type}._udp"] for NSBonjourServices
}
// connection ids: "mc:{guid}" - HubConnection.GetTransport(id) == HubTransports.Multipeer
```


## Peer links (shared by Multipeer and Nearby, `Shiny.UniversalHubs`)

```csharp
public static class PeerProtocol
{
    public const int Version = 1;                   // the handshake refuses other versions; advertised by both radios
    public const string FileResourcePrefix = "hubfile-";
}
public enum PeerMessageKind : byte { Handshake = 0x01, HandshakeReply = 0x02, Invoke = 0x10, Stream = 0x11, Cancel = 0x12, Rename = 0x13,
    Completion = 0x20, StreamItem = 0x21, StreamEnd = 0x22, Error = 0x23, Push = 0x30, HostRenamed = 0x31, Disconnect = 0x40,
    FileRequest = 0x50, FileReply = 0x51, FileDone = 0x52, MediaOpen = 0x60, MediaOpened = 0x61, MediaPacket = 0x62, MediaClose = 0x63 }
public sealed record PeerHandshakeReply(bool Accepted, string? Reason, string? HostName, string? HostId, bool Files, int? MediaMaxPacketSize);
public sealed record PeerError(string Type, string Message);
public sealed record PeerRename(string? Name);
public sealed record PeerFileRequest(bool Upload, string Name, long? Size);
public sealed record PeerFileReply(int Status, string? Message, long? Length);
public sealed record PeerFileDone(bool Succeeded, string? Message, long Bytes);
```


## Nearby (`Shiny.UniversalHubs.Nearby`)

```csharp
public static class NearbyExtensions
{
    // host - once per AddUniversalServer
    public static UniversalServerBuilder Nearby(this UniversalServerBuilder server, string serviceId, Action<NearbyHostOptions>? configure = null);
    // client - inside AddUniversalHubClient's configure, once per contract; the same serviceId and Strategy as the host
    public static UniversalHubClientOptions Nearby(this UniversalHubClientOptions options, string serviceId, Action<NearbyClientOptions>? configure = null);
}

public enum NearbyStrategy { Cluster, Star }        // Cluster: host and visit at once (P2P_CLUSTER); Star: P2P_STAR

public sealed class NearbyHostOptions
{
    public string ServiceId { get; }
    public string? EndpointName { get; set; }       // default: BleHubHostOptions.LocalName; cut to fit the endpoint info
    public NearbyStrategy Strategy { get; set; }    // Cluster
}

public sealed class NearbyClientOptions
{
    public string ServiceId { get; }
    public TimeSpan ConnectTimeout { get; set; }    // 15s, then the next transport is tried
    public string? EndpointName { get; set; }       // the host sees it as NearbyPeer.EndpointName; default: the hub client name
    public NearbyStrategy Strategy { get; set; }    // Cluster - must match the host
}

// on the host (Shiny.UniversalHubs.Host): HubConnection.Nearby
public sealed record NearbyPeer(string EndpointName);

public static class NearbyProtocol
{
    public const int MaxEndpointInfoLength = 131;
    public const int MaxBytesPayloadSize = 32768;   // longer messages travel as Part / Last frames
    public static void ValidateServiceId(string serviceId);               // 1-64 printable ASCII
    public static IReadOnlyList<string> BonjourServices(string serviceId); // ["_{SHA-256(serviceId)[0..6] HEX}._tcp"] for NSBonjourServices
    public static uint HubTag(string characteristicUuid);                  // how a hub is listed in the endpoint info
}
public enum NearbyFrameKind : byte { Message = 0x00, Part = 0x01, Last = 0x02, Open = 0x10, Close = 0x11, FileName = 0x20 }
// connection ids: "nb:{guid}" - HubConnection.GetTransport(id) == HubTransports.Nearby
```

