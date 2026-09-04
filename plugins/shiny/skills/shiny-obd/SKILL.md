---
name: shiny-obd
description: Generate code using Shiny.Obd, an OBD-II vehicle communication library for .NET with command-object pattern, adapter auto-detection, and BLE + WiFi (TCP) + serial (USB/UART) transports
auto_invoke: true
triggers:
  - obd
  - obd-ii
  - obd2
  - vehicle diagnostics
  - elm327
  - obdlink
  - IObdCommand
  - IObdConnection
  - IObdTransport
  - IObdDeviceScanner
  - ObdDiscoveredDevice
  - BleObdDeviceScanner
  - SerialObdTransport
  - SerialObdConfiguration
  - SerialObdDeviceScanner
  - SerialPortEnumerator
  - SerialPortInfo
  - AddShinyObdSerial
  - serial transport
  - usb obd
  - ttyUSB
  - WifiObdTransport
  - WifiObdConfiguration
  - WifiObdDeviceScanner
  - WifiObdEndpoint
  - AddShinyObdWifi
  - wifi transport
  - wifi obd
  - elm327 wifi
  - obdlink mx wifi
  - ObdCommand
  - ObdConnection
  - ObdException
  - ObdTimeoutException
  - obd emulator
  - protocol pinning
  - RefreshNegotiatedProtocol
  - ATSP
  - AutoConnect
  - obd simulator
  - simulate obd adapter
  - fake obd adapter
  - test obd without a car
  - driving scenario
  - simulate driving
  - simulated vehicle data
  - DrivingScenario
  - DrivingScenarioPlayer
  - StandardCommands
  - SupportedPidsCommand
  - MonitorStatusCommand
  - MonitorStatus
  - DtcReadCommand
  - ClearDtcCommand
  - DtcDecoder
  - FuelTrimCommand
  - FuelTypeCommand
  - FuelTypes
  - OxygenSensorsPresentCommand
  - OxygenSensorVoltageCommand
  - OxygenSensorLambdaCommand
  - OxygenSensorLayout
  - OxygenSensorPosition
  - CommandedAirFuelRatioCommand
  - CommandedEgrCommand
  - EgrErrorCommand
  - CommandedEvaporativePurgeCommand
  - EvapVaporPressureCommand
  - AbsoluteEvapVaporPressureCommand
  - EvapVaporPressureWideRangeCommand
  - ActualEngineTorqueCommand
  - ReferenceTorqueCommand
  - DriverDemandTorqueCommand
  - EnginePercentTorqueDataCommand
  - EnginePower
  - FuelPressureCommand
  - FuelRailPressureCommand
  - FuelRailGaugePressureCommand
  - FuelRailAbsolutePressureCommand
  - EthanolFuelPercentCommand
  - AbsoluteLoadValueCommand
  - WarmUpsSinceCodesClearedCommand
  - RelativeThrottlePositionCommand
  - AbsoluteThrottlePositionCommand
  - FuelInjectionTimingCommand
  - EngineRunTimeCommand
  - ObdStandardsCommand
  - ObdStandards
  - CalibrationVerificationNumberCommand
  - EcuNameCommand
  - InUsePerformanceTrackingCommand
  - OnBoardTestCommand
  - OnBoardTestSupportedMidsCommand
  - OnBoardTestResult
  - UnitAndScaling
  - MonitorIds
  - mode 06
  - oxygen sensor
  - lambda
  - wideband
  - egr
  - evap
  - engine torque
  - horsepower
  - OdometerCommand
  - MassAirFlowCommand
  - EngineFuelRateCommand
  - HybridBatteryLifeCommand
  - vehicle speed
  - engine rpm
  - coolant temperature
  - throttle position
  - vin command
  - trouble codes
  - diagnostic trouble codes
  - dtc
  - check engine light
  - clear codes
  - supported pids
  - fuel trim
  - odometer
  - fuel type
  - hybrid battery
  - MonitorStatusDecoder
  - MonitorReadiness
  - EmissionMonitor
  - IgnitionType
  - FuelSystemStatusCommand
  - FuelSystemState
  - FreezeFrameCommands
  - AsFreezeFrame
  - CalibrationIdCommand
  - AcceleratorPedalPositionCommand
  - TimingAdvanceCommand
  - BarometricPressureCommand
  - IntakeManifoldPressureCommand
  - AmbientAirTemperatureCommand
  - freeze frame
  - readiness monitors
  - emissions inspection
  - emissions readiness
  - fuel system status
  - closed loop
  - open loop
  - accelerator pedal position
  - timing advance
  - manifold pressure
  - barometric pressure
  - ambient air temperature
  - calibration id
  - ecu tune
  - IVinDecoder
  - VinVehicle
  - VinNumber
  - VpicVinDecoder
  - AddVinDecoder
  - vin decode
  - vin decoder
  - decode vin
  - vpic
  - nhtsa
  - make model year
  - vehicle lookup
  - vin check digit
  - CalculateCheckDigit
  - IsCheckDigitValid
  - validate vin
  - generate vin
  - test vin
  - fake vin
  - EmulatedVehicle
  - VehicleCatalog
  - emulator vehicle
  - Shiny.Obd.Emulator
  - AddObdEmulator
  - AddObdEmulatorBluetoothLE
  - ObdEmulatorHost
  - ObdEmulatorState
  - ObdEmulatorConfiguration
  - IObdEmulatorTransport
  - IObdEmulatorDispatcher
  - TcpObdServer
  - Elm327Responder
  - DrivingScenarioPlayer
  - obd emulator
  - protocol pinning
  - RefreshNegotiatedProtocol
  - ATSP
  - AutoConnect
  - fake obd adapter
  - test without a car
  - obd ble
  - BleObdTransport
  - obd adapter
  - obd scan
  - device scanner
  - obd adapter not found
  - ble scan not finding device
  - adapter not found on first connect
  - unnamed ble device
  - adapter profile
  - Shiny.Obd
  - AddShinyObdBluetoothLE
---

# Shiny.Obd Skill

You are an expert in Shiny.Obd, a .NET library for communicating with vehicles through OBD-II adapters. It uses a command-object pattern with generic return types, pluggable transports (BLE, WiFi, serial), and adapter auto-detection for ELM327 and OBDLink (STN) adapters.

## When to Use This Skill

Invoke this skill when the user wants to:
- Read vehicle data (speed, RPM, coolant temp, VIN, etc.) through OBD-II
- Create custom OBD commands with typed return values
- Connect to an OBD-II adapter over Bluetooth LE, WiFi or serial (USB/UART)
- Scan for / discover available OBD adapters
- Configure ELM327 or OBDLink adapter initialization
- Implement a custom transport (Android USB Host, J2534, a replay harness) for OBD communication
- Send raw AT commands to an OBD adapter
- Handle OBD response parsing and error handling
- Build a MAUI app with OBD integration

## Library Overview

- **Repository**: https://github.com/shinyorg/obd
- **Namespaces**: `Shiny.Obd`, `Shiny.Obd.Ble`, `Shiny.Obd.Wifi`, `Shiny.Obd.Serial`, `Shiny.Obd.Commands`, `Shiny.Obd.Emulator`
- **NuGet**: `Shiny.Obd` (core), `Shiny.Obd.Ble` (BLE), `Shiny.Obd.Wifi` (WiFi/TCP), `Shiny.Obd.Serial` (USB/UART), `Shiny.Obd.Emulator` (+ `.Ble`) — be an adapter instead of reading one
- **Targets**: `net10.0` throughout

## Core Types

### IObdCommand<T> — Command interface

Every OBD command implements this. `T` is the parsed result type.

```csharp
public interface IObdCommand<T>
{
    string RawCommand { get; }
    T Parse(byte[] data);
}
```

### ObdCommand<T> — Base class for standard Mode/PID commands

Validates mode+PID response header, strips it, and delegates to `ParseData`.

```csharp
public abstract class ObdCommand<T> : IObdCommand<T>
{
    protected ObdCommand(byte mode, byte pid);
    public byte Mode { get; }
    public byte Pid { get; }
    public virtual string RawCommand { get; } // "{Mode:X2}{Pid:X2}"
    protected abstract T ParseData(byte[] data); // data after header
}
```

### IObdConnection — Connection interface

```csharp
public interface IObdConnection : IAsyncDisposable
{
    bool IsConnected { get; }
    Task Connect(CancellationToken ct = default);
    Task Disconnect();
    Task<T> Execute<T>(IObdCommand<T> command, CancellationToken ct = default);
    Task<string> SendRaw(string command, CancellationToken ct = default);
}
```

