---
name: shiny-efcore
description: Generate code using Shiny EF Core Extensions — feature packages for EF Core 10: SQLite spatial (NetTopologySuite without SpatiaLite), sqlite-vec vector search and FTS5 full-text search that work on iOS/Android; a transactional outbox; provider-agnostic temporal history; scoped Microsoft.Extensions.AI tools and an MCP server over a DbContext; versioned run-once seeding; and an Aspire AppHost seed gate
auto_invoke: true
triggers:
  - Shiny.EntityFrameworkCore
  - efcore sqlite spatial
  - efcore spatialite ios
  - mod_spatialite
  - UseShinySpatial
  - UseShinyVectors
  - UseShinyFullText
  - HasSpatialIndex
  - HasVectorIndex
  - HasFullTextIndex
  - GeoDistanceMeters
  - IsWithinMeters
  - VectorDistance
  - VectorDistanceMetric
  - NearestTo
  - VectorMatch
  - FullTextMatch
  - FullTextRank
  - FullTextQuery
  - SqliteVec
  - sqlite-vec efcore
  - fts5 efcore
  - efcore outbox
  - AddOutbox
  - AddOutboxPublishing
  - AddDbContextOutbox
  - OutboxMessage
  - IOutboxDispatcher
  - IOutboxAdmin
  - OutboxRunner
  - WatchOutbox
  - Enqueue
  - efcore temporal
  - efcore history
  - efcore audit trail
  - HasHistory
  - UseEntityHistory
  - HistoryAsync
  - AsOfAsync
  - AsOfAllAsync
  - ChangesByActorAsync
  - ChangesBetweenAsync
  - RestoreAsync
  - GetDiffBetweenAsync
  - EntityVersion
  - AddDbContextAITools
  - DbContextAITools
  - IEntityAIToolBuilder
  - EntityAICapabilities
  - WithDbContext
  - IDbContextMcpBuilder
  - efcore mcp
  - IDbSeeder
  - AddDbSeeder
  - AddDbSeeding
  - DbSeedRunner
  - UseVersionedSeeding
  - DbSeedHistory
  - WithDbContextSeeder
  - WithDbSeeder
  - DbSeedMode
---

# Shiny EF Core Extensions

Independent feature packages for **EF Core 10 (`net10.0`)**. Each one plugs in through EF's own extension points:
options-builder extensions, `modelBuilder`/`EntityTypeBuilder` extensions, and `EF.Functions`. **Never generate a
custom base `DbContext`** for any of them.

Providers: Outbox, Temporal, AI and Seeding support **SQLite, PostgreSQL (Npgsql), SQL Server and MySQL
(`MySql.EntityFrameworkCore` / `UseMySQL`, not Pomelo, which has no EF 10 build)**. `Sqlite.Mobile` is SQLite-only.

## 1. `Shiny.EntityFrameworkCore.Sqlite.Mobile`: spatial, vectors, FTS5 on SQLite

Use this **instead of** `Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite` / `UseNetTopologySuite()`,
which needs `mod_spatialite` and can't run on iOS or Android.

```csharp
services.AddDbContext<AppDbContext>(o => o.UseSqlite(cs, s => s
    .UseShinySpatial()
    .UseShinyVectors()      // throws at startup if sqlite-vec can't load; call SqliteVec.RegisterAutoExtension() before opening raw connections
    .UseShinyFullText()));

protected override void OnModelCreating(ModelBuilder mb)
{
    mb.Entity<Place>().HasSpatialIndex(x => x.Location);                       // R*Tree pre-filter
    mb.Entity<Doc>().HasVectorIndex(x => x.Embedding, 384, VectorDistanceMetric.Cosine); // vec0
    mb.Entity<Doc>().HasFullTextIndex(x => new { x.Title, x.Body });          // FTS5 external content
}
```

- Geometry properties are NetTopologySuite types (`Point`, `Polygon`, …). Every NTS predicate translates:
  `Intersects`, `Contains`, `Within`, `Covers`, `CoveredBy`, `Touches`, `Crosses`, `Overlaps`, `Disjoint`,
  `EqualsTopologically`, `Distance`, `IsWithinDistance`, plus `X/Y`, `Area`, `Length`, `SRID` and `IsEmpty`. NTS distance is
  **planar**; for meters use `EF.Functions.GeoDistanceMeters(a, b)` / `EF.Functions.IsWithinMeters(a, b, meters)`.
