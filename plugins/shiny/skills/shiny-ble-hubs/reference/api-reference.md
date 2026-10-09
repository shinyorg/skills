# Shiny.BluetoothLE.Hubs API Reference

## Shared (`Shiny.BluetoothLE.Hubs`)

```csharp
[AttributeUsage(AttributeTargets.Interface)]
public sealed class BleHubClientAttribute : Attribute
{
    public string? ProxyName { get; set; }      // default: interface name without leading I + "Client"
}

public class BleHubProtocolOptions                     // services.ConfigureBleHubProtocol(o => ...)
{
    public int MaxPayloadSize { get; set; }      // 256 KB
    public TimeSpan ReassemblyTimeout { get; set; }   // 30s
    public TimeSpan RequestTimeout { get; set; } // 30s - per hub call, streams excluded
    public int MaxPartialMessages { get; set; }  // 16 per peer
}

public interface IBleHubSerializer             // register your own before AddBleHub/AddBleHubClient to replace JSON
{
    byte[] Serialize<T>(T value);
    T Deserialize<T>(ReadOnlySpan<byte> data);
}

public enum HubDisconnectReason                // travels on the wire - values never change
{
    ClientDisconnect = 0,    // the client called Disconnect() or was disposed
    ClientTimeout = 1,       // the link dropped (unsubscribe without a goodbye, sweep, client-side link loss)
    ServerDisconnect = 2,    // Context.Abort / IHubContext.Disconnect
    ServerShutdown = 3,      // IBleHubHost.Stop / IHubContext.Stop
    ConnectionFailed = 4     // a connect or handshake failed (client only, no Disconnected event)
}

public sealed record HubDisconnect(HubDisconnectReason Reason, string? Message = null)
{
    public string Description { get; }           // Message, or Describe(Reason)
    public static string Describe(HubDisconnectReason reason);   // "Disconnected", "Connection lost", "Disconnected by host", "Host stopped", "Connection failed"
}

public sealed record HandshakeInfo(int ProtocolVersion, string? Name, string? AppVersion, Dictionary<string, string>? Properties);

public class BleHubException : Exception;
public class BleHubProtocolException : BleHubException { ushort? MessageId; }
public class BleHubRemoteException : BleHubException { string RemoteErrorType; }   // hub method threw
public class BleHubDisconnectedException : BleHubException { HubDisconnect? Disconnect; string? Reason; }   // Reason = Disconnect?.Description
public class BleHubFileTransferNotSupportedException : BleHubException;
```

## Host (`Shiny.BluetoothLE.Hubs.Host`)

```csharp
IServiceCollection AddBleHub<THub>(string serviceUuid, string characteristicUuid, Action<BleHubOptions>? configure = null);
IServiceCollection ConfigureBleHubHost(Action<BleHubHostOptions> configure);

public class BleHubOptions
{
    public int? MaxClients { get; set; }                         // 8, null = unlimited
    public Func<HandshakeInfo, string?>? ValidateClient { get; set; }   // return a rejection reason
}

public class BleHubHostOptions
{
    public string? LocalName { get; set; }
    public TimeSpan ClientSweepInterval { get; set; }            // 10s
    public BleHubHostOptions EnableFileTransfers(string rootDirectory, Action<BleHubFileTransferOptions>? configure = null);
}

public class BleHubFileTransferOptions
{
    public string RootDirectory { get; }
    public bool Secure { get; set; }
    public bool AllowUploads { get; set; }                       // true
    public bool AllowDownloads { get; set; }                     // true
    public bool OverwriteExistingUploads { get; set; }           // true
    public long? MaxUploadSize { get; set; }
    public L2CapTransferOptions? Transfer { get; set; }
    public Func<BleHubFileRequest, bool>? Authorize { get; set; }
}

public interface IBleHubFileHandler { Task Handle(BleHubFileRequest request, CancellationToken ct); }
public sealed record BleHubFileRequest(L2CapFileRequest Request, BleHubConnectedClient? Client);

public interface IBleHubHost
{
    bool IsRunning { get; }                      // any hub running
    ushort FileTransferPsm { get; }
    event EventHandler<BleHubFileTransferredEventArgs>? FileTransferred;
    event EventHandler<BleHubFileProgressEventArgs>? FileTransferProgress;
    Task Start(CancellationToken cancellationToken = default);   // every hub
    Task Stop(string? reason = null);                            // every hub
}

public abstract class BleHub<TContract> where TContract : class
{
    public BleHubCallerContext Context { get; }
    public IHubCallerClients<BleHubPush<TContract>> Clients { get; }
    public IGroupManager Groups { get; }
    public virtual Task OnConnectedAsync();
    public virtual Task OnDisconnectedAsync(string? reason);
    public virtual Task OnDisconnectedAsync(HubDisconnect disconnect);   // default: OnDisconnectedAsync(disconnect.Description)
}

public sealed class BleHubCallerContext
{
    public string ConnectionId { get; }
    public BleHubConnectedClient Client { get; }
    public IDictionary<string, object> Items { get; }
    public CancellationToken ConnectionAborted { get; }
    public void Abort(string? reason = null);
}

public sealed class BleHubConnectedClient
{
    public string Id { get; }
    public string? Name { get; }
    public string? AppVersion { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }
    public int Mtu { get; }
    public DateTimeOffset ConnectedAt { get; }
    public ConcurrentDictionary<string, object> Items { get; }
    public Shiny.BluetoothLE.Hosting.IPeripheral? Peripheral { get; }   // the BLE central; null on another transport
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

public interface IHubCallerClients<T> : IHubClients<T>
{
    T Caller { get; }
    T Others { get; }
    T OthersInGroup(string groupName);
}

public interface IGroupManager
{
    Task AddToGroupAsync(string connectionId, string groupName, CancellationToken ct = default);
    Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken ct = default);
    IReadOnlyList<string> GetMembers(string groupName);
}

public interface IHubContext<THub>
{
    bool IsRunning { get; }
    Task Start(CancellationToken cancellationToken = default);   // this hub only
    Task Stop(string? reason = null);                            // this hub only
    IGroupManager Groups { get; }
    IReadOnlyList<BleHubConnectedClient> ConnectedClients { get; }
    event EventHandler<BleHubConnectedClient>? ClientConnected;
    event EventHandler<BleHubClientDisconnectedEventArgs>? ClientDisconnected;
    Task Disconnect(string connectionId, string? reason = null);
    // generated per hub: IHubClients<BleHubPush<TContract>> Clients { get; }   (C# 14 extension property)
}

public sealed record BleHubClientDisconnectedEventArgs(BleHubConnectedClient Client, HubDisconnect Disconnect)
{
    public string Reason { get; }                // Disconnect.Description
}

// generated per contract, eg. for event Action<GameState> StateChanged:
public static Task StateChanged(this BleHubPush<IGameHub> push, GameState gameState, CancellationToken cancellationToken = default);
```