### IObdTransport — Transport abstraction

```csharp
public interface IObdTransport : IAsyncDisposable
{
    bool IsConnected { get; }
    Task Connect(CancellationToken ct = default);
    Task Disconnect();
    Task<string> Send(string command, CancellationToken ct = default);
}
```

Three implementations ship: `BleObdTransport` (`Shiny.Obd.Ble`), `WifiObdTransport`
(`Shiny.Obd.Wifi`) and `SerialObdTransport` (`Shiny.Obd.Serial`). All three also implement
`IDisposable` so a container can tear them down on its synchronous path.

### IObdDeviceScanner — Device discovery

```csharp
public interface IObdDeviceScanner
{
    Task Scan(Action<ObdDiscoveredDevice> onDeviceFound, CancellationToken ct = default);
}
```

Cancel the token to stop scanning. Each discovered device invokes the callback.

### ObdDiscoveredDevice — Discovered adapter

```csharp
public class ObdDiscoveredDevice
{
    public string Name { get; }        // e.g. "OBDLink MX+"
    public string Id { get; }          // unique identifier (BLE UUID, IP, etc.)
    public object NativeDevice { get; } // IPeripheral for BLE, IPEndPoint for WiFi, etc.
}
```

### ObdConnection — ELM327 protocol handler

Two constructors:
- `ObdConnection(IObdTransport transport)` — auto-detects adapter via ATI
- `ObdConnection(IObdTransport transport, IObdAdapterProfile profile)` — uses explicit profile, skips detection

Properties:
- `DetectedAdapter` — `ObdAdapterInfo?` with `RawIdentifier` (string) and `Type` (ObdAdapterType enum: Unknown, Elm327, ObdLink). Null when explicit profile used.
- `Protocol` — `string?`, settable before `Connect`. The ELM protocol number to pin with `ATSP` instead of searching. Ignored when an explicit profile was supplied — pass it to the profile's constructor instead.
- `NegotiatedProtocol` — `string?`, the protocol the adapter reports it is on. Refreshed by `Connect` and by `RefreshNegotiatedProtocol()`.

Methods:
- `RefreshNegotiatedProtocol(CancellationToken)` — re-reads `ATDPN` and updates `NegotiatedProtocol`.

**Always pin the protocol on reconnect.** `ATSP0` does not choose a protocol — it defers the choice to
the first command that needs the bus, and that command pays the whole ELM search: seconds of it,
routinely longer than a command timeout. `ATZ` discards the result, so an unpinned adapter pays it
again on every reconnect. Generate this pattern whenever an app reconnects to a remembered adapter:

```csharp
var connection = new ObdConnection(transport) { Protocol = savedProtocol };   // null on a first run
await connection.Connect();

// ⚠️ Ask AFTER something has needed the bus. ATSP0 has chosen nothing at the end of Connect, so
// asking there reports null and the app never learns a number to save.
await connection.Execute(new SupportedPidsCommand(0x00));
savedProtocol = await connection.RefreshNegotiatedProtocol();
```

A stale pin is safe — it is verified with mode 01 during initialization and dropped for a search when
nothing answers.

Handles:
- ELM327 hex response parsing (single-line and multi-frame CAN)
- Multi-frame framing: discards the leading byte-count line and the `N:` frame index, then concatenates frames in order
- Spaced and unspaced hex alike (`0: 49 02 01` and `0:490201`)
- Error detection: "NO DATA", "UNABLE TO CONNECT", "BUS INIT: ...ERROR", "?"
- Strips "SEARCHING..." and "BUS INIT" prefixes

### IObdAdapterProfile — Adapter initialization

```csharp
public interface IObdAdapterProfile
{
    string Name { get; }
    Task Initialize(IObdConnection connection, CancellationToken ct = default);
}
```

Built-in profiles:
- `Elm327AdapterProfile(string? protocol = null)` — ATZ, ATE0, ATL0, ATS1, ATH0, then `ATSP{protocol}` or `ATSP0`
- `ObdLinkAdapterProfile(string? protocol = null)` — STFAC, then the Elm327 sequence, then ATCAF1

`ATZ` is sent **once** per connect, by the profile. Auto-detection probes with `ATI` alone and does not
reset first. `STFAC` restores factory defaults, so it precedes the ELM327 configuration rather than
following it.

## Standard Commands (StandardCommands static class)

| Property | Type | Command | Return | Parse Formula |
|----------|------|---------|--------|---------------|
| `VehicleSpeed` | `VehicleSpeedCommand` | `010D` | `int` (km/h) | `A` |
| `EngineRpm` | `EngineRpmCommand` | `010C` | `int` (RPM) | `((A*256)+B)/4` |
| `CoolantTemperature` | `CoolantTemperatureCommand` | `0105` | `int` (°C) | `A-40` |
| `ThrottlePosition` | `ThrottlePositionCommand` | `0111` | `double` (%) | `(A*100)/255` |
| `FuelLevel` | `FuelLevelCommand` | `012F` | `double` (%) | `(A*100)/255` |
| `CalculatedEngineLoad` | `CalculatedEngineLoadCommand` | `0104` | `double` (%) | `(A*100)/255` |
| `IntakeAirTemperature` | `IntakeAirTemperatureCommand` | `010F` | `int` (°C) | `A-40` |
| `RuntimeSinceStart` | `RuntimeSinceStartCommand` | `011F` | `TimeSpan` | `(A*256)+B` seconds |
| `Vin` | `VinCommand` | `0902` | `string` | skip count byte, ASCII decode |
| `Odometer` | `OdometerCommand` | `01A6` | `double` (km) | `((A<<24)\|(B<<16)\|(C<<8)\|D)/10` |
| `DistanceSinceCodesCleared` | `DistanceSinceCodesClearedCommand` | `0131` | `int` (km) | `(A*256)+B` |
| `ControlModuleVoltage` | `ControlModuleVoltageCommand` | `0142` | `double` (V) | `((A*256)+B)/1000` |
| `MassAirFlow` | `MassAirFlowCommand` | `0110` | `double` (g/s) | `((A*256)+B)/100` |
| `EngineFuelRate` | `EngineFuelRateCommand` | `015E` | `double` (L/h) | `((A*256)+B)/20` |
| `EngineOilTemperature` | `EngineOilTemperatureCommand` | `015C` | `int` (°C) | `A-40` |
| `FuelType` | `FuelTypeCommand` | `0151` | `byte` | `A` (J1979 code) |
| `HybridBatteryLife` | `HybridBatteryLifeCommand` | `015B` | `double` (%) | `(A*100)/255` |
| `MonitorStatus` | `MonitorStatusCommand` | `0101` | `MonitorStatus` | see readiness section below |
| `MonitorStatusThisDriveCycle` | `MonitorStatusThisDriveCycleCommand` | `0141` | `MonitorStatus` | same layout; byte A reserved |
| `FuelSystemStatus` | `FuelSystemStatusCommand` | `0103` | `FuelSystemStatus` | enumerated loop state, `A` and optional `B` |
| `IntakeManifoldPressure` | `IntakeManifoldPressureCommand` | `010B` | `int` (kPa) | `A` |
| `BarometricPressure` | `BarometricPressureCommand` | `0133` | `int` (kPa) | `A` |
| `TimingAdvance` | `TimingAdvanceCommand` | `010E` | `double` (° BTDC) | `(A/2)-64` |
| `AmbientAirTemperature` | `AmbientAirTemperatureCommand` | `0146` | `int` (°C) | `A-40` |
| `RelativeAcceleratorPedalPosition` | `RelativeAcceleratorPedalPositionCommand` | `015A` | `double` (%) | `(A*100)/255` |
| `CommandedThrottleActuator` | `CommandedThrottleActuatorCommand` | `014C` | `double` (%) | `(A*100)/255` |
| `DistanceWithMilOn` | `DistanceWithMilOnCommand` | `0121` | `int` (km) | `(A*256)+B` |
| `TimeRunWithMilOn` | `TimeRunWithMilOnCommand` | `014D` | `TimeSpan` | `(A*256)+B` **minutes** |
| `TimeSinceCodesCleared` | `TimeSinceCodesClearedCommand` | `014E` | `TimeSpan` | `(A*256)+B` **minutes** |
| `CalibrationId` | `CalibrationIdCommand` | `0904` | `IReadOnlyList<string>` | 16-byte ASCII blocks, null-padded |
| `CommandedAirFuelRatio` | `CommandedAirFuelRatioCommand` | `0144` | `double` (lambda) | `2/65536*((A*256)+B)` |
| `CommandedEgr` | `CommandedEgrCommand` | `012C` | `double` (%) | `(A*100)/255` |
| `EgrError` | `EgrErrorCommand` | `012D` | `double` (%) | `(A*100/128)-100` |
| `CommandedEvaporativePurge` | `CommandedEvaporativePurgeCommand` | `012E` | `double` (%) | `(A*100)/255` |
| `EvapVaporPressure` | `EvapVaporPressureCommand` | `0132` | `double` (Pa) | **signed** `((A*256)+B)/4` |
| `AbsoluteEvapVaporPressure` | `AbsoluteEvapVaporPressureCommand` | `0153` | `double` (kPa) | `((A*256)+B)/200` |
| `EvapVaporPressureWideRange` | `EvapVaporPressureWideRangeCommand` | `0154` | `double` (Pa) | **signed** `(A*256)+B` |
| `DriverDemandTorque` | `DriverDemandTorqueCommand` | `0161` | `int` (%) | `A-125` |
| `ActualEngineTorque` | `ActualEngineTorqueCommand` | `0162` | `int` (%) | `A-125` |
| `ReferenceTorque` | `ReferenceTorqueCommand` | `0163` | `int` (N·m) | `(A*256)+B` |
| `EnginePercentTorqueData` | `EnginePercentTorqueDataCommand` | `0164` | `EnginePercentTorqueData` | five points, each `X-125` |
| `FuelPressure` | `FuelPressureCommand` | `010A` | `int` (kPa) | `A*3` |
| `FuelRailPressure` | `FuelRailPressureCommand` | `0122` | `double` (kPa) | `0.079*((A*256)+B)` |
| `FuelRailGaugePressure` | `FuelRailGaugePressureCommand` | `0123` | `int` (kPa) | `10*((A*256)+B)` |
| `FuelRailAbsolutePressure` | `FuelRailAbsolutePressureCommand` | `0159` | `int` (kPa) | `10*((A*256)+B)` |
| `EthanolFuelPercent` | `EthanolFuelPercentCommand` | `0152` | `double` (%) | `(A*100)/255` |
| `AbsoluteLoadValue` | `AbsoluteLoadValueCommand` | `0143` | `double` (%) | `((A*256)+B)*100/255` — **not capped at 100** |
| `WarmUpsSinceCodesCleared` | `WarmUpsSinceCodesClearedCommand` | `0130` | `int` (count) | `A` |
| `RelativeThrottlePosition` | `RelativeThrottlePositionCommand` | `0145` | `double` (%) | `(A*100)/255` |
| `FuelInjectionTiming` | `FuelInjectionTimingCommand` | `015D` | `double` (°) | `(((A*256)+B)/128)-210` |
| `EngineRunTime` | `EngineRunTimeCommand` | `017F` | `EngineRunTime` | support byte, then 3 x 4-byte second counters |
| `ObdStandards` | `ObdStandardsCommand` | `011C` | `byte` | raw J1979 code; name via `ObdStandards.Describe` |
| `CalibrationVerificationNumber` | `CalibrationVerificationNumberCommand` | `0906` | `IReadOnlyList<string>` | 4-byte blocks as **hex strings** |
| `EcuName` | `EcuNameCommand` | `090A` | `string` | 20-byte ASCII, null-padded |

