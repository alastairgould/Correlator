# Correlator
Incredibly simple middleware for handling correlation headers.

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
