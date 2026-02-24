--
name: shiny-sqlitedocumentdb
description: Generate code using Shiny.SqliteDocumentDb, a schema-free SQLite JSON document store for .NET with LINQ queries and AOT support
auto_invoke: true
triggers:
  - document store
  - document db
  - SqliteDocumentStore
  - IDocumentStore
  - json document
  - schema-free
  - sqlite document
  - document database
  - json store
  - Shiny.SqliteDocumentDb
  - json_extract
  - document query
---

# Shiny SqliteDocumentDb Skill

You are an expert in Shiny.SqliteDocumentDb, a lightweight SQLite-based document store for .NET that turns SQLite into a schema-free JSON document database with LINQ querying and full AOT/trimming support.

## When to Use This Skill

Invoke this skill when the user wants to:
- Store and retrieve .NET objects as JSON documents in SQLite
- Query JSON documents with LINQ expressions or raw SQL
- Set up a schema-free document database without migrations
- Use AOT-safe document storage with `JsonTypeInfo<T>` overloads
- Stream query results with `IAsyncEnumerable<T>`
- Create JSON property indexes for faster queries
- Project query results into DTOs at the SQL level
- Compute aggregates (Max, Min, Sum, Average) across documents
- Use aggregate projections with GROUP BY via `Sql.*` markers
- Sort query results with expression-based `OrderBy<T>`
- Use transactions for atomic document operations
- Work with nested objects and child collections without table design

## Library Overview

- **Repository**: https://github.com/shinyorg/SqliteDocumentDb
- **Namespace**: `Shiny.SqliteDocumentDb`
- **NuGet**: `Shiny.SqliteDocumentDb`
- **Target**: `net10.0`

## Setup

### Direct Instantiation

```csharp
var store = new SqliteDocumentStore(new DocumentStoreOptions
{
    ConnectionString = "Data Source=mydata.db"
});
```

### Dependency Injection

```csharp
services.AddSqliteDocumentStore("Data Source=mydata.db");

// or with full options
services.AddSqliteDocumentStore(opts =>
{
    opts.ConnectionString = "Data Source=mydata.db";
    opts.TypeNameResolution = TypeNameResolution.FullName;
    opts.JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
});
```

Registers `IDocumentStore` as a singleton backed by `SqliteDocumentStore`.

### DocumentStoreOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ConnectionString` | `string` (required) | — | SQLite connection string |
| `TypeNameResolution` | `TypeNameResolution` | `ShortName` | How type names are stored (`ShortName` or `FullName`) |
| `JsonSerializerOptions` | `JsonSerializerOptions?` | camelCase, no indent | JSON serialization settings |

## AOT Setup

For AOT/trimming compatibility, create a source-generated JSON context:

```csharp
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(OrderLine))]
public partial class AppJsonContext : JsonSerializerContext;
```

**Important:** Do NOT add `[JsonSerializerContext]` attribute — it is abstract and inherited automatically.

Create an instance with your desired options:

```csharp
var ctx = new AppJsonContext(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
```

Pass `ctx.Options` to `DocumentStoreOptions.JsonSerializerOptions` so the expression visitor and serializer share the same configuration.

## Core API Reference

All methods on `IDocumentStore`. Every method has both a reflection-based overload and an AOT-safe overload accepting `JsonTypeInfo<T>`.

### Set (Upsert)

```csharp
// Auto-generated GUID key — returns the ID
var id = await store.Set(new User { Name = "Alice", Age = 25 }, ctx.User);

// Explicit key
await store.Set("user-1", new User { Name = "Alice", Age = 25 }, ctx.User);
```

### Get

```csharp
var user = await store.Get<User>("user-1", ctx.User);
```

### GetAll

```csharp
var users = await store.GetAll<User>(ctx.User);
```

### Query (Expression)

```csharp
var results = await store.Query<User>(u => u.Name == "Alice", ctx.User);
```

### Query (Raw SQL)

```csharp
var results = await store.Query<User>(
    "json_extract(Data, '$.name') = @name",
    ctx.User,
    new { name = "Alice" });
```

### Count