⚠️ PIDs `4D`/`4E` are **minutes**; `RuntimeSinceStart` (`011F`) is **seconds**. Do not copy one
formula onto the other.

⚠️ Do not confuse the four position PIDs. `ThrottlePosition` (`0111`) is *absolute throttle plate*
position, the drive-by-wire **output**, and it carries a closed-pedal floor of 12-18% that varies by
vehicle. `AcceleratorPedalPositionCommand` (`0149`/`014A`/`014B`) and
`RelativeAcceleratorPedalPosition` (`015A`) are the driver's **input**, and `015A` is already
normalised so a released pedal reads 0. When measuring how hard a vehicle is being driven, prefer
`015A`, then a pedal sensor, and only fall back to `0111` — never assume `0111` bottoms out at zero.

⚠️ `AmbientAirTemperature` (`0146`) is the air outside; `IntakeAirTemperature` (`010F`) is what the
engine is breathing, measured after the engine bay has warmed it. They are not interchangeable.

⚠️ `RelativeThrottlePosition` (`0145`) is the one to put on a UI — it is referenced to the learned
closed stop and reads 0% at rest, where `ThrottlePosition` (`0111`) never does.

⚠️ `AbsoluteLoadValue` (`0143`) is **not** capped at 100% — a boosted engine reads well above it, up to
~400%. Never clamp it, and never substitute it for `CalculatedEngineLoad` (`0104`) or vice versa.

⚠️ The two signed EVAP pressure PIDs (`0132`, `0154`) must be read as two's complement. Reading them
unsigned turns every vacuum — the entire point of the measurement — into a large positive pressure.

## Commands not on StandardCommands

These need construction data or carry their own shared instances, so never write
`StandardCommands.FuelTrim` or `StandardCommands.SupportedPids` — they do not exist.

| Command | Construct with | Return | Notes |
|---------|----------------|--------|-------|
| `FuelTrimCommand` | `FuelTrimCommand.ShortTermBank1()` / `.LongTermBank1()` / `.ShortTermBank2()` / `.LongTermBank2()` (PIDs 06/07/08/09), or `new FuelTrimCommand(pid)` | `double` (%) | `(A*100/128)-100`. 128 is zero correction; positive = ECU adding fuel |
| `SupportedPidsCommand` | `new SupportedPidsCommand(block)` for each of `SupportedPidsCommand.BlockPids` | `IReadOnlyList<byte>` | The 32 PIDs following the block PID that the vehicle answers |
| `AcceleratorPedalPositionCommand` | `AcceleratorPedalPositionCommand.D()` / `.E()` / `.F()` (PIDs 49/4A/4B), or `new AcceleratorPedalPositionCommand(pid)` | `double` (%) | `(A*100)/255`. Redundant sensors on one pedal; most vehicles report D and E |
| `DtcReadCommand` | `DtcReadCommand.Stored` (03) / `.Pending` (07) / `.Permanent` (0A) | `IReadOnlyList<string>` | SAE J2012 strings, e.g. `"P0301"` |
| `ClearDtcCommand` | `ClearDtcCommand.Instance` (04) | `bool` | True when the ECU answers `0x44` |
| mode 02 readings | `someModeOneCommand.AsFreezeFrame(frame)` | same as the mode 01 command | Never write a separate mode 02 command class |
| `FreezeFrameCommands.CausalDtc(frame)` | static factory | `string?` | The code that stored the frame; null = no snapshot |
| `OxygenSensorsPresentCommand` | `.TwoBanks()` (PID 13) / `.FourBanks()` (PID 1D) | `OxygenSensorLayout` | **Read before any sensor PID** — it decides what they mean |
| `OxygenSensorVoltageCommand` | `.Sensor(1..8)` (PIDs 14-1B) | `OxygenSensorVoltage` | Narrowband. `A/200` V; trim `100/128*B-100`, null when `B == 0xFF` |
| `OxygenSensorLambdaCommand` | `.WithVoltage(1..8)` (24-2B) / `.WithCurrent(1..8)` (34-3B) | `OxygenSensorLambda` | Wideband. Lambda `2/65536*(256A+B)` |
| `AbsoluteThrottlePositionCommand` | `.B()` (47) / `.C()` (48) | `double` (%) | Redundant drive-by-wire sensors; disagreement sets correlation codes |
| `InUsePerformanceTrackingCommand` | `.Spark()` (09 08) / `.Compression()` (09 0B) | `InUsePerformanceTracking` | Pick by engine type; the wrong one returns NO DATA |
| `OnBoardTestCommand` | `new OnBoardTestCommand(mid)` | `IReadOnlyList<OnBoardTestResult>` | Mode 06. One MID commonly answers several records |
| `OnBoardTestSupportedMidsCommand` | `new OnBoardTestSupportedMidsCommand(block)` for each of `MonitorIds.BlockMids` | `IReadOnlyList<byte>` | Mode 06 discovery |

### Oxygen sensors — always read the layout first

**Never generate a per-sensor read without first reading `OxygenSensorsPresentCommand`.** This is not
about skipping absent sensors. A vehicle answers PID `0x13` (two banks of four) **or** PID `0x1D`
(four banks of two), never both, and the choice changes what every sensor PID means:

- Under `0x13`, PID `0x16` is bank 1, sensor 3.
- Under `0x1D`, PID `0x16` is bank 2, sensor 1.

Labelling from the wrong layout sends someone to the wrong side of the engine. Use
`layout.Position(sensorIndex)` for the label — never compute bank/sensor arithmetic inline.

