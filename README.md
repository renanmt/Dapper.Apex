# Dapper.Apex

A new set of tools and extensions for Dapper that provides CRUD operations for your entities.

## How does it work?

Like Dapper does!

Simply add a refence to Dapper.Apex namespace in your repository classes and you get access to its methods via the database connection.

## Frameworks & Databases

Frameworks:
- .net 5
- .net standard 2.1

Databases:
- MS SQL Server
- MySql

> _More databases can be added to QueryHelper.SupportedDatabaseHelpers_

## Methods

Summary:

| Operation | Sync          | Async             |
| --------- | ------------- | ----------------- |
| GET       | Get           | GetAsync          |
| GET       | GetAll        | GetAllAsync       |
| GET       | GetCount      | GetCountAsync     |
| INSERT    | Insert        | InsertAsync       |
| INSERT    | InsertMany    | InsertManyAsync   |
| UPDATE    | Update        | UpdateAsync       |
| UPDATE    | UpdateMany    | UpdateManyAsync   |
| UPDATE    | UpdateFields  | UpdateFieldsAsync |
| UPDATE    | UpdateExcept  | UpdateExceptAsync |
| DELETE    | Delete        | DeleteAsync       |
| DELETE    | DeleteMany    | DeleteManyAsync   |
| DELETE    | DeleteAll     | DeleteAllAsync    |

## DapperApex class

### Initialize

The DapperApex class provides a special method that allows you to initialize all property information and query caches.

This is a very interesting feature that can be used during the application startup with two main advantages:
1. It pre-validates all your Enities and will throw errors if inconsistencies are found like:
    - Classes with no keys
    - Classes with multiple surrogate keys
    - Classes with mixed surrogate and natural keys
2. It eliminates the overhead of building property information and query caches during your first calls for each entity type.

```csharp
// Initializes a list of entity types
DapperApex.Initialize(entityTypeList, connection);
// Initializes all types of a specific namespace and assembly
DapperApex.Initialize(assembly, namespace, connection);
// Initializes all types of a specific namespace and assembly that derive from a given type
DapperApex.Initialize(assembly, namespace, typeof(BaseEntity), connection);
```

## Entity Attributes

Entity attributes are needed in some cases so Dapper.Apex can automatically create your queries.

### Table attribute

The Table attribue should be used when you need to define a specific table name for your entity. 

> _If you don't use the Table attribute, Dapper.Apex will assume the name of the entity class to be the table name._

```csharp
[Table("MyEntityTable")]
class Entity {
    ...
}
```

### Key attribute

The **Key** attribute is used to tell Dapper.Apex that a specific property is a **surrogate/artificial/db generated** key.

> _Only a max of one Key attribute is expected for each entity class._

```csharp
class Entity {
    [Key]
    public int EntityKey { get; set; }
    ...
}
```

If your entity class contains a property named **Id**, Dapper.Apex will assume it to be a **surrogate/artificial/db generated** key, 
unless you apply the attribute **ExplicitKey**. 

```csharp
class Entity {
    public int Id { get; set; } // Surrogate key
    ...
}

class Entity {
    [ExplicitKey]
    public int Id { get; set; } // Natural key
    ...
}
```

### ExplicitKey attribute

The **ExplicitKey** attribute is used to tell Dapper.Apex that specific property is a **natural/composite** key.

```csharp
class Entity {
    [ExplicitKey]
    public Guid EntityKey { get; set; }
    ...
}
```

or 

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}
```

### ReadOnly attribute

The **ReadOnly** attribute defines that a specific property must used only for data retrieval methods but ignored for data writing methods.

```csharp
class Entity {
    ...
    [ReadOnly]
    public string MyFieldName { get; get; }
    ...
}
```

### Computed attribute

The **Computed** attribute defines that a specific property will be completly ignored for all Dapper.Apex methods.

```csharp
class Entity {
    ...
    [Computed]
    public object MyComputedField { get; get; }
    ...
}
```

### GET methods

#### Get with tuple as key

You can use a Tuple to pass key values in the order they appear in your Entity class.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var tuple = ValueTuple.Create(123, "key");
var entity = connection.Get<Entity>(tuple);

// Async
var entity = await connection.GetAsync<Entity>(tuple);
```

#### Get with single key value

You can use a single value if your Entity class has a single key.

```csharp
var entity = connection.Get<Entity>(123);

// Async
var entity = await connection.GetAsync<Entity>(123);
```

#### Get with object as key

