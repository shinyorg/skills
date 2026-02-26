---
name: shiny-aspire-orleans
description: Generate code using Shiny.Aspire.Orleans for zero-friction integration between .NET Aspire and Microsoft Orleans with ADO.NET storage backends
auto_invoke: true
triggers:
  - aspire orleans
  - orleans aspire
  - WithDatabaseSetup
  - UseOrleansWithAdoNet
  - UseOrleansClientWithAdoNet
  - orleans database setup
  - orleans schema
  - orleans clustering
  - orleans grain storage
  - orleans reminders
  - OrleansFeature
  - Shiny.Aspire.Orleans
  - aspire orleans hosting
  - aspire orleans server
  - aspire orleans client
  - orleans adonet
  - orleans ado.net
---

# Shiny Aspire Orleans Skill

You are an expert in Shiny.Aspire.Orleans, a set of packages that provide zero-friction integration between .NET Aspire and Microsoft Orleans for ADO.NET storage backends. It automatically provisions Orleans database schemas and wires up clustering, grain persistence, and reminders from Aspire configuration — no manual SQL scripts or connection string plumbing required.

## When to Use This Skill

Invoke this skill when the user wants to:
- Set up Orleans with .NET Aspire using ADO.NET storage (PostgreSQL, SQL Server, or MySQL)
- Automatically create Orleans database schemas from Aspire
- Configure an Orleans silo with ADO.NET providers from Aspire-injected config
- Configure an Orleans client with ADO.NET clustering from Aspire-injected config
- Use `WithDatabaseSetup` to auto-provision Orleans tables
- Use `UseOrleansWithAdoNet` to configure a silo
- Use `UseOrleansClientWithAdoNet` to configure a client
- Select which Orleans features to provision (clustering, persistence, reminders)
- Set up multiple named grain storage providers

## Library Overview

- **Repository**: https://github.com/shinyorg/aspire
- **Target**: `net10.0`
- **Aspire**: 13.1+
- **Orleans**: 10.0+

### Packages

| Package | NuGet | Usage |
|---|---|---|
| `Shiny.Aspire.Orleans.Hosting` | Install in Aspire AppHost | Auto-runs Orleans schema scripts when the database becomes ready |
| `Shiny.Aspire.Orleans.Server` | Install in Orleans silo | Configures ADO.NET clustering, grain storage, and reminders from Aspire config |
| `Shiny.Aspire.Orleans.Client` | Install in Orleans client | Configures ADO.NET clustering from Aspire config |

### Supported Databases

| Database | ADO.NET Invariant | ProviderType Value |
|---|---|---|
| PostgreSQL | `Npgsql` | `PostgresDatabase` |
| SQL Server | `Microsoft.Data.SqlClient` | `SqlServerDatabase` |
| MySQL | `MySql.Data.MySqlClient` | `MySqlDatabase` |

## Setup

### 1. Aspire AppHost

Install `Shiny.Aspire.Orleans.Hosting` in the AppHost project.

```csharp
using Shiny.Aspire.Orleans.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pg")
    .AddDatabase("orleans-db");

var orleans = builder.AddOrleans("cluster")
    .WithClustering(db)
    .WithGrainStorage("Default", db)
    .WithReminders(db)
    .WithDatabaseSetup(db); // creates all Orleans tables automatically

builder.AddProject<Projects.MySilo>("silo")
    .WithReference(orleans)
    .WaitFor(db);

builder.AddProject<Projects.MyApi>("api")
    .WithReference(orleans.AsClient())
    .WaitFor(db);

builder.Build().Run();
```

### 2. Orleans Silo

Install `Shiny.Aspire.Orleans.Server` in the silo project.

```csharp
using Shiny.Aspire.Orleans.Server;

var builder = WebApplication.CreateBuilder(args);
builder.UseOrleansWithAdoNet(); // reads Aspire-injected config automatically
var app = builder.Build();
app.Run();
```

### 3. Orleans Client

Install `Shiny.Aspire.Orleans.Client` in the client project (e.g. an API gateway).

```csharp
using Shiny.Aspire.Orleans.Client;

var builder = WebApplication.CreateBuilder(args);
builder.UseOrleansClientWithAdoNet(); // reads Aspire-injected config automatically
var app = builder.Build();

app.MapGet("/counter/{name}", async (string name, IClusterClient client) =>
{
    var grain = client.GetGrain<ICounterGrain>(name);
    var count = await grain.GetCount();
    return Results.Ok(new { name, count });
});

app.Run();
```

## API Reference

### Hosting Package — `Shiny.Aspire.Orleans.Hosting`

#### WithDatabaseSetup

```csharp
public static OrleansService WithDatabaseSetup(
    this OrleansService orleans,
    IResourceBuilder<IResourceWithConnectionString> database,
    OrleansFeature features = OrleansFeature.All
)
```

Subscribes to Aspire's `ResourceReadyEvent` for the database resource. When the database is up and accepting connections, it executes embedded Orleans SQL schema scripts. The database type (PostgreSQL, SQL Server, MySQL) is auto-detected from the Aspire resource.

**Parameters:**
- `orleans` — The Orleans service from `builder.AddOrleans()`
- `database` — An Aspire database resource (e.g. from `AddPostgres`, `AddSqlServer`, `AddMySql`)
- `features` — Which Orleans features to provision (default: `OrleansFeature.All`)