```csharp
var layout = await connection.Execute(OxygenSensorsPresentCommand.TwoBanks());
foreach (var sensor in layout.Sensors)
{
    var reading = await connection.Execute(OxygenSensorVoltageCommand.Sensor(sensor.SensorIndex));
    Console.WriteLine($"{sensor}: {reading.Volts:F3} V");     // sensor.ToString() is "B1S2"
}
```

Narrowband (`OxygenSensorVoltageCommand`, 14-1B) and wideband (`OxygenSensorLambdaCommand`, 24-2B or
34-3B) are different measurements. **Do not compare their voltages** and do not offer both for the
same sensor without probing `SupportedPidsCommand`.

`ShortTermFuelTrim` is null when the vehicle sets `0xFF` (sensor not used in trim). Never coerce that
to 0 or to 99.2 — it is an absence.

When generating diagnostic guidance: a healthy upstream narrowband oscillates ~0.1-0.9 V several times
a second when hot (a mid-range reading means a lazy sensor, **not** a perfect mixture); a downstream
sensor sits steady ~0.6-0.7 V and mirroring the upstream swing means the catalyst is spent. Always
sample over seconds — never diagnose from one reading.

### EGR and EVAP

Generate `CommandedEgr` and `EgrError` **together**. Commanded alone says only what was asked for.

The three EVAP pressure PIDs are **not interchangeable** and must never be converted between:

| PID | Command | Unit | Reference |
|-----|---------|------|-----------|
| 32 | `EvapVaporPressure` | signed Pa, ±8 kPa | atmosphere |
| 54 | `EvapVaporPressureWideRange` | signed Pa, ±32 kPa | atmosphere |
| 53 | `AbsoluteEvapVaporPressure` | unsigned kPa | vacuum — ~101 kPa is atmospheric |

Probe with `SupportedPidsCommand` and use whichever the vehicle answers.

### Torque and power

Torque PIDs are percentages of a reference figure — **neither is meaningful alone**. Generate a read of
`ReferenceTorque` **once**, outside any poll loop (it is a constant for the engine), then use
`EnginePower`:

```csharp
var reference = await connection.Execute(StandardCommands.ReferenceTorque);   // once
// per sample:
var kw = EnginePower.Kilowatts(
    await connection.Execute(StandardCommands.ActualEngineTorque),
    reference,
    await connection.Execute(StandardCommands.EngineRpm)
);
```

Offer `MetricHorsepower` (PS, 735.5 W) or `MechanicalHorsepower` (hp, 745.7 W) explicitly — never
write a bare "horsepower" conversion, since the two differ by ~1.4% and silently disagreeing about a
car's output is worse than asking. Negative torque is normal (overrun). This is flywheel output, not
wheel output — never present it as a dyno figure.

### Mode 06 — on-board test results

The only mode that answers "how close is this to failing". Generate discovery first — there are 224
MIDs and an unsupported one returns NO DATA:

```csharp
foreach (var block in MonitorIds.BlockMids)          // 00, 20, 40, 60, 80, A0
{
    foreach (var mid in await connection.Execute(new OnBoardTestSupportedMidsCommand(block)))
    {
        foreach (var test in await connection.Execute(new OnBoardTestCommand(mid)))
        {
            if (test.Passed == false)
                Console.WriteLine($"{test.Monitor}: {test.Value} {test.Unit} outside {test.Minimum}-{test.Maximum}");
        }
    }
}
```

Rules:

1. **Never decode a mode 06 value without `UnitAndScaling`.** The same 16-bit number is 0.25 rpm/bit
   under one identifier and 0.122 mV under another. `OnBoardTestResult.Value` already applies it.
2. **Never treat a raw value as unsigned by default.** Identifiers `0x80` and above are signed; reading
   one unsigned turns a small negative into ~65,535 and a passing test into a dramatic failure.
3. `Value`, `Unit`, `Passed` and `BandPosition` are **null** when the identifier is outside the standard
   table. Surface that as unknown; `RawValue`/`RawMinimum`/`RawMaximum` are still there.
4. `BandPosition` (0 at the lower limit, 1 at the upper) is the feature worth building on — a passing
   test at 0.95 trended over time is a component you can schedule. Prefer it over a bare pass/fail UI.
5. `Monitor` is null for MIDs above `0xDF` (manufacturer-defined). Do not invent a name.
6. Mode 06 is CAN-only here. On a pre-CAN vehicle it throws `ObdException` naming that as the cause.

### ECU identity and in-use performance

`CalibrationId` (09 04) and `CalibrationVerificationNumber` (09 06) are a **pair** — generate both when
the goal is tune/reflash detection. The CVN is computed over the calibration itself, so a reflash that
keeps the same ID still changes it. CVNs are returned as uppercase hex strings; never parse them to a
number (leading zeroes are significant and they have no arithmetic meaning).

`InUsePerformanceTrackingCommand.Spark()` / `.Compression()` — pick by engine type
(`MonitorStatus.Ignition` or `ObdStandards.IsHeavyDuty` tells you which). `Ratio` is null when the
denominator is zero: "never had the opportunity" is a different finding from "had the opportunity and
never ran", and only the second is a vehicle problem.

### Reading a J1979 fuel type

`FuelTypeCommand` returns a raw code; `FuelTypes.Describe` names it. It answers **null** for `0x00`
("not available") and for anything outside the table — never the string "Unknown", because a caller
storing or displaying this has to be able to tell an absent answer from a claim about the vehicle.
Read it once per connection, not on a poll: a vehicle does not change what it burns.

```csharp
var fuelType = FuelTypes.Describe(await connection.Execute(StandardCommands.FuelType));
```

### Probing supported PIDs first

Querying an unsupported PID returns NO DATA, so read the bitmask blocks once per connection and
only poll what the vehicle answers. An unsupported reading must surface as **missing, not zero** —
the odometer PID is absent on most vehicles and hybrid battery life on every vehicle without a pack,
and a zero there is indistinguishable from a dead pack.

```csharp
var supported = new HashSet<byte>();
foreach (var block in SupportedPidsCommand.BlockPids)   // 00, 20, 40, 60, 80, A0, C0
{
    foreach (var pid in await connection.Execute(new SupportedPidsCommand(block)))
        supported.Add(pid);
}
```

### Reading and clearing trouble codes

```csharp
var stored = await connection.Execute(DtcReadCommand.Stored);        // mode 03 — turn the MIL on
var pending = await connection.Execute(DtcReadCommand.Pending);      // mode 07
var permanent = await connection.Execute(DtcReadCommand.Permanent);  // mode 0A — ECU clears these
var cleared = await connection.Execute(ClearDtcCommand.Instance);    // mode 04
```

Modes 03/07/0A/04 carry **no PID**, so these implement `IObdCommand<T>` directly rather than
extending `ObdCommand<T>` — follow that pattern for any other PID-less mode. `DtcDecoder` is the
public parser behind them: it strips the mode echo, then uses payload **parity** to tell a CAN
reply (`43 <count> <pairs>`) from a pre-CAN one (`43 <pairs>`) rather than assuming a transport.

⚠️ Mode 04 also resets the emissions readiness monitors, which then take several drive cycles to
re-run and will fail an emissions test in the meantime. Always put it behind an explicit user
confirmation that says so — never call it as part of a connect or refresh routine.

### Emissions monitor readiness

`MonitorStatus` decodes all four bytes of PID 0x01, not just the lamp. `MonitorStatusDecoder` is the
public parser and is testable without a transport.

```csharp
var status = await connection.Execute(StandardCommands.MonitorStatus);
if (status.IsReadyForInspection == false)
    Report(status.Incomplete.Select(x => x.Monitor));
```

Rules that matter when generating code against this:

- `Monitors` contains **only the monitors the vehicle supports**. Do not write code that treats a
  missing monitor as incomplete — a monitor that does not exist on a car has no readiness state.
- The completion bits are **inverted** in the raw bytes (a set bit means the test is still running).
  `MonitorReadiness.Complete` has already flipped it; never flip it again.
- Which monitors bytes C/D describe depends on `IgnitionType`, taken from bit 3 of byte B. The same
  bit means different monitors on a petrol and a diesel — the decoder handles it, so never index the
  bytes yourself.
- `EmissionMonitor.GasolineParticulateFilter` is that bit's meaning under ISO 15031-5:2015 and
  later; earlier revisions defined it as A/C refrigerant monitoring.
- A vehicle reads not-ready for several drive cycles after codes are cleared with nothing wrong with
  it. Pair it with `TimeSinceCodesCleared` (PID 0x4E) before telling a user their car has a problem.
