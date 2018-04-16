# Correlator
Receive, Generate, Log and pass through correlation ids

## How to Use

```csharp
services.AddCorrelator()
```

When adding HttpClients to your container use the `AddCorrelationHeaders` fluent interface on the client to make sure any requests 
performed using the client get the correlation ids added to the headers

```csharp
services.AddHttpClient<ValuesClient>(client => client.BaseAddress = new Uri(Configuration["ValuesServiceUri"]))
        .AddCorrelationHeaders()
```
