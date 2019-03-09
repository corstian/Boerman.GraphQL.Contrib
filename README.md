# GraphQL Helper Methods

This library contains a bunch of methods to make my life as developer easier. Some common tasks have been abstracted away in reusable and composable sections.

## Functionality

The main reason to use this library are the following methods:

- `Slice()` - An extension method to the `Query` object (SqlKata) which applies cursor based pagination to the provided query.
- `ToConnection()` - An extension method to the `Query` object which applies pagination (using the `Slice` method) and returns a `Connection` object.
- `EntityLoader()` - An extension method on the `IDataLoaderContextAccessor` to retrieve a single value via the data-loader based on a LINQ predicate. The loader will combine all similiar requests.
- `EntityCollectionLoader()` - An extension method on the `IDataLoaderContextAccessor` to retrieve multiple values based on a LINQ predicate. The loader will combine all similiar requests.

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
You can copy some specific code over from this repository or you can install the whole package via NuGet.

## Dependencies
This library has quite a few dependencies. This comes from my own workflow where I start using Entity Framework, and use SqlKata for some specialized logic. Because Entity Framework acts as my base I also reuse the data models with SqlKata.

## Further content
All of this functionality has been documented over the course of several blog posts:

