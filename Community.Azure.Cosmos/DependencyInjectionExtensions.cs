using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Community.Azure.Cosmos
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, CosmosClientBuilder builder, string clientId = CosmosClientCollection.DefaultId)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (services.SingleOrDefault(d => d.ServiceType == typeof(CosmosClientCollection) && d.ImplementationInstance as CosmosClientCollection != null)?.ImplementationInstance is not CosmosClientCollection collection)
            {
                collection = new CosmosClientCollection();
                services.AddSingleton(collection);
            }

            if (!collection.Items.ContainsKey(clientId))
            {
                collection.Items.Add(clientId, new Lazy<CosmosClient>(() => builder.Build(), LazyThreadSafetyMode.ExecutionAndPublication));
            }
            
            return services;
        }

        public static IServiceCollection AddCosmosDatabase<TDatabase>(
            this IServiceCollection services,
            bool createIfNotExists,
            string databaseId,
            ThroughputProperties? throughput = null,
            string clientId = CosmosClientCollection.DefaultId)
        {
            return services.AddCosmosDatabase<TDatabase>(createIfNotExists, (sp) => databaseId, (sp) => throughput, clientId);
        }
        
        public static IServiceCollection AddCosmosDatabase<TDatabase>(
            this IServiceCollection services,             
            bool createIfNotExists,
            Func<IServiceProvider, string> databaseId,
            Func<IServiceProvider, ThroughputProperties?>? throughputProperties = null,
            string clientId = CosmosClientCollection.DefaultId)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (databaseId is null)
            {
                throw new ArgumentNullException(nameof(databaseId));
            }

            if (services.SingleOrDefault(d => d.ServiceType == typeof(CosmosClientCollection) && d.ImplementationInstance as CosmosClientCollection != null)?.ImplementationInstance is CosmosClientCollection collection && collection.Items.TryGetValue(clientId, out var client))
            {
                services.TryAddSingleton(sp => new CosmosDatabase<TDatabase>(client, databaseId(sp), throughputProperties?.Invoke(sp), createIfNotExists));
            }
            else
            {
                throw new InvalidOperationException($"CosmosClien with id {clientId} not registered. Call {nameof(AddCosmosClient)} method first");
            };

            return services;
        }

        public static IServiceCollection AddCosmosContainer<TDatabase, TContainer>(
            this IServiceCollection services,
            string containerId)
        {
            return services.AddCosmosContainer<TDatabase, TContainer>(false, sp => new ContainerProperties(containerId, "/any"));
        }

        public static IServiceCollection AddCosmosContainer<TDatabase, TContainer>(
            this IServiceCollection services, 
            bool createIfNotExists, 
            Func<IServiceProvider, ContainerProperties> containerProperties,
            Func<IServiceProvider, ThroughputProperties>? throughputProperties = null)
        {            
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }            

            if (!services.Any(d => d.ServiceType == typeof(CosmosDatabase<TDatabase>)))
            {
                throw new InvalidOperationException($"CosmosDatabase with type {typeof(CosmosDatabase<TDatabase>)} not added. Call {nameof(AddCosmosDatabase)} method first");
            }
           
            services.TryAddSingleton(sp =>
            {
                return new CosmosContainer<TContainer>(sp.GetRequiredService<CosmosDatabase<TDatabase>>(), createIfNotExists, containerProperties(sp), throughputProperties?.Invoke(sp));
            });

            return services;
        }        
    }
}
