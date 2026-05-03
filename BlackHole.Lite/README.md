# BlackHole.Lite

A fast, code-first ORM for SQLite — Entity Framework-style productivity, without a `DbContext`.

[![NuGet](https://img.shields.io/nuget/v/BlackHole.Core.Lite.svg)](https://www.nuget.org/packages/BlackHole.Core.Lite)
[![Downloads](https://img.shields.io/nuget/dt/BlackHole.Core.Lite.svg)](https://www.nuget.org/packages/BlackHole.Core.Lite)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

Define your entities, call `SuperNova` once at startup, and BlackHole.Lite creates and migrates the SQLite database for you. No context classes, no migration files, no boilerplate.

---

## Why BlackHole.Lite

- **Zero ceremony.** No `DbContext`, no migration commands, no scaffolding.
- **Schema lives in code.** Add a property, restart the app — the column appears.
- **Familiar API.** `HasOne`, `WithMany`, `HasForeignKey`, `Include`, `ThenInclude` — if you know EF Core, you already know this.
- **Multi-database support.** Register many SQLite files and route entities by namespace.
- **Raw SQL escape hatch.** `BHConnection` for the cases LINQ can't express.
- **Tiny footprint.** Only depends on `Microsoft.Data.Sqlite.Core` and `SQLitePCLRaw`.

---

## Installation

```bash
dotnet add package BlackHole.Core.Lite
```

Requires .NET 8 or higher. SQLite is bundled — nothing to install on the host.

---

## Quick Start

### 1. Define an entity

```csharp
using BlackHole.Entities;

public class User : BHEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
}
```

`BHEntity` provides an auto-increment `Id` primary key.

### 2. Bootstrap once at startup

```csharp
using BlackHole.Configuration;

BlackHoleConfiguration.SuperNova(settings =>
{
    settings.AddDatabase("MyAppDb");
});
```

That single call scans the calling assembly for entity classes, creates the SQLite file, and builds (or alters) the tables.

### 3. Use it

```csharp
using BlackHole.Core;

var users = BHDataProvider.For<User>();

users.InsertEntry(new User { Name = "Mike", Email = "mike@example.com", Age = 32 });

var mike    = users.GetEntryWhere(u => u.Email == "mike@example.com");
var adults  = users.GetAllEntriesWhere(u => u.Age >= 18);
var hasAnna = users.AnyEntry(u => u.Name == "Anna");
```

---

## Relationships

Derive from the generic `BHEntity<T>` to configure foreign keys, delete behaviors, and navigation properties.

```csharp
public class Order : BHEntity<Order>
{
    public int CustomerId { get; set; }
    public DateTime PlacedAt { get; set; }
    public decimal Total { get; set; }

    public BHIncludeItem<Customer>? Customer { get; set; }

    public override void Configure(RelationBuilder<Order> modelBuilder)
    {
        modelBuilder.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(OnDeleteBehavior.Cascade);
    }
}

public class Customer : BHEntity<Customer>
{
    public string Name { get; set; } = string.Empty;
    public BHIncludeList<Order>? Orders { get; set; }

    public override void Configure(RelationBuilder<Customer> modelBuilder) { }
}
```

Eager-load related entities with `Include` / `ThenInclude`:

```csharp
var orders = BHDataProvider.For<Order>()
    .Include(o => o.Customer)
    .GetAllEntries();

var customers = BHDataProvider.For<Customer>()
    .Include(c => c.Orders)
        .ThenInclude(o => o.Items)
    .GetAllEntries();
```

---

## Attributes

Fine-tune the generated schema with property-level attributes:

| Attribute                      | Effect                                                                |
| ------------------------------ | --------------------------------------------------------------------- |
| `[Unique]`                     | Adds a UNIQUE constraint on the column.                               |
| `[NotNullable]`                | Forces NOT NULL on a reference type column.                           |
| `[VarCharSize(n)]`             | Caps a string column to `n` characters.                               |
| `[ForeignKey(typeof(T))]`      | Marks the column as a foreign key to entity `T`.                      |
| `[UseActivator]` *(class-level)* | Enables soft-delete. `Delete*` calls flip `Inactive=1` instead of removing. |

```csharp
[UseActivator]
public class Customer : BHEntity
{
    [Unique]
    [VarCharSize(120)]
    public string Email { get; set; } = string.Empty;

    [NotNullable]
    public string DisplayName { get; set; } = string.Empty;
}
```

---

## Joins & Stored Views

For ad-hoc joins that project into a custom shape, build a `BlackHoleDto`:

```csharp
public class OrderRow : BlackHoleDto
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

var rows = BHDataProvider
    .InnerJoin<Order, Customer>()
    .On(o => o.CustomerId, c => c.Id)
    .Where((o, c) => o.Total > 100)
    .Cast<OrderRow>()
    .SelectAll();
```

If you run the same join often, register it once at startup with `IBHInitialViews` and call it later by DTO type:

```csharp
List<OrderRow> rows = BHDataProvider.ExecuteView<OrderRow>();
```

Available join types: `InnerJoin`, `LeftJoin`, `RightJoin`, `OuterJoin`.

---

## Transactions

```csharp
using var tx = new BHTransaction();

var customers = BHDataProvider.For<Customer>();
var orders    = BHDataProvider.For<Order>();

int customerId = customers.InsertEntry(new Customer { Name = "Mike" }, tx);
orders.InsertEntry(new Order { CustomerId = customerId, Total = 99.99m }, tx);

tx.Commit();
```

If you forget to commit, the transaction auto-rolls back on `Dispose`. Every CRUD method has an overload that accepts a `BHTransaction`.

---

## Raw SQL

```csharp
var conn = BHDataProvider.GetConnection();

var args = new BHParameters();
args.Add("minAge", 18);

var adults = conn.Query<User>(
    "SELECT * FROM \"User\" WHERE Age >= @minAge",
    args);

int total = conn.QueryFirst<int>("SELECT COUNT(*) FROM \"Order\"", null);
```

---

## Multiple Databases

Register more than one database and route entities to them by namespace:

```csharp
BlackHoleConfiguration.SuperNova(settings =>
{
    settings.AddDatabase("UsersDb",    "MyApp.Domain.Users");
    settings.AddDatabase("ProductsDb", "MyApp.Domain.Products");
});

var users = BHDataProvider.For<User>("UsersDb");
var skus  = BHDataProvider.For<Sku>("ProductsDb");
```

---

## Soft Delete

Decorate an entity class with `[UseActivator]` and `Delete*` calls flip `Inactive = 1` instead of removing the row. `GetAll*` calls automatically hide inactive rows.

```csharp
[UseActivator]
public class Account : BHEntity { /* ... */ }

var accounts = BHDataProvider.For<Account>();
accounts.DeleteEntryById(42);          // sets Inactive = 1
var live = accounts.GetAllEntries();   // id=42 not returned
```

---

## Seed Data

Implement `IBHInitialData` to insert default rows the first time the database is created:

```csharp
public class DefaultRoles : IBHInitialData
{
    public void SeedData(BHDataInitializer initializer)
    {
        initializer.AddData(new Role { Name = "Admin" });
        initializer.AddData(new Role { Name = "User" });
    }
}
```

The seeder runs automatically on first DB creation. Subsequent runs do nothing.

---

## Documentation

Full documentation, with examples for every feature, is available at the project site:
**[mikarsoft.com](https://mikarsoft.com)**

---

## Requirements

- .NET 8.0+
- No external SQLite install required (uses `SQLitePCLRaw.bundle_e_sqlite3`)

---

## License

[MIT](LICENSE.md) © Mikarsoft Ltd

---

## Links

- **NuGet:** [BlackHole.Core.Lite](https://www.nuget.org/packages/BlackHole.Core.Lite)
- **Source:** [github.com/Mikarsoft/BlackHole.Core.Lite](https://github.com/Mikarsoft/BlackHole.Core.Lite)
- **Issues & feedback:** [GitHub Issues](https://github.com/Mikarsoft/BlackHole.Core.Lite/issues)