```csharp
var count = await store.Count<User>(u => u.Age == 25, ctx.User);

// Raw SQL
var count = await store.Count<User>(
    "json_extract(Data, '$.age') > @minAge",
    new { minAge = 30 });
```

### Remove

```csharp
// By ID
bool deleted = await store.Remove<User>("user-1");

// By expression — returns number deleted
int deleted = await store.Remove<User>(u => u.Age < 18, ctx.User);
```

### Clear

```csharp
int deletedCount = await store.Clear<User>();
```

### RunInTransaction

```csharp
await store.RunInTransaction(async tx =>
{
    await tx.Set("u1", new User { Name = "Alice", Age = 25 }, ctx.User);
    await tx.Set("u2", new User { Name = "Bob", Age = 30 }, ctx.User);
    // Commits on success, rolls back on exception
});
```

## Expression Query Patterns

The expression visitor translates LINQ expressions to `json_extract` SQL. Property names are resolved from `JsonTypeInfo` metadata, so `[JsonPropertyName]` and naming policies are respected.

### Equality and Comparisons

```csharp
u => u.Name == "Alice"       // json_extract(Data, '$.name') = @p0
u => u.Age > 25              // json_extract(Data, '$.age') > @p0
u => u.Age <= 25             // json_extract(Data, '$.age') <= @p0
```

### Logical Operators

```csharp
u => u.Age == 25 && u.Name == "Alice"          // (... AND ...)
u => u.Name == "Alice" || u.Name == "Bob"      // (... OR ...)
u => !(u.Name == "Alice")                       // NOT (...)
```

### Null Checks

```csharp
u => u.Email == null          // ... IS NULL
u => u.Email != null          // ... IS NOT NULL
```

### String Methods

```csharp
u => u.Name.Contains("li")       // ... LIKE '%' || @p0 || '%'
u => u.Name.StartsWith("Al")     // ... LIKE @p0 || '%'
u => u.Name.EndsWith("ob")       // ... LIKE '%' || @p0
```

### Nested Object Properties

```csharp
o => o.ShippingAddress.City == "Portland"
// json_extract(Data, '$.shippingAddress.city') = @p0
```

### Collection Queries with Any()

```csharp
// Object collection — filter by child property
o => o.Lines.Any(l => l.ProductName == "Widget")
// EXISTS (SELECT 1 FROM json_each(...) WHERE ...)

// Primitive collection — filter by value
o => o.Tags.Any(t => t == "priority")
// EXISTS (SELECT 1 FROM json_each(...) WHERE value = @p0)

// Check if collection has any elements
o => o.Tags.Any()
// json_array_length(Data, '$.tags') > 0
```

### Collection Queries with Count()

```csharp
// Count elements (no predicate)
o => o.Lines.Count() > 1
// json_array_length(Data, '$.lines') > 1

// Count matching elements (with predicate)
o => o.Lines.Count(l => l.Quantity >= 3) >= 1
// (SELECT COUNT(*) FROM json_each(...) WHERE ...) >= 1
```

### DateTime and DateTimeOffset

Values are formatted as ISO 8601 to match `System.Text.Json` output:

```csharp
var cutoff = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
e => e.StartDate > cutoff

var start = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
e => e.CreatedAt >= start && e.CreatedAt < end
```

### Captured Variables

```csharp
var targetName = "Alice";
u => u.Name == targetName    // Extracted from closure at translate time
```

## Projections

Project into DTOs at the SQL level via `json_object` — no full document deserialization needed.

### Flat Projection

```csharp
var results = await store.Query<User, UserSummary>(
    u => u.Age == 25,
    u => new UserSummary { Name = u.Name, Email = u.Email },
    ctx.User,
    ctx.UserSummary);
```

### Nested Source Properties

```csharp
var results = await store.Query<Order, OrderSummary>(
    o => o.Status == "Shipped",
    o => new OrderSummary { Customer = o.CustomerName, City = o.ShippingAddress.City },
    ctx.Order,
    ctx.OrderSummary);
```

### GetAll with Projection