- `MonitorStatusThisDriveCycle` (PID 0x41) shares the type, but byte A is reserved — its `MilOn` and
  `DtcCount` are always false/0 and must not be surfaced.

### Freeze frames (mode 02)

Mode 02 accepts the same PIDs as mode 01 and scales them identically. **Never write a separate mode
02 command class** — call `AsFreezeFrame()` on the mode 01 command.

```csharp
var causal = await connection.Execute(FreezeFrameCommands.CausalDtc());
if (causal != null)
{
    var rpm = await connection.Execute(StandardCommands.EngineRpm.AsFreezeFrame());
    var load = await connection.Execute(StandardCommands.CalculatedEngineLoad.AsFreezeFrame());
}
```

⚠️ **Always gate mode 02 reads on `CausalDtc` returning non-null.** When there is no stored snapshot
the frame is zero-filled, so an engine load of 0% and a coolant temperature of -40 °C come back
looking like measurements rather than like an absence.

The mode 02 header is **three bytes** (`42 <PID> <frame>`), not two, because the frame number is
echoed. `AsFreezeFrame` throws `ObdException` on a non-mode-01 command — mode 09 identifiers are not
sampled at a moment, so there is no frame to ask for.

## VIN Decoding (Shiny.Obd.Vin)

Registration, and the only two forms that exist:

```csharp
services.AddVinDecoder();                        // built-in NHTSA vPIC
services.AddVinDecoder<MyRegistryVinDecoder>();  // your own IVinDecoder
```

`AddVinDecoder` also calls `AddHttpClient()` and registers with `TryAddSingleton`, so the first
registration wins. Do not hand-register `VpicVinDecoder`; do not resolve it by concrete type.

```csharp
var vin = await connection.Execute(StandardCommands.Vin);   // mode 09 PID 02
var vehicle = await vinDecoder.Decode(vin);                 // null when it cannot be identified
```

`VinVehicle` (provider-neutral, every field nullable):

| Property | Type | Bounds |
|----------|------|--------|
| `Make` / `Model` / `Trim` | `string?` | |
| `ModelYear` | `int?` | 1900-2100 |
| `FuelType` / `Electrification` | `string?` | |
| `EngineCylinders` | `int?` | 1-16 |
| `EngineDisplacementLitres` | `double?` | 0-20 |
| `EngineHorsepower` | `int?` | 1-2000 |
| `DriveType` / `BodyClass` / `TransmissionStyle` | `string?` | |
| `IsUsable` | `bool` | make or model identified |

Rules that matter when generating code against this:

- **Never re-parse or re-clean the result.** The values arrive typed, bounded and with the
  registries' `"Not Applicable"` / `"Not Available"` / `"N/A"` placeholders already stripped to null.
  Writing a second cleaning pass in the consumer is the mistake this shape exists to prevent.
- **Null means the registry had nothing.** Never substitute `"Unknown"` or `0` — these values reach
  users and AI prompts, where a placeholder reads as a fact about the car.
- **`IVinDecoder` never throws**, by contract. Do not wrap calls in try/catch for control flow, and
  any implementation you write must return null rather than throwing or guessing.
- **Coverage falls off outside North America.** A decode with a make and model and nothing else is a
  success, not a failure — do not treat a missing displacement as an error.
- `VinNumber.IsPlausible` / `Normalize` are the pure pre-check (17 chars, no I/O/Q). `Decode` applies
  them itself, so only call them directly when you need to know *why* nothing came back.
- **The check digit is a separate, opt-in pair — never add it to a read path.**
  `VinNumber.CalculateCheckDigit(vin)` returns the position-9 character (`'0'`–`'9'` or `'X'`, or null
  when the input is implausible), and `IsCheckDigitValid(vin)` compares it to what is there. It is
  deliberately outside `IsPlausible` because the check digit is only mandatory in North America, so
  plenty of legitimate European and Asian VINs fail it. Use it for **user-typed** VINs (catching a
  transposition) and for **generating** VINs that a decoder will accept — never to reject a VIN read
  off a vehicle.

  ```csharp
  // a VIN for a test fixture: real WMI + descriptor, computed check digit
  var vin = "3VWRA7AU" + VinNumber.CalculateCheckDigit("3VWRA7AU0FM024518") + "FM024518";
  ```
- **Mode 01 PID 0x51 outranks the registry for fuel type.** `FuelTypes.Describe` off the bus needs no
  network and is the more trustworthy source on a rebadged or grey-import vehicle:
  `var fuel = fromBus ?? vehicle?.FuelType;`

## BLE Transport (Shiny.Obd.Ble)

### BleObdConfiguration

```csharp
public class BleObdConfiguration
{
    public string ServiceUuid { get; set; } = "FFF0";
    public string ReadCharacteristicUuid { get; set; } = "FFF1";
    public string WriteCharacteristicUuid { get; set; } = "FFF2";
    public string? DeviceNameFilter { get; set; }
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool AutoConnect { get; set; }   // default false
}
```

**Leave `AutoConnect` off** for an adapter somebody is waiting on, which is the normal OBD case. On
Android it selects `ConnectGatt(autoConnect: true)`, the background connection path, where the
controller only attempts during widely spaced scan windows — tens of seconds for an adapter a direct
connect reaches in a few hundred milliseconds. It also arms the platform's own reconnect, which races
any caller supervising the session itself. Turn it on only when nothing in the app is doing that job.

The transport reads the write characteristic's advertised properties on connect and writes with or
without a GATT response accordingly; do not assume either.

### BleObdTransport

Three constructors:
- `BleObdTransport(IBleManager bleManager, BleObdConfiguration config)` — scans for adapter
- `BleObdTransport(IPeripheral peripheral, BleObdConfiguration config)` — uses pre-discovered peripheral
- `BleObdTransport(ObdDiscoveredDevice device, BleObdConfiguration config)` — uses device from scanner

Uses Shiny.BluetoothLE v4 APIs:
- `ConnectAsync` for task-based connection
- `NotifyCharacteristic` for RX notifications
- `WriteCharacteristicAsync` for TX writes
- Collects notification bytes until `>` prompt, returns complete response

An exchange that hits `CommandTimeout` throws `ObdTimeoutException` and is then closed off — a late
reply arriving afterwards is discarded rather than completing the next command's wait. `Disconnect()`
fails an in-flight command straight away instead of making the caller wait out the timeout.

### BLE scanning rules

Three rules the library already follows internally. Generated code that scans with `IBleManager`
directly must follow them too, or it will not find adapters on iOS.

**1. Never match on `IPeripheral.Name` alone.** On iOS `CBPeripheral.Name` is null while scanning a
peripheral that has never been connected to — the name is only in the advertisement. Always fall
back:

```csharp
var name = scanResult.Peripheral.Name ?? scanResult.AdvertisementData?.LocalName;
```

**2. Never pass the adapter's `ServiceUuid` as a scan filter.** iOS matches a scan filter against the
*advertisement*, and most ELM327 clones don't advertise their GATT service — it appears only after
connecting. `bleManager.Scan(new ScanConfig("FFF0"))` finds nothing at all on iPhone. Scan
unfiltered and use `ServiceUuid` after connecting.

**3. Never require a name, and never identify a remembered adapter by one.** Rule 1 makes the name
*better*; it does not make it reliable. Plenty of ELM327 clones advertise no name at all, and on iOS
even the fallback is empty until CoreBluetooth has connected to that peripheral once and cached it —
so `where(x => !string.IsNullOrEmpty(name))` is in practice a **first-connection-of-the-process**
filter. The symptom is distinctive and easy to misread: pairing works, the first reconnect after a
cold start fails, and every reconnect after that succeeds.

```csharp
// WRONG - drops unnamed adapters, and on iOS that is most of them on the first run
.Where(x => !string.IsNullOrEmpty(x.Name))

// RIGHT - the peripheral id is the only key always present
.Where(x => x.Peripheral.Uuid == rememberedId)
```

Persist `IPeripheral.Uuid` for a remembered adapter and match on that. Use a name filter only when
the *user* asked to narrow by name — then excluding unnamed devices is correct, because a filter
cannot match a name that isn't there. `BleObdDeviceScanner` surfaces unnamed adapters with `Name` as
an empty string, so a picker should fall back to the id or RSSI for the row label.

`BleObdDeviceScanner` logs every advertisement at `Debug` level before filtering (name,
`Peripheral.Name`, id, RSSI, advertised service UUIDs). When a user reports that an adapter isn't
found, tell them to enable debug logging (`builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Debug)`)
and read that dump rather than guessing at UUIDs.