You can use any object that has properties with same of the keys in your Entity class.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var key = new Entity() { NumberId = 123, TextId = "key" };
var entity = connection.Get(key);

// Annonymous objects also work!

var key = new { NumberId = 123, TextId = "key" };
var entity = connection.Get<Entity>(key);

// Async
var entity = await connection.GetAsync<Entity>(key);
```

#### Get All

Gets all records of a specific entity.

```csharp
var entities = connection.GetAll<Entity>(key);

// Async
var entities = await connection.GetAllAsync<Entity>(key);
```

#### Get Count

Gets the total count of records of a specific entity.

```csharp
var count = connection.GetCount<Entity>();

// Async
var count = await connection.GetCountAsync<Entity>();
```

### INSERT methods

#### Single Insert

Use it when you need to insert a single entity object.

> _Entities with surrogate keys will have their objects automatically updated with the key value coming from the database operation._

```csharp
var entity = new Entity();
...
connection.Insert(entity);

// Async
connection.InsertAsync(entity);
```

#### Insert Many

Use it to easily insert several entity objects with a single method call.<br>
Works with any collections of objects.

> _Like the single insert operation, entities with surrogate keys will have their objects automatically updated with the key value coming from the database operation._

```csharp
var entityList = new List<Entity>() {...};
connection.InsertMany(entityList);

// Async
connection.InsertManyAsync(entityList);
```

### UPDATE methods

#### Single Update

Use it when you need to update a single entity object.

```csharp
connection.Update(entity);

// Async
await connection.UpdateAsync(entity);
```

#### Update Many

Use it to easily update several entity objects with a single method call.<br>
Works with any collections of objects.

```csharp
connection.UpdateMany(entityList);

// Async
await connection.UpdateManyAsync(entityList);
```

#### Update Fields

Use it when you wish to update only specific fields of the entity.

```csharp
var fieldsToUpdate = new List<string>() { { "Field1" }, { "Field2" } };
connection.UpdateFields(entity, fieldsToUpdate);

// Async
await connection.UpdateFieldsAsync(entity, fieldsToUpdate);
```

#### Update Except

Use it when you wish to update the entire entity except for certain fields.

```csharp
var fieldsToIgnore = new List<string>() { { "Field1" }, { "Field2" } };
connection.UpdateFields(entity, fieldsToIgnore);

// Async
await connection.UpdateFieldsAsync(entity, fieldsToIgnore);
```

### DELETE methods

#### Delete with tuple as key

You can use a Tuple to pass key values in the order they appear in your Entity class.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var tuple = ValueTuple.Create(123, "key");
connection.Delete<Entity>(tuple);

// Async
await connection.DeleteAsync<Entity>(tuple);
```

#### Single Delete

Use it to delete a single entity object from the database.

```csharp
connection.Delete(entity);

// Async
await connection.DeleteAsync(entity);
```

#### Delete Many

Use it to easily delete several entity objects with a single method call.<br>
Works with any collections of objects.

```csharp
connection.DeleteMany(entityList);

// Async
await connection.DeleteManyAsync(entityList);
```

#### Delete All

Deletes all records of a specific entity.

```csharp
connection.DeleteAll<Entity>();

// Async
await connection.DeleteAllAsync<Entity>();
```

### Insert Operation control

Depending on your needs, you may want to choose between sending all the insert operations in a single SQL statement or do inserts one by one.

For most cases the performance change is not significant, but if network latency and/or number of entities are a factor, the right operation mode may save you some valuable waiting time.

The operation modes are basically two:
* **SingleShot:** _Sends all insert operations in a single statement to the database._<br>
This mode uses more CPU to craft the SQL statement based on the amount of entities for the operation.

* **OneByOne:** _Sends all insert operations one by one to the database._<br>
This mode uses will sends a number of calls to the database based on the amount of entities for the operation.

> _By default, all **InsertMany** operations will use the One by One method._

```csharp
connection.InsertMany(entityList, operationMode: OperationMode.SingleShot);
connection.InsertMany(entityList, operationMode: OperationMode.OneByOne);
```

## Why another library?

> **TLDR:** I was bored :unamused: and wanted to build something useful :smirk:.

By being a heavy user of Dapper and Dapper.Contrib, I've found myself working way too much around some of Dapper.Contrib problems and lack of features.

Because some unsuccessful merge requests I made in the original project (one waiting to be merged for years :older_man:), 
I decided to try to create something to replace Dapper.Contrib entirely in my projects without the need for much refactoring.