```csharp
var results = await store.GetAll<Order, OrderDetail>(
    o => new OrderDetail { Customer = o.CustomerName, LineCount = o.Lines.Count() },
    ctx.Order,
    ctx.OrderDetail);
```

### Collection Methods in Projections

```csharp
// Count()
o => new OrderDetail { LineCount = o.Lines.Count() }
// SQL: json_array_length(Data, '$.lines')

// Count(predicate)
o => new OrderDetail { GadgetCount = o.Lines.Count(l => l.ProductName == "Gadget") }
// SQL: (SELECT COUNT(*) FROM json_each(...) WHERE ...)

// Any()
o => new OrderDetail { HasLines = o.Lines.Any() }
// SQL: CASE WHEN json_array_length(...) > 0 THEN json('true') ELSE json('false') END

// Any(predicate)
o => new OrderDetail { HasPriority = o.Tags.Any(t => t == "priority") }
// SQL: CASE WHEN EXISTS (...) THEN json('true') ELSE json('false') END

// Collection aggregates — Sum, Max, Min, Average
o => new R { TotalQty = o.Lines.Sum(l => l.Quantity) }
// SQL: (SELECT SUM(json_extract(value, '$.quantity')) FROM json_each(Data, '$.lines'))

o => new R { MaxPrice = o.Lines.Max(l => l.UnitPrice) }
// SQL: (SELECT MAX(json_extract(value, '$.unitPrice')) FROM json_each(Data, '$.lines'))
```

## Ordering

Sort results at the SQL level using `OrderBy<T>`. Available on all `GetAll`, `Query`, `GetAllStream`, and `QueryStream` expression methods.

```csharp
// Ascending
var users = await store.GetAll(ctx.User, OrderBy<User>.Ascending(u => u.Age));

// Descending
var users = await store.GetAll(ctx.User, OrderBy<User>.Descending(u => u.Age));

// With query predicate
var results = await store.Query<User>(
    u => u.Age > 25,
    ctx.User,
    OrderBy<User>.Ascending(u => u.Name));

// With projection
var results = await store.GetAll<User, UserSummary>(
    u => new UserSummary { Name = u.Name, Email = u.Email },
    ctx.User,
    ctx.UserSummary,
    OrderBy<User>.Ascending(u => u.Name));

// With streaming
await foreach (var user in store.GetAllStream(ctx.User, OrderBy<User>.Descending(u => u.Age)))
{
    Console.WriteLine(user.Name);
}
```

The `orderBy` parameter is always optional (`null` default) — all existing call sites work unchanged.

Generated SQL: `ORDER BY json_extract(Data, '$.age') ASC`

## Scalar Aggregates

Compute Max, Min, Sum, Average across all documents or filtered by a predicate.

```csharp
var maxAge = await store.Max<User, int>(u => u.Age, ctx.User);
var minAge = await store.Min<User, int>(u => u.Age, ctx.User);
var totalAge = await store.Sum<User, int>(u => u.Age, ctx.User);
var avgAge = await store.Average<User>(u => u.Age, ctx.User);

// With predicate filter
var maxAge = await store.Max<User, int>(u => u.Age < 35, u => u.Age, ctx.User);
```

## Aggregate Projections (GROUP BY)

Use `Sql` marker class for aggregate projections with automatic GROUP BY.

```csharp
var results = await store.Aggregate<Order, OrderStats>(
    o => new OrderStats
    {
        Status = o.Status,            // GROUP BY column
        OrderCount = Sql.Count(),     // COUNT(*)
    },
    ctx.Order,
    ctx.OrderStats);

// All Sql markers: Sql.Count(), Sql.Max(x.Prop), Sql.Min(x.Prop), Sql.Sum(x.Prop), Sql.Avg(x.Prop)

// With predicate filter
var results = await store.Aggregate<Order, OrderStats>(
    o => o.Status == "Shipped",
    o => new OrderStats { Status = o.Status, OrderCount = Sql.Count() },
    ctx.Order,
    ctx.OrderStats);
```

## Streaming

All list-returning methods have `IAsyncEnumerable<T>` streaming counterparts that yield results one-at-a-time without buffering.

