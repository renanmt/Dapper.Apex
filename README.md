# Dapper.Apex ![Dapper.Apex](icon.png)

A new set of tools and extensions for Dapper that provides CRUD operations for your entities.

## Install

Install it directly from Nuget package manager in visual studio.

**Nuget Url**: 
https://www.nuget.org/packages/Dapper.Apex/

## How does it work?

**Like Dapper does!**

Simply add a refence to Dapper.Apex namespace in your repository classes and you get access to its extension methods via the database connection.

## Frameworks & Databases

Frameworks:
- .net 5
- .net standard 2.1

Databases:
- MS SQL Server
- MySql

> _More databases can be added to QueryHelper.SupportedDatabaseHelpers_

## Contents

- [Methods](#methods)
- [DapperApex class](#dapperapex-class)
  - [Initialize](#initialize)
- [Entity Attributes](#entity-attributes)
  - [Table Attribute](#table-attribute)
  - [Key Attribute](#key-attribute)
  - [ExplicitKey Attribute](#explicit-key-attribute)
  - [ReadOnly Attribute](#readonly-attribute)
  - [Computed Attribute](#computed-attribute)
- [Methods](#methods)
  - [GET methods](#get-methods)
    - [Get](#get)
      - [Get with single key value](#get-single-key)
      - [Get with tuple as key](#get-tuple)
      - [Get with array as key](#get-array)
      - [Get with collection as key](#get-collection)
      - [Get with Dictionary as key](#get-dictionary)
      - [Get with ExpandoObject as key](#get-expando)
      - [Get with object as key](#get-object)
    - [Get All](#get-all)
  - [INSERT methods](#insert-methods)
    - [Single Insert](#insert)
    - [Insert Many](#insert-many)
  - [UPDATE methods](#update-methods)
    - [Single Update](#update)
    - [Update Many](#update-many)
    - [Update Fields](#update-fields)
    - [Update Except](#update-except)
  - [DELETE methods](#delete-methods)
    - [Delete with a key](#delete-key)
    - [Single Delete](#delete)
    - [Delete Many](#delete-many)
  - [COUNT](#count)
  - [EXISTS](#exists)
- [Insert Operation Control](#insert-operation-control)
- [Why another library?](#why-another-library)

## Methods {#methods}

Summary:

| Operation | Sync          | Async             |
| --------- | ------------- | ----------------- |
| GET       | Get           | GetAsync          |
| GET       | GetAll        | GetAllAsync       |
| INSERT    | Insert        | InsertAsync       |
| INSERT    | InsertMany    | InsertManyAsync   |
| UPDATE    | Update        | UpdateAsync       |
| UPDATE    | UpdateMany    | UpdateManyAsync   |
| UPDATE    | UpdateFields  | UpdateFieldsAsync |
| UPDATE    | UpdateExcept  | UpdateExceptAsync |
| DELETE    | Delete        | DeleteAsync       |
| DELETE    | DeleteMany    | DeleteManyAsync   |
| DELETE    | DeleteAll     | DeleteAllAsync    |
| COUNT     | Count         | CountAsync        |
| EXISTS    | Exists        | ExistsAsync       |

## DapperApex class {#dapperapex-class}

### Initialize {#initialize}

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

## Entity Attributes {#entity-attributes}

Entity attributes are needed in some cases so Dapper.Apex can automatically create your queries.

### Table Attribute {#table-attribute}

The Table attribue should be used when you need to define a specific table name for your entity. 

> _If you don't use the Table attribute, Dapper.Apex will assume the name of the entity class to be the table name._

```csharp
[Table("MyEntityTable")]
class Entity {
    ...
}
```

### Key Attribute {#key-attribute}

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

### ExplicitKey Attribute {#explicit-key-attribute}

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

### ReadOnly Attribute {#readonly-attribute}

The **ReadOnly** attribute defines that a specific property must used only for data retrieval methods but ignored for data writing methods.

```csharp
class Entity {
    ...
    [ReadOnly]
    public string MyFieldName { get; get; }
    ...
}
```

### Computed Attribute {#computed-attribute}

The **Computed** attribute defines that a specific property will be completly ignored for all Dapper.Apex methods.

```csharp
class Entity {
    ...
    [Computed]
    public object MyComputedField { get; get; }
    ...
}
```

## Methods {#methods}

### GET methods {#get-methods}

#### Get {#get}

The Get method can be used with many different types of key objects to retrieve your entities.

Currently supported: single value types, tuples, arrays, collections, dictionaries, ExpandoObject, 
annonymous objects or any other object containing properties with the same names of your entity keys.


##### Get with single key value {#get-single-key}

You can use a single value if your Entity class has a single key.

```csharp
class Entity {
    public int Id { get; get; }
    ...
}

var entity = connection.Get<Entity>(123);

// Async
var entity = await connection.GetAsync<Entity>(123);
```

##### Get with tuple as key {#get-tuple}

You can use a tuple as a key to retrieve your entities.

The values must appear in the tuple in the same order they appear in your entity model.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var key = ValueTuple.Create(123, "key");
var entity = connection.Get<Entity>(key);

// Async
var entity = await connection.GetAsync<Entity>(key);
```

##### Get with array as key {#get-array}

You can use an array as a key to retrieve your entities.

The values must appear in the array in the same order they appear in your entity model.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var key = new object[] { 123, "key" };
var entity = connection.Get<Entity>(key);

// Async
var entity = await connection.GetAsync<Entity>(key);
```

##### Get with collection as key {#get-collection}

You can use a collection of single items that implements IEnumerable as a key to retrieve your entities.

The values must appear in the collection in the same order they appear in your entity model.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var key = new List<object>() { 123, "key" };
var entity = connection.Get<Entity>(key);

// Async
var entity = await connection.GetAsync<Entity>(key);
```

##### Get with Dictionary as key {#get-dictionary}

You can use a dictionary as a key to retrieve your entities. 

The dictionary has to contain keys with the same name of your entity keys.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var key = new Dictionary<string, object>();
key.Add("NumberId", 123);
key.Add("TextId", "key");
var entity = connection.Get<Entity>(key);

// Async
var entity = await connection.GetAsync<Entity>(key);
```

##### Get with ExpandoObject as key {#get-expando}

You can use an ExpandoObject as a key to retrieve your entities. 

The ExpandoObject has to contain keys with the same name of your entity keys.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

dynamic key = new ExpandoObject();
key.NumberId = 123;
key.TextId = "key";
var entity = connection.Get<Entity>(key as ExpandoObject);


// Async
var entity = await connection.GetAsync<Entity>(key);
```

##### Get with object as key {#get-object}

You can use any object that has properties with the same name of the keys in your Entity class.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

//
// Anonymous Object
//
var key = new { NumerId = 123, TextId = "key" };
var entity = connection.Get<Entity>(key);

//
// Entity / any other object
//
var key = new Entity() { NumerId = 123, TextId = "key" };
var entity = connection.Get(key);

// Async
var entity = await connection.GetAsync<Entity>(key);
```

#### Get All {#get-all}

Gets all records of a specific entity.

```csharp
var entities = connection.GetAll<Entity>(key);

// Async
var entities = await connection.GetAllAsync<Entity>(key);
```

### INSERT methods {#insert-methods}

#### Single Insert {#insert}

Use it when you need to insert a single entity object.

> _Entities with surrogate keys will have their objects automatically updated with the key value coming from the database operation._

```csharp
var entity = new Entity();
...
connection.Insert(entity);

// Async
connection.InsertAsync(entity);
```

#### Insert Many {#insert-many}

Use it to easily insert several entity objects with a single method call.<br>
Works with any collections of objects.

> _Like the single insert operation, entities with surrogate keys will have their objects automatically updated with the key value coming from the database operation._

```csharp
var entityList = new List<Entity>() {...};
connection.InsertMany(entityList);

// Async
connection.InsertManyAsync(entityList);
```

### UPDATE methods {#update-methods}

#### Single Update {#update}

Use it when you need to update a single entity object.

`Update` will return `true` if the entity was found and successfully updated.

```csharp
var updated = connection.Update(entity);

// Async
var updated = await connection.UpdateAsync(entity);
```

#### Update Many {#update-many}

Use it to easily update several entity objects with a single method call.<br>
Works with any collections of objects.

```csharp
connection.UpdateMany(entityList);

// Async
await connection.UpdateManyAsync(entityList);
```

#### Update Fields {#update-fields}

Use it when you wish to update only specific fields of the entity.

`UpdateFields` will return `true` if the entity was found and successfully deleted.

```csharp
var fieldsToUpdate = new List<string>() { { "Field1" }, { "Field2" } };
var updated = connection.UpdateFields(entity, fieldsToUpdate);

// Async
await connection.UpdateFieldsAsync(entity, fieldsToUpdate);
```

#### Update Except {#update-except}

Use it when you wish to update the entire entity except for certain fields.

`UpdateExcept` will return `true` if the entity was found and successfully deleted.

```csharp
var fieldsToIgnore = new List<string>() { { "Field1" }, { "Field2" } };
var updated = connection.UpdateExcept(entity, fieldsToIgnore);

// Async
var updated = await connection.UpdateExceptAsync(entity, fieldsToIgnore);
```

### DELETE methods {#delete-methods}

#### Delete with a key {#delete-key}

Like [Get](#get-methods) methods, you can use lots of different types of object as a key to fetch you entities.

[Single values](#get-single-key), [Tuples](#get-tuple), [Arrays](#get-array), [Collections](#get-collection), 
[Dictionaries](#get-dictionary), [ExpandoObject](#get-expando), [Objects in general](#get-object) are all accepted 
as key parameters.

Delete will return `true` if the entity was found and successfully deleted.

```csharp
class Entity {
    [ExplicitKey]
    public int NumberId { get; get; }
    [ExplicitKey]
    public string TextId { get; get; }
    ...
}

var key = ValueTuple.Create(123, "key");
var deleted = connection.Delete<Entity>(key);

// Async
var deleted = await connection.DeleteAsync<Entity>(tuple);
```

#### Single Delete {#delete}

Use it to delete a single entity object from the database.

Delete will return `true` if the entity was found and successfully deleted.

```csharp
var deleted = connection.Delete(entity);

// Async
var deleted = await connection.DeleteAsync(entity);
```

#### Delete Many {#delete-many}

Use it to easily delete several entity objects with a single method call.<br>
Works with any collections of objects.

Returns `true` if all entities where found and successfully deleted.

```csharp
var deleted = connection.DeleteMany(entityList);

// Async
var deleted = await connection.DeleteManyAsync(entityList);
```

#### Delete All {#delete-all}

Deletes all records of a specific entity.

Returns the total number of deleted records.

```csharp
var deletedCount = connection.DeleteAll<Entity>();

// Async
var deletedCount = await connection.DeleteAllAsync<Entity>();
```

### COUNT {#count}

Retrieves the total number of records of a specific entity.

```csharp
var count = connection.Count<Entity>();

// Async
var count = await connection.CountAsync<Entity>();
```

### EXISTS {#exists}

Checks if an entity exists in the database.

Returns `true` if the entity is found.

```csharp
var exists = connection.Exists<Entity>();

// Async
var exists = await connection.ExistsAsync<Entity>();
```

## Insert Operation control {#insert-operation-control}

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

## Why another library? {#why-another-library}

> **TLDR:** I was bored :unamused: and wanted to build something useful :smirk:.

By being a heavy user of Dapper and Dapper.Contrib, I've found myself working way too much around some of Dapper.Contrib problems and lack of features.

Because some unsuccessful merge requests I made in the original project (one waiting to be merged for years :older_man:), 
I decided to try to create something to replace Dapper.Contrib entirely in my projects without the need for much refactoring.
