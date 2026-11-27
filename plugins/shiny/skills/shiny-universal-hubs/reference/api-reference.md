# Shiny.UniversalHubs API Reference

Everything is in `namespace Shiny.UniversalHubs` (wire-level types in `Shiny.UniversalHubs.Protocol`).

## Contracts and hubs (`Shiny.UniversalHubs`)

```csharp
[AttributeUsage(AttributeTargets.Interface)]
public sealed class UniversalHubContractAttribute : Attribute;

public abstract class UniversalHub<TContract> where TContract : class
{
    public HubCallerContext Context { get; }
    public IHubCallerClients<HubPush<TContract>> Clients { get; }      // generated extension methods per contract event
    public IGroupManager Groups { get; }
    public HubConnection Connection { get; }

    public virtual Task OnConnectedAsync(HubConnection connection);
    public virtual Task OnDisconnectedAsync(HubConnection connection, HubDisconnect disconnect);
    public virtual Task OnRenamedAsync(string? previousName);          // throw to refuse
}

public sealed class HubCallerContext
{
    public string ConnectionId { get; }
    public HubConnectedClient Client { get; }
    public IDictionary<string, object> Items { get; }
    public CancellationToken ConnectionAborted { get; }
    public void Abort(string? reason = null);                          // after the current reply
}

public sealed record HubConnection(string Id, IHubTransport? Transport, HubConnectedClient Client)
{
    public IDictionary<string, object> Properties { get; }             // = Client.Items
    public T? Get<T>() where T : class;                                // transport context by type
    public static HubConnection From(HubConnectedClient client);
}

public sealed class HubConnectedClient
{
    public string Id { get; }
    public string? Name { get; }
    public string? AppVersion { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }     // from the handshake
    public int Mtu { get; }                                            // BLE only
    public DateTimeOffset ConnectedAt { get; }
    public ConcurrentDictionary<string, object> Items { get; }
    public T? Get<T>() where T : class;
}

public interface IHubClients<T>
{
    T All { get; }
    T AllExcept(params IEnumerable<string> excludedConnectionIds);
    T Client(string connectionId);
    T Clients(params IEnumerable<string> connectionIds);
    T Group(string groupName);
    T Groups(params IEnumerable<string> groupNames);
    T GroupExcept(string groupName, params IEnumerable<string> excludedConnectionIds);
}
public interface IHubCallerClients<T> : IHubClients<T> { T Caller { get; } T Others { get; } T OthersInGroup(string groupName); }

public interface IGroupManager
{
    Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default);
    Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default);
    IReadOnlyList<string> GetMembers(string groupName);
}

public interface IHubContext<THub> where THub : class
{
    bool IsRunning { get; }
    Task Start(CancellationToken cancellationToken = default);         // on every transport the hub picked
    Task Stop(string? reason = null);
    IGroupManager Groups { get; }
    IReadOnlyList<HubConnectedClient> ConnectedClients { get; }
    event EventHandler<HubConnectedClient>? ClientConnected;
    event EventHandler<HubClientDisconnectedEventArgs>? ClientDisconnected;
    event EventHandler<HubClientRenamedEventArgs>? ClientRenamed;
    Task Disconnect(string connectionId, string? reason = null);
    event EventHandler<HubMediaChannelEventArgs>? MediaChannelOpened;
    IHubMediaChannel? GetMediaChannel(string connectionId);
    // generated: context.Clients => IHubClients<HubPush<TContract>>
}

public sealed record HubDisconnect(HubDisconnectReason Reason, string? Message = null) { public string Description { get; } }
public enum HubDisconnectReason { ClientDisconnect, ClientTimeout, ServerDisconnect, ServerShutdown, ConnectionFailed }
```

## Host registration