### ObdTimeoutException

```csharp
public class ObdTimeoutException : ObdException
{
    public string Command { get; }    // the command that went unanswered
    public TimeSpan Timeout { get; }  // the deadline that elapsed
}
```

Thrown when the adapter does not answer within the transport's `CommandTimeout`. **Not** an
`OperationCanceledException` — generated polling code must be able to tell a quiet adapter apart from
its own cancellation token firing:

```csharp
try
{
    reading = await connection.Execute(StandardCommands.VehicleSpeed, ct);
}
catch (OperationCanceledException) when (ct.IsCancellationRequested)
{
    throw;                      // our own shutdown
}
catch (ObdException)
{
    reading = null;             // this PID is a write-off, the loop carries on
}
```

## Registration (DI)

```csharp
// WiFi — every platform, including iOS and Android
services.AddShinyObdWifi();

// Serial — Windows, Linux, macOS, Mac Catalyst
services.AddShinyObdSerial(config => config.PortNameFilter = "OBDLink");

// BLE — all platforms Shiny.BluetoothLE supports
services.AddShinyObdBluetoothLE();
```

Each registers `IObdTransport`, `IObdConnection`, `IObdDeviceScanner` and their configuration as
**singletons**, using `TryAdd`.

**Picking a transport when the user has not said which.** Ask what adapter they have rather than
guessing. If they name a target platform but no adapter:

| Target | Default to | Why |
|---|---|---|
| iOS | WiFi or BLE | Serial is `PlatformNotSupportedException` |
| Android | WiFi or BLE | Serial compiles and then fails at runtime — see below |
| Raspberry Pi / fleet device | Serial | Wired, no pairing, cannot wander out of range |
| Windows / macOS / Linux desktop | Serial or WiFi | Whatever adapter is on hand |

WiFi is the only transport with no platform caveats at all.

**Critical rule for non-mobile targets.** `AddShinyObdBluetoothLE()` registers the BLE manager itself
**only on iOS and Android**. On Linux, Blazor WebAssembly, Windows and Apple desktop you must
generate a `services.AddBluetoothLE()` call as well, from the platform package the app references:

```csharp
services.AddBluetoothLE();          // Shiny.BluetoothLE.Linux / .Blazor / .BluetoothLE
services.AddShinyObdBluetoothLE();  // order does not matter — DI resolves lazily
```

Never try to make Shiny.Obd.Ble do this for the user: `Shiny.BluetoothLE.Linux` and
`Shiny.BluetoothLE.Blazor` both ship `net10.0` assemblies declaring
`Shiny.AddBluetoothLE(IServiceCollection)`, so referencing both is a CS0121 ambiguity. Omitting the
call throws an `ObdException` naming the package to install.

**Never generate two of `AddShinyObdWifi()` / `AddShinyObdSerial()` / `AddShinyObdBluetoothLE()` for a
fallback chain.** `TryAdd` means the first one registered wins and the rest are silently ignored. For
a multi-transport fallback, construct the transports directly and try them in order.

## WiFi Transport (Shiny.Obd.Wifi)

A raw TCP socket to an ELM327 WiFi adapter — OBDLink MX Wi-Fi, Veepeak WiFi, Vgate iCar, ESP8266/
ESP32 clones. **Works on every platform identically**, including iOS and Android. There is no
platform package to add.

Note for users who ask about the **OBDLink MX+**: that model is Bluetooth (BLE + classic SPP), not
WiFi — generate `Shiny.Obd.Ble` for it. The WiFi model in that line is the **OBDLink MX Wi-Fi**.

### WifiObdConfiguration

```csharp
public class WifiObdConfiguration
{
    public string? Host { get; set; }                     // null = discover
    public int Port { get; set; } = 35000;
    public WifiObdEndpoint[] EndpointCandidates { get; set; }
    public bool AutoDetectEndpoint { get; set; } = true;
    public bool IncludeGatewayCandidates { get; set; } = true;
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan ProbeTimeout { get; set; } = TimeSpan.FromSeconds(2);
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(20);  // Zero disables
    public bool NoDelay { get; set; } = true;
    public Action<Socket>? ConfigureSocket { get; set; }
}

public record WifiObdEndpoint(string Host, int Port);   // ToString() => "host:port"
```

`192.168.0.10:35000` is the OBDLink/ScanTool default and what most clones copied; `192.168.4.1` is
the stock ESP8266/ESP32 SoftAP address. A minority of clones use port 23.

### WifiObdTransport

Constructors:
- `WifiObdTransport(WifiObdConfiguration config, ILogger<WifiObdTransport>? logger = null)`
- `WifiObdTransport(string host, int port = 35000, ILogger<WifiObdTransport>? logger = null)` — sets `AutoDetectEndpoint = false`
- `WifiObdTransport(ObdDiscoveredDevice device, WifiObdConfiguration config, ILogger<WifiObdTransport>? logger = null)`

`ConnectedEndpoint` (`WifiObdEndpoint?`) and `DetectedIdentifier` (`string?`) report what was reached.

### Registration

```csharp
services.AddShinyObdWifi();                          // probe for the adapter
services.AddShinyObdWifi("192.168.0.10", 35000);     // pin it, skip detection
services.AddShinyObdWifi(config => config.KeepAliveInterval = TimeSpan.Zero);
```

### Rules to apply when generating WiFi code

1. **Never claim a TCP connect means the adapter is there.** Anything listening accepts — a router on
   `192.168.0.1` completes the handshake and then says nothing. `AutoDetectEndpoint` validates with
   ATI and only accepts a `>`-terminated reply. Leave it on unless the user pins a host.
2. **Joining the WiFi network is the app's job, not the transport's.** On **Android** the adapter's
   AP has no internet, so the OS keeps the default route on cellular and the socket connects to
   nothing; generate `ConnectivityManager.BindProcessToNetwork(network)` or bind the socket via
   `ConfigureSocket`. On **iOS**, add `NSLocalNetworkUsageDescription` to `Info.plist` — a denial is
   silent and looks like a dead adapter.
3. **Never write an auto-reconnect loop around the transport.** A dropped socket loses the adapter's
   session state (`ATE0`, `ATS1`, the negotiated protocol) because that state lives in the adapter.
   Reconnecting the socket alone yields a live connection with echo back on, whose replies parse as
   garbage instead of failing. Recover by calling `ObdConnection.Connect()` again, which re-runs the
   profile.
4. **Do not set `NoDelay = false`.** An OBD exchange is a tiny write and a tiny reply — precisely the
   traffic Nagle delays. It costs tens of milliseconds on every PID read.
5. **Assume one client per adapter.** Most WiFi adapters accept exactly one TCP connection. Do not
   generate two transports against one adapter, and do not hold a scan open while connecting.
6. `KeepAliveInterval` exists because clone firmware drops sockets idle for 30–60s. Leave it on for
   an interactive app; a tight polling loop never goes idle either way.

### WifiObdDeviceScanner

Probes `Host` → each interface's default gateway → `EndpointCandidates`, reporting the ones that
answer ATI. `ObdDiscoveredDevice.Name` is the ATI identity, `Id` is `"host:port"`, `NativeDevice` is
the `WifiObdEndpoint`. Re-probes every 5s so a picker UI stays live, reporting each adapter once.

**Do not generate a subnet sweep.** It is slow, rude on a shared network, and pointless — these
adapters run their own AP on a short list of known addresses. Add unusual ones to
`EndpointCandidates` instead.

## Serial Transport (Shiny.Obd.Serial)

USB or UART. Built on `System.IO.Ports` — works on **Windows, Linux, macOS and Mac Catalyst**. Do
not tell users `System.IO.Ports` is Windows-only; that is wrong for every desktop platform.

**Never generate `Shiny.Obd.Serial` code for an Android or iOS target.** Recommend `Shiny.Obd.Ble`
there instead. iOS/tvOS throw `PlatformNotSupportedException`. Android is the trap: the assembly is
not marked unsupported and the native library ships for the `android-*` RIDs, so a `net10.0-android`
project references this package and **compiles cleanly**, then fails at runtime with
`UnauthorizedAccessException` — `/dev/ttyUSB*` is `root:usb` and unreachable from an app on an
unrooted device. Android USB adapters require the USB Host API (`UsbManager`, permission intent,
`openDevice()`, bulk transfers), which would be a separate `IObdTransport` implementation.

**Default to this transport for any fixed/unattended install** (Raspberry Pi appliance, dashcam,
fleet device). BLE is for phones and for adapters that cannot be wired. Serial has no pairing, no
scan, and no reconnect storm after a power cycle.