#### OrleansFeature (Flags Enum)

```csharp
[Flags]
public enum OrleansFeature
{
    Clustering = 1,   // Membership tables for silo discovery
    Persistence = 2,  // Grain storage tables
    Reminders = 4,    // Reminder tables
    All = Clustering | Persistence | Reminders
}
```

Use to limit which schemas are provisioned:

```csharp
// Only clustering and persistence (no reminders)
orleans.WithDatabaseSetup(db, OrleansFeature.Clustering | OrleansFeature.Persistence);

// Only clustering
orleans.WithDatabaseSetup(db, OrleansFeature.Clustering);
```

#### DatabaseType (Enum)

```csharp
public enum DatabaseType
{
    SqlServer,
    PostgreSQL,
    MySql
}
```

Auto-detected from the Aspire resource — you do not need to specify this directly.

### Server Package — `Shiny.Aspire.Orleans.Server`

#### UseOrleansWithAdoNet

```csharp
public static IHostApplicationBuilder UseOrleansWithAdoNet(
    this IHostApplicationBuilder builder
)
```

Reads Aspire-injected configuration and configures the Orleans silo with ADO.NET providers for:
- **Clustering** — from `Orleans:Clustering:ProviderType` and `Orleans:Clustering:ServiceKey`
- **Grain Storage** — from `Orleans:GrainStorage:{Name}:ProviderType` and `Orleans:GrainStorage:{Name}:ServiceKey` (supports multiple named providers)
- **Reminders** — from `Orleans:Reminders:ProviderType` and `Orleans:Reminders:ServiceKey`

The connection string is resolved from `ConnectionStrings:{ServiceKey}` using Aspire's standard connection string injection.

### Client Package — `Shiny.Aspire.Orleans.Client`

#### UseOrleansClientWithAdoNet

```csharp
public static IHostApplicationBuilder UseOrleansClientWithAdoNet(
    this IHostApplicationBuilder builder
)
```

Reads Aspire-injected configuration and configures the Orleans client with ADO.NET clustering only (clients do not need grain storage or reminders). Reads `Orleans:Clustering:ProviderType` and `Orleans:Clustering:ServiceKey`.

## Configuration Flow

Aspire injects the following configuration when you use `.WithReference(orleans)`:

```
Orleans:Clustering:ProviderType = "PostgresDatabase"
Orleans:Clustering:ServiceKey   = "orleans-db"
Orleans:GrainStorage:Default:ProviderType = "PostgresDatabase"
Orleans:GrainStorage:Default:ServiceKey   = "orleans-db"
Orleans:Reminders:ProviderType  = "PostgresDatabase"
Orleans:Reminders:ServiceKey    = "orleans-db"
ConnectionStrings:orleans-db    = "Host=...;Database=..."
```

## Schema Provisioning Order

`WithDatabaseSetup` runs embedded SQL scripts in order:

1. **Main** — creates the `OrleansQuery` table (query registry)
2. **Clustering** — creates `OrleansMembershipVersionTable`, `OrleansMembershipTable`, and stored procedures
3. **Persistence** — creates `OrleansStorage` table and stored procedures
4. **Reminders** — creates `OrleansRemindersTable` and stored procedures

## Switching Databases

Swap the Aspire resource builder — everything else stays the same:

```csharp
// PostgreSQL
var db = builder.AddPostgres("pg").AddDatabase("orleans-db");

// SQL Server
var db = builder.AddSqlServer("sql").AddDatabase("orleans-db");

// MySQL
var db = builder.AddMySql("mysql").AddDatabase("orleans-db");
```

## Multiple Grain Storage Providers

```csharp
// AppHost
var orleans = builder.AddOrleans("cluster")
    .WithClustering(db)
    .WithGrainStorage("Default", db)
    .WithGrainStorage("Archive", archiveDb)
    .WithDatabaseSetup(db);

// Grain
public class MyGrain(
    [PersistentState("state", "Default")] IPersistentState<MyState> state,
    [PersistentState("archive", "Archive")] IPersistentState<ArchiveState> archive
) : Grain, IMyGrain { }
```

Each named provider reads from `Orleans:GrainStorage:{Name}:ProviderType` and `Orleans:GrainStorage:{Name}:ServiceKey`.

## Code Generation Best Practices

1. **Always use `WithDatabaseSetup`** in the AppHost to auto-provision schemas — never require manual SQL scripts.
2. **Always call `WaitFor(db)`** on projects that reference Orleans, so the database is ready before the silo starts.
3. **Use `.AsClient()`** when wiring a client project — this provides only clustering config, not full silo config.
4. **One call per project** — use `UseOrleansWithAdoNet()` in silo projects and `UseOrleansClientWithAdoNet()` in client projects.
5. **Feature flags are optional** — only use `OrleansFeature` flags if the user explicitly wants to skip certain schemas.
6. **Don't hardcode connection strings** — Aspire injects them automatically via configuration.
7. **Don't manually configure ADO.NET invariants** — the packages auto-detect the correct invariant from the provider type.
8. **Use named grain storage** for multiple persistence stores — each name maps to a separate configuration section.
