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
    All = Ble | Http
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
    // once, with at least one hub; registers AddBleHubServer + AddShinyHttpServer(autoStart: false)
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
    public static HubTransports GetTransport(string connectionId);
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
    public HubTransports Transports { get; }                    // None = the host has gone
    public int? Rssi { get; }
    public static HubHostInfo FromUrl(Uri serverUrl, string endpoint = "/hubs");   // HTTP only
}

public sealed record HubConnectOptions(string? Name = null, string? AppVersion = null, Dictionary<string, string>? Properties = null)
{
    public HubTransports Prefer { get; init; }                  // Http
    public HubTransports Allowed { get; init; }                 // All
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
