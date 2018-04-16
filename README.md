# Correlator
Incredibly simple middleware for receiving, logging and sending correlation headers for asp.net core 2.1 and higher

## Why

Correlation header middleware already exists, but since asp.net core 2.1 things like HTTPClientFactory has made the implementation of such middleware easier. Instead of building my own infrastructure i'm utilising asp.net core 2.1 as much as possible to create a really simple implementation. The asp.net core 2.1 features also make it easier to use from an API perspective.

It also has sane defaults and works out the box with very little work from the programmer.

## Requirements

ASP.NET Core 2.1 or higher.

## How to Use

To add the middleware

```csharp
services.AddCorrelator()
```

When adding HttpClients to your container use the `AddCorrelationHeaders` fluent interface on the client to make sure any requests 
performed using the client get the correlation id added to the headers

```csharp
services.AddHttpClient<ValuesClient>(client => client.BaseAddress = new Uri(Configuration["ValuesServiceUri"]))
        .AddCorrelationHeaders()
```