### SerialObdConfiguration

```csharp
public class SerialObdConfiguration
{
    public string? PortName { get; set; }              // null = discover
    public int BaudRate { get; set; } = 38400;
    public int[] BaudRateCandidates { get; set; } = [38400, 115200, 9600, 500000];
    public bool AutoDetectBaudRate { get; set; } = true;
    public string? PortNameFilter { get; set; }        // e.g. "OBDLink", "FTDI"
    public Parity Parity { get; set; } = Parity.None;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
    public Handshake Handshake { get; set; } = Handshake.None;
    public bool DtrEnable { get; set; } = true;        // most USB bridges hold reset until DTR
    public bool RtsEnable { get; set; } = true;
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan OpenSettleDelay { get; set; } = TimeSpan.FromMilliseconds(500);
}
```

### SerialObdTransport

Three constructors:
- `SerialObdTransport(SerialObdConfiguration config, ILogger<SerialObdTransport>? logger = null)`
- `SerialObdTransport(string portName, ILogger<SerialObdTransport>? logger = null)`
- `SerialObdTransport(ObdDiscoveredDevice device, SerialObdConfiguration config, ILogger? logger = null)`

Exposes `ConnectedPortName` and `ConnectedBaudRate` after connecting — worth logging when discovery
or baud probing is in play.

### Registration

```csharp
services.AddShinyObdSerial(config => config.PortNameFilter = "OBDLink");
services.AddShinyObdSerial("/dev/ttyUSB0");   // shorthand for a known port
```

Registers `IObdTransport`, `IObdConnection` and `IObdDeviceScanner` as **singletons** — an adapter is
one physical resource, and a scoped registration would have two consumers fighting over one port.

### SerialPortEnumerator / SerialObdDeviceScanner

`SerialPortEnumerator.Discover()` returns `IReadOnlyList<SerialPortInfo>`, likely-adapters first.
`SerialObdDeviceScanner` wraps it as an `IObdDeviceScanner`, re-reading on an interval so a picker UI
stays live; `ObdDiscoveredDevice.NativeDevice` is the `SerialPortInfo`.

```csharp
public record SerialPortInfo(string PortName, string Description, string? StablePath, bool IsLikelyAdapter);
```

**Always prefer `StablePath` (the Linux `/dev/serial/by-id` link) when generating config or
reconnect logic.** Numbered names like `/dev/ttyUSB0` are assigned in USB enumeration order, so
another USB serial device can take the name across a reboot.

### Linux notes to include when generating deployment code

- The service user needs the `dialout` group.
- ModemManager probes USB-serial devices and holds them open; add `ENV{ID_MM_DEVICE_IGNORE}="1"` udev
  rules for vendors `0403` (FTDI), `1a86` (CH340), `10c4` (CP210x).

## Code Generation Patterns

### Creating a custom OBD command (standard PID)

```csharp
public class BarometricPressureCommand : ObdCommand<int>
{
    public BarometricPressureCommand() : base(0x01, 0x33) { }
    protected override int ParseData(byte[] data) => data[0];
}
```

### Creating a custom OBD command (non-standard)

```csharp
public class ManufacturerCommand : IObdCommand<string>
{
    public string RawCommand => "2101";
    public string Parse(byte[] data) => BitConverter.ToString(data);
}
```

### Full connection setup with BLE

```csharp
var transport = new BleObdTransport(bleManager, new BleObdConfiguration
{
    DeviceNameFilter = "OBDLink"
});
var connection = new ObdConnection(transport);
await connection.Connect();

var speed = await connection.Execute(StandardCommands.VehicleSpeed);
var rpm = await connection.Execute(StandardCommands.EngineRpm);
var vin = await connection.Execute(StandardCommands.Vin);
```

### Scanning for devices then connecting

```csharp
var scanner = new BleObdDeviceScanner(bleManager);
var cts = new CancellationTokenSource();
ObdDiscoveredDevice? selected = null;

await scanner.Scan(device =>
{
    selected = device;
    cts.Cancel(); // stop after first device
}, cts.Token);

var transport = new BleObdTransport(selected!, new BleObdConfiguration());
var connection = new ObdConnection(transport);
await connection.Connect();
```

### MAUI DI registration

```csharp
// In MauiProgram.cs
builder.Services.AddBluetoothLE();  // Shiny BLE v4 — namespace: Shiny
builder.Services.AddShinyObdBluetoothLE(new BleObdConfiguration
{
    DeviceNameFilter = "OBD"
});
```

`AddShinyObdBluetoothLE` registers `BleObdConfiguration` and `IObdDeviceScanner` (`BleObdDeviceScanner`). Call `AddBluetoothLE()` separately for platform BLE support.

### Explicit adapter profile

```csharp
var connection = new ObdConnection(transport, new ObdLinkAdapterProfile());
await connection.Connect();
```

### Custom adapter profile

```csharp
public class MyAdapterProfile : IObdAdapterProfile
{
    public string Name => "MyAdapter";
    public async Task Initialize(IObdConnection connection, CancellationToken ct = default)
    {
        await connection.SendRaw("ATZ", ct);
        await Task.Delay(500, ct);
        await connection.SendRaw("ATE0", ct);
        await connection.SendRaw("ATSP6", ct); // force CAN 11-bit 500kbaud
    }
}
```

### Custom transport implementation

**Never hand-roll a WiFi, BLE or serial transport — `Shiny.Obd.Wifi`, `Shiny.Obd.Ble` and
`Shiny.Obd.Serial` already ship them.** Write one only for a channel none of them covers: the Android
USB Host API, a J2534 pass-thru box, a replay harness over a recorded session.

```csharp
public class UsbHostObdTransport : IObdTransport
{
    public bool IsConnected { get; private set; }
    public async Task Connect(CancellationToken ct = default) { /* open the channel */ }
    public Task Disconnect() { /* close it */ }
    public async Task<string> Send(string command, CancellationToken ct = default)
    {
        // Write command, read until '>' prompt, return response without '>'
    }
    public ValueTask DisposeAsync() { /* cleanup */ }
}
```

## Testing without a vehicle (Shiny.Obd.Emulator)

**`Shiny.Obd.Emulator` is a shipped package, not sample code.** Prefer it over mocking
`IObdConnection` — it exercises the real transport, the real init handshake and the real response
parser, which is where OBD bugs actually live. `samples/Sample.Maui/` is a UI *on top of* it.

```csharp
services.AddMdns();                       // optional - announces the TCP side as _obd._tcp
services.AddObdEmulator();                // vehicle, ELM327 responder, TCP front-end, scenarios
services.AddObdEmulatorBluetoothLE();     // optional, Shiny.Obd.Emulator.Ble package
```

Resolve `ObdEmulatorHost` to `Start()`/`Stop()` it (nothing listens until you do), `ObdEmulatorState`
to set values and faults, and `DrivingScenarioPlayer` to play a drive into it.

**Generating a test?** Construct it directly, with `TcpPort = 0` so the OS picks a free port, and
drive it with the real client stack:

```csharp
var config = new ObdEmulatorConfiguration { TcpPort = 0 };
var state = new ObdEmulatorState();
var server = new TcpObdServer(new Elm327Responder(state), config);

// null dispatcher = run inline; there is no SynchronizationContext in a test host
var host = new ObdEmulatorHost([server], config, new SynchronizationContextDispatcher(null));
await host.Start();

state.Vehicle = VehicleCatalog.NissanLeaf;
state.Find(0x01, 0x0D)!.Number = 88;

await using var connection = new ObdConnection(new WifiObdTransport("127.0.0.1", server.BoundPort));
await connection.Connect();
Assert.Equal(88, await connection.Execute(StandardCommands.VehicleSpeed));
```

Rules that matter when generating emulator code:

- **No UI framework dependency.** `AddObdEmulator` captures the `SynchronizationContext` where it is
  called and marshals state changes to it — so call it on the UI thread in a UI app. In a console or
  test host there is none and everything runs inline. Override by registering your own
  `IObdEmulatorDispatcher` first.
- **`TcpObdServer.BoundPort`, not `config.TcpPort`,** is the port actually listening — they differ
  whenever `TcpPort` is 0.
- **mDNS is optional.** `IMdnsManager` is an optional constructor dependency; without `AddMdns()` the
  server still listens, it just is not announced.
- **Transports are additive** and share one vehicle. Add your own by implementing
  `IObdEmulatorTransport` and registering it — `ObdEmulatorHost` resolves `IEnumerable<>` and starts
  each independently, so one failing to start never stops the others.