### GetAllStream

```csharp
await foreach (var user in store.GetAllStream<User>(ctx.User))
{
    Console.WriteLine(user.Name);
}
```

### GetAllStream with Projection

```csharp
await foreach (var summary in store.GetAllStream<Order, OrderSummary>(
    o => new OrderSummary { Customer = o.CustomerName, City = o.ShippingAddress.City },
    ctx.Order,
    ctx.OrderSummary))
{
    Console.WriteLine($"{summary.Customer} in {summary.City}");
}
```

### QueryStream with Expression

```csharp
await foreach (var user in store.QueryStream<User>(u => u.Age > 30, ctx.User))
{
    Console.WriteLine(user.Name);
}
```

### QueryStream with Raw SQL

```csharp
await foreach (var user in store.QueryStream<User>(
    "json_extract(Data, '$.name') = @name",
    ctx.User,
    new { name = "Alice" }))
{
    Console.WriteLine(user.Name);
}
```

### QueryStream with Projection

```csharp
await foreach (var summary in store.QueryStream<Order, OrderSummary>(
    o => o.Status == "Shipped",
    o => new OrderSummary { Customer = o.CustomerName, City = o.ShippingAddress.City },
    ctx.Order,
    ctx.OrderSummary))
{
    Console.WriteLine(summary.Customer);
}
```

**Note:** Streaming methods hold the internal semaphore for the duration of enumeration. Consume results promptly and avoid interleaving other store operations within the same `await foreach` loop.

## Index Management

Methods on `SqliteDocumentStore` directly (not on `IDocumentStore`) since indexes are DDL, not document CRUD.

### Create an Index

```csharp
await store.CreateIndexAsync<User>(u => u.Name, ctx.User);
// CREATE INDEX IF NOT EXISTS idx_json_User_name
// ON documents (json_extract(Data, '$.name'))
// WHERE TypeName = 'User';
```

### Nested Property Index

```csharp
await store.CreateIndexAsync<Order>(o => o.ShippingAddress.City, ctx.Order);
```

### Drop a Specific Index

```csharp
await store.DropIndexAsync<User>(u => u.Name, ctx.User);
```

### Drop All Indexes for a Type

```csharp
await store.DropAllIndexesAsync<User>();
```

Index names are deterministic (`idx_json_{typeName}_{jsonPath}`). `CreateIndexAsync` uses `IF NOT EXISTS`, so calling it multiple times is safe.

## Transactions

```csharp
await store.RunInTransaction(async tx =>
{
    await tx.Set("u1", new User { Name = "Alice", Age = 25 }, ctx.User);
    await tx.Set("u2", new User { Name = "Bob", Age = 30 }, ctx.User);
    // Commits on success, rolls back on exception
});
```

The `tx` parameter is an `IDocumentStore` scoped to the transaction. All operations within the callback share the same SQLite transaction.

## Code Generation Best Practices

1. **Always use AOT overloads** — pass `JsonTypeInfo<T>` to every API call (e.g., `ctx.User`, `ctx.Order`)
2. **Pass `ctx.Options` for shared config** — set `DocumentStoreOptions.JsonSerializerOptions = ctx.Options` so the expression visitor resolves property names correctly
3. **Derive from `JsonSerializerContext`** — add `[JsonSerializable(typeof(T))]` for each type; do NOT add `[JsonSerializerContext]` attribute
4. **Include projection and aggregate types in the JSON context** — if using `Query<T, TResult>` or `Aggregate<T, TResult>`, register both `T` and `TResult`
5. **Use streaming for large result sets** — prefer `GetAllStream`/`QueryStream` when processing results incrementally
6. **Create indexes for frequently queried properties** — use `store.CreateIndexAsync<T>(expr, jsonTypeInfo)` for up to 30x faster queries
7. **Use `Dictionary<string, object?>` for AOT-safe raw SQL parameters** — anonymous objects work but dictionaries are fully AOT-compatible
8. **Keep index management separate** — index methods are on `SqliteDocumentStore`, not `IDocumentStore`; cast or use the concrete type