```csharp
public static IServiceCollection AddUniversalServer(this IServiceCollection services, Action<UniversalServerBuilder> configure);

public sealed class UniversalServerBuilder
{
    public UniversalServerBuilder Configure(Action<UniversalServerOptions> configure);
    public UniversalServerBuilder Protocol(Action<HubProtocolOptions> configure);       // app-wide
    public UniversalServerBuilder AddHub<THub>(string name, Action<UniversalHubBuilder> configure) where THub : class;
    public UniversalServerBuilder AllowDynamicHubs(Action<DynamicHubOptions>? configure = null);
    public UniversalServerBuilder AddTransport<TTransport>() where TTransport : class, IHubTransport;
    // transport packages: .BluetoothLE(o => ...), .Http(o => ...), .HttpServer(http => ...), .Multipeer(type, o => ...), .Nearby(id, o => ...)
}

public sealed class UniversalHubBuilder
{
    public string Name { get; }
    public UniversalHubBuilder MaxClients(int? maxClients);                              // default 8, across transports
    public UniversalHubBuilder ValidateClient(Func<HandshakeInfo, string?> validate);
    public UniversalHubBuilder Serializer(IHubSerializer serializer);
    public UniversalHubBuilder UseTransport<TTransport>(object? settings = null) where TTransport : class, IHubTransport;
    // transport packages: .UseBluetoothLE(service, characteristic, advertise), .UseHttp(type, allowTunnel), .UseMultipeer(), .UseNearby()
}

public class UniversalServerOptions
{
    public string? LocalName { get; set; }
    public string HostId { get; set; }                                   // new Guid per launch unless set
    public TimeSpan ResumeWindow { get; set; } = 30s;                    // zero turns holding off
    public int ResumeBufferSize { get; set; } = 128;
    public Func<UniversalHubFileRequest, bool>? AuthorizeFile { get; set; }
    public HubFileTransferOptions? FileTransfers { get; }
    public UniversalServerOptions EnableFileTransfers(string rootDirectory, Action<HubFileTransferOptions>? configure = null);
    public HubMediaChannelOptions? MediaChannels { get; }
    public UniversalServerOptions EnableMediaChannels(int maxPacketSize = 128 * 1024);
}

public class HubFileTransferOptions(string rootDirectory)
{
    public string RootDirectory { get; }
    public bool AllowUploads { get; set; } = true;
    public bool AllowDownloads { get; set; } = true;
    public bool OverwriteExistingUploads { get; set; } = true;
    public long? MaxUploadSize { get; set; }
}
public sealed record UniversalHubFileRequest(HubTransferDirection Direction, string FileName, long? Size, HubConnectedClient? Client);
```

## Host at runtime

```csharp
public interface IUniversalHubHost
{
    IReadOnlyList<IHubTransport> Transports { get; }
    IReadOnlyList<IHubTransport> RunningTransports { get; }
    IReadOnlyList<string> ListenUrls { get; }                          // HTTP
    Task<IReadOnlyList<IHubTransport>> Start(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IHubTransport>> Start(IEnumerable<Type> transports, CancellationToken cancellationToken = default);
    Task Stop(string? reason = null);
    Task Stop(IEnumerable<Type> transports, string? reason = null);
    Task<IReadOnlyList<IHubTransport>> StartHub<THub>(CancellationToken cancellationToken = default) where THub : class;
    Task<IReadOnlyList<IHubTransport>> StartHub<THub>(IEnumerable<Type> transports, CancellationToken cancellationToken = default) where THub : class;
    Task StopHub<THub>(string? reason = null) where THub : class;
    Task StopHub<THub>(IEnumerable<Type> transports, string? reason = null) where THub : class;
    IReadOnlyList<IHubTransport> GetRunningTransports<THub>() where THub : class;
    event EventHandler? RunningChanged;
    Task Rename(string? localName, CancellationToken cancellationToken = default);
    event EventHandler<HubFileTransferredEventArgs>? FileTransferred;
    event EventHandler<HubFileProgressEventArgs>? FileTransferProgress;
}

// UniversalHubHostExtensions
host.Start<TTransport>(); host.Stop<TTransport>(reason);
host.StartHub<THub, TTransport>(); host.StopHub<THub, TTransport>(reason);
host.IsRunning<TTransport>(); host.GetTransport<TTransport>();
```

## Transport seams

