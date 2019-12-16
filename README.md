# GraphQL Helper Methods

This library contains a bunch of methods to make my life as developer easier. Some common tasks have been abstracted away in reusable and composable sections.

## Functionality

The main reason to use this library are the following methods:

- `Slice()` - An extension method to the `Query` object (SqlKata) which applies cursor based pagination to the provided query.
- `ToConnection()` - An extension method to the `Query` object which applies pagination (using the `Slice` method) and returns a `Connection` object.
- `EntityLoader()` - An extension method on the `IDataLoaderContextAccessor` to retrieve a single value via the data-loader based on a LINQ predicate. The loader will combine all similiar requests.
- `EntityCollectionLoader()` - An extension method on the `IDataLoaderContextAccessor` to retrieve multiple values based on a LINQ predicate. The loader will combine all similiar requests.
- A more natural way to add authorization to queries. [A detailed blog post explaining this functionality is available here](https://corstianboerman.com/2019-12-16/graphql-dotnet-authorization.html).

## Usage

### Slice

```csharp
new Query("Companies")
    .Where("City", "Amsterdam")
    .Slice(
        after: "cursor",
        first: 25);
```

### ToConnection

```csharp
Connection<Company>()
    .Name("companies")
    .ResolveAsync(async context => {
       return await new Query("Companies")
            .Where("City", "Amsterdam")
            .ToConnection<Company, object>(context);
    });
```

### EntityLoader

```csharp
Field<AircraftType, Aircraft>()
    .Name("aircraft")
    .ResolveAsync(async context =>
    {
        return await dataLoader.EntityLoader(
            dbProvider.Get().Aircraft,
            aircraft => aircraft.Id,
            context.Source.AirfieldId);
    });
```

### EntityCollectionLoader

```csharp
List<Flight> flights = await dataLoader.EntityCollectionLoader(
    dbProvider.Get().Flights,
    flight => flight.AircraftId,
    context.Source.Id)
```

## Installation
You can copy some specific code over from this repository or you can install the whole package via [NuGet](https://www.nuget.org/packages/Boerman.GraphQL.Contrib/).

```
Install-Package Boerman.GraphQL.Contrib
```

or 

```
dotnet add package Boerman.GraphQL.Contrib
```

## Dependencies
This library has quite a few dependencies. This comes from my own workflow where I start using Entity Framework, and use SqlKata for some specialized logic. Because Entity Framework acts as my base I also reuse the data models with SqlKata.

## Further content
All of this functionality has been documented over the course of several blog posts:

- [Implementing pagination with GraphQL.NET and Relay](http://corstianboerman.com/2019-03-08/implementing-pagination-with-graphql-net-and-relay.html)
- [Cursor based pagination with SQL Server using C# (and SQL)](http://corstianboerman.com/2019-03-06/cursor-based-pagination-with-sql-server.html)
- [Generic data loaders for Entity Framework in GraphQL](http://corstianboerman.com/posts/2019-02-12/generic-data-loaders-for-entity-framework-in-graphql.html)
