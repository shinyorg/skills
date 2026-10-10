# Shiny.BluetoothLE.Hubs API Reference

## Shared (`Shiny.BluetoothLE.Hubs`)

```csharp
[AttributeUsage(AttributeTargets.Interface)]
public sealed class BleHubClientAttribute : Attribute
{
    public string? ProxyName { get; set; }      // default: interface name without leading I + "Client"
}

public class BleHubProtocolOptions                     // app-wide: server.Protocol(o => ...) or AddBleHubClient(..., o => o.Protocol(p => ...))
{
    public const string DefaultServiceUuid = "98ac0390-867f-4c0d-b9f5-4266bdeac29b";   // host's and clients' ServiceUuid default
    public int MaxPayloadSize { get; set; }      // 256 KB
    public TimeSpan ReassemblyTimeout { get; set; }   // 30s
    public TimeSpan RequestTimeout { get; set; } // 30s - per hub call, streams excluded
    public int MaxPartialMessages { get; set; }  // 16 per peer
}

public interface IBleHubSerializer             // register your own before AddBleHubServer/AddBleHubClient to replace JSON
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
public class BleHubRemoteException : BleHubException
{
    string RemoteErrorType;                      // hub method threw
    public const string RenameRefused = "HubRenameRefused";   // the host refused a Rename - Message says why
}
public class BleHubDisconnectedException : BleHubException { HubDisconnect? Disconnect; string? Reason; }   // Reason = Disconnect?.Description
public class BleHubFileTransferNotSupportedException : BleHubException;
```

## Host (`Shiny.BluetoothLE.Hubs.Host`)

```csharp
// once per app, at least one hub; adds AddBluetoothLeHosting() on Android/iOS/Mac Catalyst
IServiceCollection AddBleHubServer(Action<BleHubServerBuilder> configure);

public sealed class BleHubServerBuilder
{
    BleHubServerBuilder ServiceUuid(string serviceUuid);                      // sets BleHubHostOptions.ServiceUuid
    BleHubServerBuilder Host(Action<BleHubHostOptions> configure);
    BleHubServerBuilder Protocol(Action<BleHubProtocolOptions> configure);   // app-wide
    BleHubServerBuilder AddHub<THub>(string characteristicUuid, Action<BleHubOptions>? configure = null);   // throws on a duplicate hub/characteristic or bad UUID
}

public class BleHubOptions
{
    public int? MaxClients { get; set; }                         // 8, null = unlimited
    public Func<HandshakeInfo, string?>? ValidateClient { get; set; }   // return a rejection reason
}

public class BleHubHostOptions
{
    public string ServiceUuid { get; set; }                      // BleHubProtocolOptions.DefaultServiceUuid - the one service holding every hub; advertised
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
    Task Rename(string? localName, CancellationToken cancellationToken = default);   // sets LocalName, re-advertises, tells clients - no stop
}

public abstract class BleHub<TContract> where TContract : class
{
    public BleHubCallerContext Context { get; }
    public IHubCallerClients<BleHubPush<TContract>> Clients { get; }
    public IGroupManager Groups { get; }
    public virtual Task OnConnectedAsync();
    public virtual Task OnDisconnectedAsync(string? reason);
    public virtual Task OnDisconnectedAsync(HubDisconnect disconnect);   // default: OnDisconnectedAsync(disconnect.Description)
    public virtual Task OnRenamedAsync(string? previousName);            // Context.Client.Name is the new name; throw to refuse
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
    public string? Name { get; }                 // handshake name, or the latest rename
    public string? AppVersion { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }
    public int Mtu { get; }                       // ATT MTU of the client's BLE link (frames are Mtu - 3); 0 on another transport
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
    event EventHandler<BleHubClientRenamedEventArgs>? ClientRenamed;
    Task Disconnect(string connectionId, string? reason = null);
    // generated per hub: IHubClients<BleHubPush<TContract>> Clients { get; }   (C# 14 extension property)
}

public sealed record BleHubClientDisconnectedEventArgs(BleHubConnectedClient Client, HubDisconnect Disconnect)
{
    public string Reason { get; }                // Disconnect.Description
}

public sealed record BleHubClientRenamedEventArgs(BleHubConnectedClient Client, string? PreviousName);   // Client.Name = new name

// generated per contract, eg. for event Action<GameState> StateChanged:
public static Task StateChanged(this BleHubPush<IGameHub> push, GameState gameState, CancellationToken cancellationToken = default);
```