- `IsWithinDistance` / `IsWithinMeters` use the R*Tree; `Distance(...) < d` does not.
- Vectors: `float[]` or `ReadOnlyMemory<float>` properties. Exact distance via
  `EF.Functions.VectorDistance(x.Embedding, q, VectorDistanceMetric.Cosine)` in `OrderBy`. The KNN form is
  `db.Docs.NearestTo(d => d.Embedding, q, k)` → `IQueryable<VectorMatch<T>>` (`Entity`, `Distance`). Filters
  apply after the k nearest are found, so it can return fewer than k.
- Full text: `EF.Functions.FullTextMatch(entity, q)` or `FullTextMatch(entity.Prop, q)`;
  order by `EF.Functions.FullTextRank(entity, q)` (bm25, **lower is better**). Pass user input through
  `FullTextQuery.Escape(text, prefixLastTerm: true)`.
- All three index types require a **single integer primary key** (the rowid); other keys fail at model build.
  Triggers keep the index tables in sync, including for `ExecuteUpdate`/`ExecuteDelete`/raw SQL, and both
  `EnsureCreated` and migrations create them.
- A spatial-indexed table can't be written by tools outside EF (the trigger calls a registered function). For raw
  connections, call `SqliteSpatial.RegisterFunctions(connection)`.
- Apps that don't use vectors can drop the natives with `<ShinySqliteVecNative>false</ShinySqliteVecNative>`.

## 2. `Shiny.EntityFrameworkCore.Outbox`

```csharp
protected override void OnModelCreating(ModelBuilder mb) => mb.AddOutbox();   // table "outbox"; migrations pick it up

services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs)
    .AddOutboxPublishing(p => p.Publish<Order>(
        c => c.Operation == OutboxOperations.Delete ? null : new OrderChanged(c.Entity.Id),   // null = publish nothing
        OutboxOperations.Insert | OutboxOperations.Update,
        partitionKey: c => c.Entity.Id.ToString())));

services.AddDbContextOutbox<AppDbContext, BusDispatcher>(o => { o.OrderedPartitions = true; o.MaxAttempts = 8; });
// or: services.AddDbContextOutbox<AppDbContext>((msg, ct) => bus.Publish(msg.MessageType, msg.Payload, ct));

db.Orders.Add(order);
db.Enqueue(new OrderPlaced(order.Id));   // tracked entity, commits with the order
await db.SaveChangesAsync();
```

- `IOutboxDispatcher.Dispatch(OutboxMessage, ct)`: return to acknowledge, throw to retry. It is resolved from a
  fresh scope for each message. **Delivery is at least once, so consumers must be idempotent.**
- Declarative events for inserts with store-generated keys are built after the first save and written in the
  same transaction. With `EnableRetryOnFailure`, wrap the save in `db.Database.CreateExecutionStrategy()` with
  your own transaction.
- `IOutboxAdmin` provides `PendingCount`, `ScheduledCount`, `DeadLetterCount`, `OldestPendingAt`, `DeadLetters`,
  `Requeue`, `RequeueAllDeadLettered` and `PurgeProcessed`. `OutboxRunner.DrainOnce` is for tests and mobile;
  `db.WatchOutbox<TEvent>()` streams messages.
- Columns are pinned snake_case (`OutboxColumns`). Relational providers only.

## 3. `Shiny.EntityFrameworkCore.Temporal`

```csharp
mb.Entity<Order>().HasHistory(h => { h.Retention = TimeSpan.FromDays(90); h.MaxVersions = 50; });

services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs).UseEntityHistory(h =>
    h.ResolveActor = sp => sp.GetService<ICurrentUser>()?.Name));   // or h.CaptureActor = () => ...
```

- Both calls are required. Without `UseEntityHistory()`, writes record nothing and reads throw. Actor delegates go
  on `UseEntityHistory`, **not** `HasHistory`, which keeps the model migration-safe.
- Read API on `DbContext` (keys work like `Find`; use `object[]` for composite keys): `HistoryAsync<T>(key)`,
  `AsOfAsync<T>(key, instant)`, `AsOfAllAsync<T>(instant)`, `ChangesByActorAsync<T>(actor)`,
  `ChangesBetweenAsync<T>(from, to)` (half-open), `RestoreAsync<T>(key, version)` (writes a new version and
  calls `SaveChangesAsync`), `GetDiffBetweenAsync<T>(key, v1, v2)` → RFC 6902 `JsonPatchDocument<T>`.