```csharp
public interface IHubTransport
{
    string DisplayName { get; }
    bool IsSupported { get; }
    bool IsRunning(IHubBinding hub);
    Task Start(IReadOnlyList<IHubBinding> hubs, CancellationToken cancellationToken);
    Task Stop(IReadOnlyList<IHubBinding> hubs, HubDisconnect disconnect);
    Task HostRenamed(CancellationToken cancellationToken) => Task.CompletedTask;
}

public interface IHubBinding
{
    string Key { get; }                    // the hub's name
    string Name { get; }
    Type? HubType { get; }                 // null for a dynamic hub
    IReadOnlySet<Type> Transports { get; }
    IHubEndpoint Endpoint { get; }
    T? GetSettings<T>() where T : class;   // what UseTransport / UseBluetoothLE / UseHttp stored
}

public interface IHubEndpoint
{
    HubDefinition Registration { get; }
    Type ContractType { get; }
    IHubSerializer Serializer { get; }
    Task<string?> Connect(string connectionId, HandshakeInfo info, IHubPeerChannel channel, CancellationToken cancellationToken);   // null = accepted
    HubMethodKind GetMethodKind(string method);
    Task<HubInvocationResult> Invoke(string connectionId, string method, ReadOnlyMemory<byte> arguments, CancellationToken cancellationToken);
    IAsyncEnumerable<byte[]> Stream(string connectionId, string method, ReadOnlyMemory<byte> arguments, CancellationToken cancellationToken);
    void Disconnected(string connectionId, IHubPeerChannel channel, HubDisconnect disconnect);
    string? EnableResume(string connectionId);
    TimeSpan ResumeWindow { get; }
    Task<string?> Resume(string resumeToken, string connectionIdPrefix, IHubPeerChannel channel, Func<string, Task> accepted, CancellationToken cancellationToken);
    Task<string?> Rename(string connectionId, string? name, CancellationToken cancellationToken);
    Task Disconnect(string connectionId, HubDisconnect disconnect);
    HubConnectedClient? FindClient(string connectionId);
    bool MediaChannelOpened(string connectionId, IHubMediaChannel channel);
}

public interface IHubPeerChannel
{
    Task Push(string eventName, byte[] arguments, CancellationToken cancellationToken);
    Task Disconnect(HubDisconnect disconnect, CancellationToken cancellationToken);
    Task HostRenamed(string? hostName, CancellationToken cancellationToken) => Task.CompletedTask;
    IHubTransport? Transport => null;                 // what HubConnection.Transport reports
    object? GetFeature(Type type) => null;            // what HubConnection.Get<T>() finds
}

public interface IDynamicHubTransport : IHubTransport
{
    object? CreateSettings(DynamicHubDefinition definition);   // null = don't serve it; throw = refuse it
}

public interface IHubClientTransport
{
    string DisplayName { get; }
    bool IsSupported { get; }
    IObservable<HubHostSighting> Discover(HubClientContext client);
    Task Connect(HubClientContext client, HubHostInfo host, CancellationToken cancellationToken);
}

public sealed class HubClientContext
{
    public string HubName { get; }
    public ILogger? Logger { get; }
    public HubHandshakeOptions ConnectOptions { get; }   // Name, AppVersion, Properties, AutomaticReconnect
    public Task Connect(Func<IHubConnectionTransportEvents, IHubConnectionTransport> createTransport, CancellationToken cancellationToken);
}

public sealed record HubHostSighting(string Name, string? HostId, HubEndpoint? Endpoint);   // Endpoint null = gone
public abstract record HubEndpoint;

public interface IHubConnectionTransport          // one connection's link
{
    Task<HandshakeAck> Handshake(HandshakeInfo info, CancellationToken cancellationToken);
    Task<ReadOnlyMemory<byte>> Invoke(string method, byte[] arguments, CancellationToken cancellationToken);
    IAsyncEnumerable<ReadOnlyMemory<byte>> Stream(string method, byte[] arguments, CancellationToken cancellationToken);
    Task Rename(string? name, CancellationToken cancellationToken);
    bool CanTransferFiles { get; }
    Task<HubFileTransferResult> Upload(Stream source, long length, string remoteFileName, IProgress<HubTransferProgress>? progress, CancellationToken cancellationToken);
    Task<HubFileTransferResult> Download(string remoteFileName, Stream destination, IProgress<HubTransferProgress>? progress, CancellationToken cancellationToken);
    Task Close(CancellationToken cancellationToken);
    bool CanOpenMediaChannel { get; }
    Task<IHubMediaChannel> OpenMediaChannel(CancellationToken cancellationToken);
}
public interface IHubConnectionTransportEvents
{
    void Pushed(string eventName, ReadOnlyMemory<byte> arguments);
    void HostRenamed(string? hostName);
    void Closed(HubDisconnect disconnect);
    void Reconnecting(HubDisconnect drop) { }
    void Reconnected() { }
}
```

## Client

```csharp
public static IServiceCollection AddUniversalHubClient<TContract>(this IServiceCollection services, string hubName, Action<UniversalHubClientOptions> configure);

public class UniversalHubClientOptions
{
    public bool AutomaticReconnect { get; set; } = true;
    public UniversalHubClientOptions UseTransport<TTransport>(Func<IServiceProvider, TTransport> create, Action<IServiceCollection>? services = null) where TTransport : class, IHubClientTransport;
    public UniversalHubClientOptions Priority(params Type[] transports);
    public UniversalHubClientOptions Priority<T1, T2>();  // <T1,T2,T3>, <T1,T2,T3,T4>
    public UniversalHubClientOptions Serializer(IHubSerializer serializer);
    public UniversalHubClientOptions Protocol(Action<HubProtocolOptions> configure);
    // transport packages: .UseBluetoothLE(service, characteristic, scanFor), .UseHttp(type, h => ...), .UseMultipeer(type, o => ...), .UseNearby(id, o => ...)
}

public interface IHubConnection
{
    HubClientStatus Status { get; }                  // Disconnected, Connecting, Connected, Disconnecting, Reconnecting
    string? HostName { get; }
    string? ClientName { get; }
    event EventHandler<HubStatusChangedEventArgs>? StatusChanged;
    event EventHandler? Connected;
    event EventHandler<HubDisconnect>? Disconnected;
    event EventHandler<string?>? HostRenamed;
    Task Disconnect();
    Task Rename(string? name, CancellationToken cancellationToken = default);
    bool CanTransferFiles { get; }
    Task<HubFileTransferResult> UploadFile(string localFilePath, string? remoteFileName = null, IProgress<HubTransferProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<HubFileTransferResult> UploadStream(Stream source, long length, string remoteFileName, IProgress<HubTransferProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<HubFileTransferResult> DownloadFile(string remoteFileName, string localFilePath, IProgress<HubTransferProgress>? progress = null, CancellationToken cancellationToken = default);
    bool CanOpenMediaChannel { get; }
    Task<IHubMediaChannel> OpenMediaChannel(CancellationToken cancellationToken = default);
}
public interface IHubProxy<out TContract> : IHubConnection { TContract Hub { get; } }

public interface IUniversalHubClient<out TContract> : IHubProxy<TContract> where TContract : class
{
    IReadOnlyList<IHubClientTransport> Transports { get; }          // priority order
    IHubClientTransport? Transport { get; }                         // carrying the connection
    bool IsEnabled(Type transport);
    Task SetEnabled(Type transport, bool enabled);
    event EventHandler? EnabledChanged;
    IObservable<HubHostInfo> DiscoverAll();
    IObservable<HubHostInfo> DiscoverAll(IEnumerable<Type> transports);
    Task Connect(HubHostInfo host, HubConnectOptions? options = null, CancellationToken cancellationToken = default);
}
// UniversalHubClientTransportExtensions: client.IsEnabled<T>(), Enable<T>(), Disable<T>(), Solo<T>()

public sealed record HubHostInfo(string Key, string Name, string? HostId)
{
    public IReadOnlyDictionary<Type, HubEndpoint> Endpoints { get; }
    public IReadOnlyList<Type> Transports { get; }                   // priority order; empty = gone
    public TEndpoint? Endpoint<TEndpoint>() where TEndpoint : HubEndpoint;
    public bool IsSeenOver<TTransport>() where TTransport : IHubClientTransport;
}

public sealed record HubConnectOptions(string? Name = null, string? AppVersion = null, Dictionary<string, string>? Properties = null)
{
    public IReadOnlyList<Type> Preferred { get; init; }
    public IReadOnlySet<Type>? Allowed { get; init; }
    public HubConnectOptions Prefer<TTransport>();  public HubConnectOptions Prefer(params IEnumerable<Type> transports);
    public HubConnectOptions Allow<TTransport>();   public HubConnectOptions Allow(params IEnumerable<Type> transports);
    public HubConnectOptions Only<TTransport>();
}
```

## Serializers

```csharp
public interface IHubSerializer
{
    string Name { get; }                       // compared in the handshake
    byte[] Serialize<T>(T value);
    T Deserialize<T>(ReadOnlySpan<byte> data);
}
public sealed class JsonHubSerializer(ISerializer? serializer = null) : IHubSerializer   // Name = "json"
```

## Files and media

