using Microsoft.Extensions.DependencyInjection;
using Community.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace Dam.Model.CosmosDb
{

    public static class CosmosDbServices
    {
        private class DefaultDatabase { }

        public static IServiceCollection AddCosmosDb(this IServiceCollection services)
        {
            services.AddCosmosClient(sp => new CosmosClientBuilder(sp.GetRequiredService<IConfiguration>().GetConnectionString("CosmosDb"))
            .WithCustomSerializer(
                new CosmosSystemTextJsonSerializer(
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    })));

            services.AddCosmosDatabase<DefaultDatabase>(false, "dam");

            services.AddCosmosContainer<DefaultDatabase, Tenant>(true, sp =>
            {
                ContainerProperties containerProperties = new ContainerProperties("tenant", Entity.PropertyName.Pk);
                containerProperties.IndexingPolicy.DefaultDamSetup(false);
                containerProperties.IndexingPolicy.ClearIndexingPaths();

                return containerProperties;
            });

            services.AddCosmosContainer<DefaultDatabase, TenantIndexed>(true, sp =>
            {
                ContainerProperties containerProperties = new ContainerProperties("tenant-indexed", Entity.PropertyPath.Type);
                IndexingPolicy indexingPolicy = containerProperties.IndexingPolicy;
                indexingPolicy.DefaultDamSetup(true);
                indexingPolicy.ClearIndexingPaths();
                indexingPolicy.IncludeEntityPaths();
                indexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = $"{Tenant.PropertyPath.Name}/?" });
                UniqueKey uniqueName = new UniqueKey();
                uniqueName.Paths.Add(TenantIndexed.PropertyPath.Name);
                containerProperties.UniqueKeyPolicy.UniqueKeys.Add(uniqueName);

                return containerProperties;
            });

            return services;
        }
    }
}
