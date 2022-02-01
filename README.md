# Community.Azure.Cosmos
Adds dependency injection capabilities for Cosmos database and containers. Add serializer utilized System.Text.Json
## Register
```
services.AddCosmosClient(sp => new CosmosClientBuilder(
    sp.GetRequiredService<IConfiguration>().GetConnectionString("CosmosDb"))            
            .WithCustomSerializer(
                new CosmosSystemTextJsonSerializer(
                    new System.Text.Json.JsonSerializerOptions() 
                    {
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    })));

services.AddCosmosDatabase<Db4>(true, "testDb4");

services.AddCosmosContainer<Db4, Cnt2>(true, (sp) => new ContainerProperties("new-test-cnt", "/pk"));
```
## Use
```
IServiceProvider sp;

// Get CosmosClient
Lazy<CosmosClient> client = sp.GetRequiredService<CosmosClientFactory>().GetCosmosClientLazy(clientId);

// Get Database
CosmosDatabase<Db4> db = sp.GetRequiredService<CosmosDatabase<Db4>>();

// Get Container
CosmosContainer<Cnt2> db = sp.GetRequiredService<CosmosContainer<Cnt2>>();
```