```csharp
public enum HubTransferDirection { Upload, Download }
public sealed record HubFileTransferResult(HubTransferDirection Direction, string FileName, long BytesTransferred, TimeSpan Elapsed);
public sealed record HubTransferProgress(long BytesPerSecond, long? BytesToTransfer, long BytesTransferred) { public double? Fraction { get; } }
public sealed record HubFileTransferredEventArgs(HubConnectedClient? Client, string PeerIdentifier, HubTransferDirection Direction, string FileName, string LocalFilePath, long BytesTransferred, TimeSpan Elapsed);
public sealed record HubFileProgressEventArgs(HubConnectedClient? Client, string PeerIdentifier, HubTransferDirection Direction, string FileName, HubTransferProgress Progress);

public interface IHubMediaChannel : IAsyncDisposable
{
    string? ConnectionId { get; }
    int MaxPacketSize { get; }
    bool IsOpen { get; }
    ValueTask Send(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ReadOnlyMemory<byte>> Receive(CancellationToken cancellationToken = default);   // one reader
    Task Completion { get; }
}
public sealed record HubMediaChannelEventArgs(HubConnectedClient Client, IHubMediaChannel Channel);
```

## Bluetooth LE (`Shiny.UniversalHubs.BluetoothLE`)

```csharp
hub.UseBluetoothLE(string serviceUuid, string characteristicUuid, bool advertise = true);
server.BluetoothLE(Action<BluetoothLETransportOptions> configure);
options.UseBluetoothLE(string serviceUuid, string characteristicUuid, string? scanFor = null);   // client
definition.UseBluetoothLE(string characteristicUuid, string? serviceUuid = null, bool advertise = true);   // dynamic hub

public sealed class BluetoothLETransportOptions
{
    public TimeSpan ClientSweepInterval { get; set; } = 10s;
    public bool SecureFileTransfers { get; set; }
    public bool SecureMediaChannels { get; set; }
    public L2CapTransferOptions? FileTransfer { get; set; }
    public string? DynamicServiceUuid { get; set; }
}

public sealed class BleHostTransport : IDynamicHubTransport;        // DisplayName "Bluetooth LE"
public sealed class BleClientTransport : IHubClientTransport;
public sealed record BleHubEndpoint(IPeripheral Peripheral, string? Name, int Rssi) : HubEndpoint { public string Id { get; } }
public interface IBleFileHandler { Task Handle(BleFileRequest request, CancellationToken cancellationToken); }   // take over L2CAP files
connection.Peripheral(); client.Peripheral();                     // Shiny.BluetoothLE.Hosting.IPeripheral
```

UUIDs: 128-bit, or 16-bit (`"180D"`) / 32-bit (`"ABCD1234"`) expanded onto `0000xxxx-0000-1000-8000-00805f9b34fb`.
Advertisement budget: 31 bytes = 3 (flags) + per UUID size in use (2 + n × 2 / 4 / 16). Registration throws when every
`advertise: true` service won't fit.

## HTTP (`Shiny.UniversalHubs.Http`)

```csharp
hub.UseHttp(string? mdnsServiceType = null, bool allowTunnel = false);
server.Http(Action<HttpTransportOptions> configure);
server.HttpServer(Action<ShinyHttpServerBuilder> configure);
options.UseHttp(string? mdnsServiceType = null, Action<HttpClientTransportOptions>? configure = null);   // client
definition.UseHttp(string? mdnsServiceType = null, bool allowTunnel = false);                             // dynamic hub

public class HttpTransportOptions
{
    public string BasePath { get; set; } = "/hubs";            // WebSocket at {BasePath}/{hub name}
    public bool Advertise { get; set; } = true;                 // mDNS
    public string ServiceType { get; set; } = "_shinyhub._tcp";
    public bool ManageHttpServer { get; set; } = true;
    public IList<string> DynamicServiceTypes { get; }
}
public sealed class HttpClientTransportOptions
{
    public string ServiceType { get; }
    public TimeSpan ConnectTimeout { get; set; } = 6s;
    public TimeSpan RediscoverTimeout { get; set; } = 3s;
    public Func<HttpMessageHandler>? MessageHandlerFactory { get; set; }
}

public sealed class HttpHostTransport : IDynamicHubTransport;       // DisplayName "HTTP", ListenUrls
public sealed class HttpClientTransport : IHubClientTransport;
public sealed record HttpHubEndpoint(DiscoveredHttpServer Server) : HubEndpoint;
public static class HttpHubHost { public static HubHostInfo FromUrl(Uri serverUrl, string basePath = "/hubs"); }
connection.HttpContext();                                         // the WebSocket's request
```

## Multipeer and Nearby