## Client (`Shiny.BluetoothLE.Hubs.Client`)

```csharp
IServiceCollection AddBleHubClient<TContract>(string serviceUuid, string characteristicUuid);
// resolves as IBleHubClient<TContract>, TContract and the generated proxy

public interface IBleHubConnection
{
    BleHubClientStatus Status { get; }            // Disconnected, Connecting, Connected, Disconnecting
    BleHubHostInfo? Host { get; }
    string? HostName { get; }
    bool CanTransferFiles { get; }
    event EventHandler<BleHubStatusChangedEventArgs>? StatusChanged;
    event EventHandler? Connected;
    event EventHandler<HubDisconnect>? Disconnected;   // not raised for ConnectionFailed
    IObservable<BleHubHostInfo> Discover();       // scan by the hub's service UUID; dispose to stop
    Task Connect(BleHubHostInfo host, BleHubConnectOptions? options = null, CancellationToken cancellationToken = default);
    Task Disconnect();
    Task<L2CapTransferResult> UploadFile(string localFilePath, string? remoteFileName = null, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<L2CapTransferResult> UploadStream(Stream source, long length, string remoteFileName, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<L2CapTransferResult> DownloadFile(string remoteFileName, string localFilePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
}

public interface IBleHubClient<out TContract> : IBleHubConnection
{
    TContract Hub { get; }
}

public sealed record BleHubHostInfo(IPeripheral Peripheral, string? Name, int Rssi) { public string Id { get; } }
public sealed record BleHubStatusChangedEventArgs(BleHubClientStatus Status, HubDisconnect? Disconnect = null)
{
    public string? Reason { get; }               // Disconnect?.Description
}
public sealed record BleHubConnectOptions(string? Name = null, string? AppVersion = null, Dictionary<string, string>? Properties = null);
```

## Other transports (hidden seams)

`[EditorBrowsable(Never)]` - for transport packages, not app code. To serve hubs over Wi-Fi as well, use
**Shiny.SwitchboardR** (`AddSwitchboardR().AddHub<THub>()` on the host, `AddSwitchboardRClient<TContract>()` on the client)
rather than these directly.

- `IHubContext<THub>.TransportEndpoint` → `IBleHubTransportEndpoint`: `Connect(connectionId, HandshakeInfo, IBleHubPeerChannel, ct)`
  (returns a rejection or null), `GetMethodKind`, `Invoke` → `BleHubInvocationResult(Result, AbortRequested, AbortReason)`,
  `Stream`, `Disconnected(connectionId, HubDisconnect)`, `Disconnect(connectionId, HubDisconnect)`, `FindClient`. Clients connected this way share Clients, Groups, MaxClients and
  IHubContext with BLE clients.
- `IBleHubPeerChannel`: `Push(eventName, encodedArguments, ct)`, `Disconnect(HubDisconnect, ct)` - implemented by the transport.
- `BleHubClient.ConnectExternal(events => IBleHubClientTransport, options, ct)`, `BleHubClient.ExternalTransport`.
- `IBleHubClientTransport`: `Handshake`, `Invoke`, `Stream`, `CanTransferFiles`, `Upload`, `Download`, `Close`.
  `IBleHubClientTransportEvents`: `Pushed(eventName, encodedArguments)`, `Closed(HubDisconnect)`.