- Results are `EntityVersion<T>` (`Key`, `Version`, `ValidFrom`, `ValidTo`, `Operation`, `Actor`, `Entity`, detached).
- `ExecuteUpdate`/`ExecuteDelete` on a history entity **throw** unless `AllowBulkWrites = true`. Raw SQL bypasses
  history.
- With `AddDbContextPool`, `ResolveActor` only sees the root provider. Use `AddDbContext` for request-scoped actors.

## 4. `Shiny.EntityFrameworkCore.AI` + `.Mcp`

```csharp
services.AddScoped<ICurrentTenant, CurrentTenant>();   // register before the tools
services.AddDbContextAITools<AppDbContext>(b => b
    .AddEntity<Order>(t => t
        .Where<ICurrentTenant>((tenant, _) => o => o.TenantId == tenant.Id)   // the model cannot remove or widen it
        .AllowQuery().AllowCount().AllowAggregate().AllowGet()
        .Description("Customer orders")
        .Ignore(o => o.InternalNotes)
        .MaxPageSize(50))
    .AddEntity<Customer>());   // no Allow* call = read-only

IReadOnlyList<AITool> tools = sp.GetRequiredService<DbContextAITools<AppDbContext>>().Tools;
// no DI: dbContextFactory.CreateAITools(b => ...)

services.AddMcpServer().WithHttpTransport()
    .WithDbContext<AppDbContext>(m => m.AllowWrites().ExposeResources().ExposePrompts()
        .AddEntity<Order>(t => t.Where<ICurrentTenant>((x, _) => o => o.TenantId == x.Id).AllowAll()));
app.MapMcp("/mcp");
```

- Tools are named `{entity_snake}_get_by_id|_query|_count|_aggregate|_insert|_update|_delete`.
- A scope that can't be resolved (missing service, the callback throws, or it returns null) fails the call.
  Out-of-scope ids look like missing ones. Inserts and updates are checked against the scope in memory: use
  `OnInsert((o, ctx) => o.TenantId = ...)` to stamp hidden fields first.
- Update is a **patch**: fields the model omits keep their stored values.
- Navigations are excluded; the model follows a foreign key through the other entity's own tools. Get, update and
  delete need a single visible CLR primary key.
- MCP writes need **both** `AllowWrites()` and the entity's write capabilities.

## 5. `Shiny.EntityFrameworkCore.Seeding`

```csharp
public sealed class CountrySeeder : IDbSeeder<AppDbContext>
{
    public string Name => "countries";
    public int Version => 2;   // bump to re-run
    public async Task SeedAsync(AppDbContext db, CancellationToken ct) { /* idempotent upserts */ }
}

services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs).UseVersionedSeeding<AppDbContext>()); // runs on Migrate/EnsureCreated
services.AddDbSeeder<AppDbContext, CountrySeeder>();
services.AddDbSeeding<AppDbContext>(o => o.Initialization = DbInitialization.Migrate);    // hosted service: migrate, then seed

// no generic host (MAUI):
await DbSeedRunner.RunAsync(db, [new CountrySeeder()], ct);
```

- The marker table `__ShinySeedHistory` is created with raw SQL, so the model needs **no changes**. A seeder runs
  exactly once across concurrent instances (row lock + re-check). Seeders must still be idempotent, because a
  version bump re-runs them over existing data.
- Inside `AddDbContext`, use the generic `UseVersionedSeeding<AppDbContext>()`.
- The marker table counts as "a table" to `EnsureCreated`. Prefer `Migrate`, or call `EnsureCreated` before
  any seeding.

## 6. `Shiny.EntityFrameworkCore.Aspire.Hosting`

```csharp
builder.AddPostgres("pg").WithDataVolume().AddDatabase("appdb")
    .WithDbContextSeeder<AppDbContext>(
        (o, cs) => o.UseNpgsql(cs),
        (db, gate, ct) => DbSeedRunner.RunAsync(db, [new CountrySeeder()], ct),
        DbSeedMode.FirstTimeOnly,            // or Recreate (EnsureDeleted, then seed on every start)
        DbInitialization.Migrate);
```

- Seeding happens only on **first setup** (no marker) or **Recreate**. The marker lives in the database, so
  dropping the volume makes the next start seed again. The marker is written only after the seed succeeds.
- `WithDbSeeder(configureMarkerConnection, seed, mode)` is the raw form: the callback owns the schema and the wipe.
- The AppHost references the EF provider and the `DbContext` project.