```csharp
server.Multipeer(string serviceType, Action<MultipeerHostOptions>? configure = null);  hub.UseMultipeer();
options.UseMultipeer(string serviceType, Action<MultipeerClientOptions>? configure = null);
public sealed class MultipeerHostTransport; public sealed class MultipeerClientTransport;
public sealed record MultipeerHubEndpoint(string DisplayName, IReadOnlyDictionary<string, string> Info) : HubEndpoint;
public sealed record MultipeerPeer(string DisplayName);   connection.MultipeerPeer();

server.Nearby(string serviceId, Action<NearbyHostOptions>? configure = null);  hub.UseNearby();
options.UseNearby(string serviceId, Action<NearbyClientOptions>? configure = null);
public sealed class NearbyHostTransport; public sealed class NearbyClientTransport;
public sealed record NearbyHubEndpoint(string EndpointName) : HubEndpoint;
public sealed record NearbyPeer(string EndpointName);     connection.NearbyPeer();
NearbyProtocol.BonjourServices(serviceId);                // for NSBonjourServices
```

## Dynamic hubs

```csharp
public sealed class DynamicHubOptions { public int MaxHubs { get; set; } = 6; }
public sealed class DynamicHubDefinition(string name)
{
    public string Name { get; }
    public int? MaxClients { get; init; } = 8;
    public DynamicHubDefinition With(object transportSettings);
    public T? GetSettings<T>() where T : class;
}
public interface IDynamicHubs
{
    IReadOnlyList<IDynamicHub> Hubs { get; }
    IDynamicHub? Find(string name);
    IDynamicHub Add(DynamicHubDefinition definition, IDynamicHubHandler handler);
    Task Remove(string name, string? reason = null);
}
public interface IDynamicHub
{
    DynamicHubDefinition Definition { get; }
    IReadOnlyList<IHubTransport> RunningTransports { get; }
    Task<IReadOnlyList<IHubTransport>> Start(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IHubTransport>> Start(IEnumerable<Type> transports, CancellationToken cancellationToken = default);
    Task Stop(string? reason = null);
    IReadOnlyList<HubConnection> Connections { get; }
    IGroupManager Groups { get; }
    IHubClients<DynamicHubPush> Clients { get; }        // .Send(eventName, serializedArguments)
    Task Disconnect(string connectionId, string? reason = null);
}
public interface IDynamicHubHandler
{
    DynamicHubMethodKind GetMethodKind(string method);  // Invoke, Stream, NotFound
    Task<byte[]?> Invoke(DynamicHubCall call, CancellationToken cancellationToken);
    IAsyncEnumerable<byte[]> Stream(DynamicHubCall call, CancellationToken cancellationToken);
    Task OnConnectedAsync(HubConnection connection);
    Task OnDisconnectedAsync(HubConnection connection, HubDisconnect disconnect);
    Task OnRenamedAsync(HubConnection connection, string? previousName);
}

public static IServiceCollection AddDynamicHubClients(this IServiceCollection services, Action<UniversalHubClientOptions> configure);
public interface IDynamicHubClients { IUniversalHubClient<IDynamicHubCalls> Create(string hubName, Action<UniversalHubClientOptions>? configure = null); }
public interface IDynamicHubCalls
{
    Task<ReadOnlyMemory<byte>> Invoke(string method, IReadOnlyList<ReadOnlyMemory<byte>> arguments, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ReadOnlyMemory<byte>> Stream(string method, IReadOnlyList<ReadOnlyMemory<byte>> arguments, CancellationToken cancellationToken = default);
    event Action<string, IReadOnlyList<ReadOnlyMemory<byte>>>? Pushed;
}
```

## Exceptions and protocol

```csharp
public class HubException : Exception;                          // "Host refused: ..." for refusals
public class HubRemoteException : HubException { public string RemoteErrorType { get; } }
public class HubDisconnectedException : HubException { public HubDisconnect? Disconnect { get; } }
public class HubFileTransferNotSupportedException : HubException;
public class HubProtocolOptions { int MaxPayloadSize = 256 KB; TimeSpan ReassemblyTimeout = 30s; TimeSpan RequestTimeout = 30s; int MaxPartialMessages = 16; }

FrameCodec.ProtocolVersion = 2;        // BLE frames
PeerProtocol.Version = 2;              // the session over HTTP, Multipeer, Nearby
UniversalHubProtocol.Version = 3;      // HTTP routes and mDNS TXT keys (proto, hostId, name, path, hubs)
```