## Client (`Shiny.BluetoothLE.Hubs.Client`)

```csharp
// once per contract; adds AddBluetoothLE() on Android/iOS/Mac Catalyst (Apple: background alerts off)
IServiceCollection AddBleHubClient<TContract>(string characteristicUuid, Action<BleHubClientOptions>? configure = null);
// resolves as IBleHubClient<TContract>, TContract and the generated proxy

public sealed class BleHubClientOptions
{
    public string ServiceUuid { get; set; }                       // BleHubProtocolOptions.DefaultServiceUuid - must match the host's
    public BleHubClientOptions Protocol(Action<BleHubProtocolOptions> configure);   // app-wide
}

public interface IBleHubConnection
{
    BleHubClientStatus Status { get; }            // Disconnected, Connecting, Connected, Disconnecting
    BleHubHostInfo? Host { get; }
    string? HostName { get; }                    // kept up to date when the host renames
    string? ClientName { get; }                  // BleHubConnectOptions.Name or the latest Rename; null while disconnected
    bool CanTransferFiles { get; }
    event EventHandler<BleHubStatusChangedEventArgs>? StatusChanged;
    event EventHandler? Connected;
    event EventHandler<HubDisconnect>? Disconnected;   // not raised for ConnectionFailed
    event EventHandler<string?>? HostRenamed;     // HostName already updated; in order with hub events
    IObservable<BleHubHostInfo> Discover();       // scan by BleHubClientOptions.ServiceUuid; dispose to stop; skips this app's own host (same UUID + advertised name)
    Task Connect(BleHubHostInfo host, BleHubConnectOptions? options = null, CancellationToken cancellationToken = default);
    Task Disconnect();
    Task Rename(string? name, CancellationToken cancellationToken = default);   // no-op if unchanged; refusal = BleHubRemoteException (RenameRefused)
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
**Shiny.UniversalHubs** (`AddUniversalServer(server => server.AddHub<THub>(...))` on the host, `AddUniversalHubClient<TContract>(...)` on the client)
rather than these directly.

- `IHubContext<THub>.TransportEndpoint` → `IBleHubTransportEndpoint`: `Connect(connectionId, HandshakeInfo, IBleHubPeerChannel, ct)`
  (returns a rejection or null), `GetMethodKind`, `Invoke` → `BleHubInvocationResult(Result, AbortRequested, AbortReason)`,
  `Stream`, `Disconnected(connectionId, HubDisconnect)`, `Disconnect(connectionId, HubDisconnect)`,
  `Rename(connectionId, name, ct)` (null, or the refusal reason to relay as `RenameRefused`), `FindClient`. Clients connected this way share Clients, Groups, MaxClients and
  IHubContext with BLE clients.
- `IBleHubPeerChannel`: `Push(eventName, encodedArguments, ct)`, `Disconnect(HubDisconnect, ct)`, `HostRenamed(hostName, ct)`
  (default no-op, called in order with `Push`) - implemented by the transport.
- `BleHubClient.ConnectExternal(events => IBleHubClientTransport, options, ct)`, `BleHubClient.ExternalTransport`.
- `IBleHubClientTransport`: `Handshake`, `Invoke`, `Stream`, `Rename` (default throws `NotSupportedException`),
  `CanTransferFiles`, `Upload`, `Download`, `Close`.
  `IBleHubClientTransportEvents`: `Pushed(eventName, encodedArguments)`, `HostRenamed(hostName)` (in order with `Pushed`),
  `Closed(HubDisconnect)`.