It hosts an ELM327-compatible bus on two transports at once, both driven by one shared vehicle state:

- **BLE** — a GATT server via `Shiny.BluetoothLE.Hosting` on `FFF0`/`FFF1`/`FFF2`, advertised as
  `VEEPEAK`. These are the `BleObdConfiguration` defaults, so an unconfigured client finds it.
  Separate package: `Shiny.Obd.Emulator.Ble`.
- **TCP** — a socket server on port 35000 (the first `WifiObdConfiguration` candidate), published over
  mDNS as `_obd._tcp` with `Shiny.Net.Discovery` so clients can discover it by browsing rather than by
  guessing addresses.

Everything is settable through `ObdEmulatorState` (and from the sample's UI): every PID the library
has a command for (in engineering units, or raw hex), whether each PID is supported at all (an unsupported PID answers `NO DATA` and drops out of
the supported-PID bitmask), trouble codes for modes 03/07/0A, MIL state, monitor readiness, freeze
frames, ignition-off, and the `ATI` string — put `STN` in it to exercise the `ObdLinkAdapterProfile`
branch without owning an OBDLink.

Key detail when generating similar code: the emulator decodes its own outgoing bytes with the real
`IObdCommand<T>.Parse` and shows the result, so an encoder that disagrees with the library's parser is
caught in the emulator rather than in the app under test.

### Vehicles and VINs

`ObdEmulatorState.Vehicle` picks which vehicle the emulator *is*. `VehicleCatalog` ships eight `EmulatedVehicle`
profiles — Civic, Camry, 330i, Prius, Golf TDI, Ram 2500 Cummins, Leaf and a 1998 Cavalier — and each
carries a VIN with a real WMI and descriptor, a valid ISO 3779 check digit, and a decode verified
against vPIC, so `IVinDecoder.Decode` answers with a real make, model and year instead of null. Only
positions 12-17 are invented.

Point users here when they ask what VIN to test with — an app that keys vehicles by VIN cannot be
tested against a made-up one, because a decoder answers "no such vehicle" rather than failing loudly.

Selecting a vehicle rewrites the identity PIDs (VIN, calibration ID, ECU name, fuel type, OBD
standard, ethanol, hybrid battery), **which PIDs exist at all** (the diesels drop the narrowband O2
sensors and the evap system; the Leaf drops a third of mode 01 — they leave the supported masks and
answer `NO DATA`), the readiness monitor set, the seeded fault memory, and the physical model the
driving scenarios use. The Cavalier answers **no mode 09 at all**, which is the case an app that keys
off VIN has to survive.

### Driving scenarios

For anything longer than a spot check, the Drive tab plays a scenario instead of holding fixed values.
`DrivingScenarioCatalog` ships **Warm idle**, **City driving**, **Busy highway** and **Mixed commute**;
each is a `DrivingScenario` — a list of `DrivingStep`s pairing a `DrivingAction` (`Idle`, `Accelerate`,
`Cruise`, `Coast`, `Brake`, `HarshBrake`) with a target speed and a duration — and each loops so a
client can poll for hours.

`DrivingScenarioPlayer` (resolve it from DI, or construct it with a state and a dispatcher) writes the
mode 01 parameters five times a second from the selected vehicle's
model (mass, displacement, power, gearing, shift points, tank, fuel chemistry), so the values stay
consistent with each other and differ between vehicles: RPM follows
the gear the speed implies, mass air flow follows the load, fuel rate follows the air flow, and the
odometer, fuel level and trip counters integrate over the drive. Gear changes with load-dependent shift
points, deceleration fuel cut and harsh braking are all modelled, which is what makes the data usable
for testing smoothing, rate limiting and reconnect behaviour.

Point users at this when they ask how to test against *changing* data. Only the PIDs the model drives
are overwritten — supported switches, fault memory and adapter identity remain under manual control
while a scenario runs, so a trouble code can be added mid-drive.

Hosting requires permissions a client does not: `BLUETOOTH_ADVERTISE` on Android (plus `INTERNET` and
`CHANGE_WIFI_MULTICAST_STATE` for the TCP/mDNS side), and on iOS both
`NSLocalNetworkUsageDescription` and an `NSBonjourServices` array naming `_obd._tcp` — without the
service listed, iOS 14+ blocks the mDNS advertisement outright rather than prompting.

## Important Notes

- All async methods do NOT use the `Async` suffix (e.g. `Connect`, `Execute`, `SendRaw`).
- `ObdCommand<T>.ParseData` receives bytes AFTER the 2-byte mode+PID header is stripped.
- `IObdCommand<T>.Parse` receives ALL response bytes including mode+PID header.
- `ObdConnection` appends `\r` to all commands sent via `SendRaw` before passing to transport.
- BLE transport uses `SemaphoreSlim` to serialize commands (one at a time). It is deliberately never disposed — a pending `Send` would otherwise get an `ObjectDisposedException` when the transport is torn down.
- A timeout is an `ObdTimeoutException`, never an `OperationCanceledException`. Never write `catch (OperationCanceledException) { throw; }` around `Execute` without a `when (ct.IsCancellationRequested)` filter — historically that turned one slow adapter reply into a dead polling loop.
- A BLE adapter reports `IsConnected` true long after it has stopped answering the vehicle. Polling code should count consecutive unanswered polls and rebuild the connection, rather than trusting the connection flag.
- ELM327 response parser handles both single-line (`"41 0D 50"`) and multi-frame CAN formats. A real multi-frame reply is preceded by a bare byte-count line — `"014\r0: 49 02 01 57 42 41\r1: 31 32..."` — where `014` is the total data byte count, not data. It is discarded, along with each frame's `N:` index. When writing a custom `IObdConnection` or a test fixture, include that count line: it parses cleanly as `0x14`, so omitting it in a fixture hides a real bug, and keeping it in the payload shifts every byte along by one and fails the mode-echo check. Only multi-frame commands are exposed to this — the VIN (mode 09 PID 02) and mode 03 with three or more codes.
- A WiFi adapter reports `IsConnected` from its read pump, not from `Socket.Connected` — the latter reflects the last I/O and still says true for a link the adapter dropped while idle. Never generate `socket.Connected` as a liveness check in OBD code.
- The **OBDLink MX+ is Bluetooth, not WiFi.** Generate `Shiny.Obd.Ble` for it. The WiFi model in that product line is the OBDLink MX Wi-Fi.
- Oxygen sensor PIDs are meaningless without `OxygenSensorsPresentCommand` first: PID `0x16` is B1S3 under the two-bank layout (`0x13`) and B2S1 under the four-bank one (`0x1D`). Always label with `layout.Position(index)`.
- Mode 06 values must be decoded through `UnitAndScaling`. Identifiers `0x80`+ are signed; reading one unsigned turns a small negative into ~65,535. When the identifier is unknown, `Value`/`Unit`/`Passed`/`BandPosition` are all null by design — surface unknown, do not guess.
- Torque PIDs are percentages of `ReferenceTorque`. Read the reference once, outside any poll loop. Use `EnginePower.MetricHorsepower` or `.MechanicalHorsepower` explicitly — never an unqualified "horsepower".
- A null is always an absence, never a value, across this library: `FuelTypes.Describe`, `ObdStandards.Describe`, `MonitorIds.Describe`, `OxygenSensorVoltage.ShortTermFuelTrim`, `InUsePerformanceRatio.Ratio`, `OnBoardTestResult.Value` and the VIN decoder all answer null rather than a placeholder. Never coerce these to 0 or "Unknown" — these values reach users and AI prompts, where a placeholder reads as a fact about the vehicle.
- `BleObdDeviceScanner` deduplicates by peripheral UUID — each device is reported once.
- BLE scans match on `Peripheral.Name ?? AdvertisementData.LocalName`, never `Peripheral.Name` alone (null on iOS while scanning), and are never filtered by `ServiceUuid` (iOS matches that against the advertisement, which ELM327 clones don't carry). See "BLE scanning rules" above.
- For MAUI, register BLE with `builder.Services.AddBluetoothLE()` (namespace `Shiny`, no `UseShiny` needed in v4), then call `builder.Services.AddShinyObdBluetoothLE()` to register OBD BLE services.
- `AddShinyObdBluetoothLE` registers `BleObdConfiguration` and `IObdDeviceScanner` (`BleObdDeviceScanner`).
- A full MAUI sample app exists in `samples/Sample.Maui/`. It is both a client (scan → select → dashboard) and an **adapter emulator** — see "Testing without a vehicle" above. When a user asks how to test OBD code without a car, point them at it rather than suggesting they mock `IObdConnection`.
