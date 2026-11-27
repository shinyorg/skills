---
name: shiny-ble-hubs
description: Shiny.BluetoothLE.Hubs (BLE hubs) is retired - it is now Shiny.UniversalHubs with the Shiny.UniversalHubs.BluetoothLE transport. Use this to migrate old [BleHubClient] / BleHub<T> / AddBleHubServer / AddBleHubClient code; for new code use the shiny-universal-hubs skill.
auto_invoke: true
triggers:
  - ble hubs
  - bluetoothle hubs
  - bluetooth le hubs
  - Shiny.BluetoothLE.Hubs
  - Shiny.BluetoothLE.Hubs.Host
  - Shiny.BluetoothLE.Hubs.Client
  - BleHub
  - BleHubClient
  - BleHubClientAttribute
  - IBleHubClient
  - IBleHubConnection
  - IBleHubSerializer
  - IBleHubMediaChannel
  - BleHubException
  - BleHubRemoteException
  - AddBleHubServer
  - AddBleHubClient
  - IBleHubHost
  - SBH001
  - SBH005
---

# Shiny.BluetoothLE.Hubs - retired

Shiny.BluetoothLE.Hubs no longer ships on its own. Its hubs, generator, files and media channels live in
**Shiny.UniversalHubs**, and Bluetooth LE is one transport among several: **Shiny.UniversalHubs.BluetoothLE**. Write new
code with the `shiny-universal-hubs` skill; use this one to translate old code.

## Packages

| Old | New |
|---|---|
| `Shiny.BluetoothLE.Hubs` | `Shiny.UniversalHubs` |
| `Shiny.BluetoothLE.Hubs.Host`, `Shiny.BluetoothLE.Hubs.Client` | `Shiny.UniversalHubs` + `Shiny.UniversalHubs.BluetoothLE` |

## Code

```csharp
// before
[BleHubClient] public interface IGameHub { ... }
public class GameHub : BleHub<IGameHub> { public override Task OnConnectedAsync() => ...; }
services.AddBleHubServer(server => server.AddHub<GameHub>(characteristicUuid, o => o.MaxClients = 6));
services.AddBleHubClient<IGameHub>(characteristicUuid);

// after
[UniversalHubContract] public interface IGameHub { ... }
public class GameHub : UniversalHub<IGameHub> { public override Task OnConnectedAsync(HubConnection connection) => ...; }
services.AddUniversalServer(server => server
    .AddHub<GameHub>("game", hub => hub.MaxClients(6).UseBluetoothLE(serviceUuid, characteristicUuid)));
services.AddUniversalHubClient<IGameHub>("game", o => o.UseBluetoothLE(serviceUuid, characteristicUuid));
```

| Old | New |
|---|---|
| `using Shiny.BluetoothLE.Hubs;` | `using Shiny.UniversalHubs;` |
| `BleHub<T>.OnConnectedAsync()` / `OnDisconnectedAsync(HubDisconnect)` | `UniversalHub<T>.OnConnectedAsync(HubConnection)` / `OnDisconnectedAsync(HubConnection, HubDisconnect)` |
| `IBleHubHost` | `IUniversalHubHost` (`Start<BleHostTransport>()`, `StartHub<THub>()`) |
| `IBleHubClient<T>` / `IBleHubConnection` | `IUniversalHubClient<T>` / `IHubConnection` |
| `IBleHubSerializer` | `IHubSerializer` (now has a `Name`) |
| `BleHubException`, `BleHubRemoteException`, `BleHubDisconnectedException` | `HubException`, `HubRemoteException`, `HubDisconnectedException` |
| `IBleHubMediaChannel` | `IHubMediaChannel` |
| `BleHubClientStatus`, `BleHubProtocolOptions`, `BleHubConnectedClient` | `HubClientStatus`, `HubProtocolOptions`, `HubConnectedClient` |
| `client.Peripheral` | `client.Peripheral()` |
| one service UUID for every hub | each hub's own (`UseBluetoothLE(service, characteristic, advertise)`), 16/32-bit UUIDs allowed |
| `SBH0xx` diagnostics | `SUH0xx` |

The BLE wire moved to `FrameCodec.ProtocolVersion` 2, so old and new apps refuse each other's handshake - update both
sides together.
