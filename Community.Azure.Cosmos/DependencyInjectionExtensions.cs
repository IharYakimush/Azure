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

            CosmosClientCollection collection = services.SingleOrDefault(d => d.ServiceType == typeof(CosmosClientCollection) && d.ImplementationInstance as CosmosClientCollection != null)?.ImplementationInstance as CosmosClientCollection;

            if (collection is null)
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
            string databaseId, 
            string clientId = CosmosClientCollection.DefaultId)
        {
            return services.AddCosmosDatabase<TDatabase>(databaseId, false, null, clientId);
        }

        public static IServiceCollection AddCosmosDatabaseCreateIfNotExists<TDatabase>(
            this IServiceCollection services, 
            string databaseId, 
            ThroughputProperties throughput = null, 
            string clientId = CosmosClientCollection.DefaultId)
        {
            return services.AddCosmosDatabase<TDatabase>(databaseId, true, throughput, clientId);
        }

        private static IServiceCollection AddCosmosDatabase<TDatabase>(
            this IServiceCollection services, 
            string databaseId, 
            bool createIfNotExists, 
            ThroughputProperties throughput, 
            string clientId)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (databaseId is null)
            {
                throw new ArgumentNullException(nameof(databaseId));
            }

            CosmosClientCollection collection = services.SingleOrDefault(d => d.ServiceType == typeof(CosmosClientCollection) && d.ImplementationInstance as CosmosClientCollection != null)?.ImplementationInstance as CosmosClientCollection;

            if (collection != null && collection.Items.TryGetValue(clientId, out var client))
            {
                services.TryAddSingleton(new CosmosDatabase<TDatabase>(client, databaseId, throughput, createIfNotExists));
            }
            else
            {
                throw new InvalidOperationException($"CosmosClien with id {clientId} not registered. Call {nameof(AddCosmosClient)} method first");
            };

            return services;
        }

        public static IServiceCollection AddCosmosContainer<TDatabase, TContainer>(this IServiceCollection services, string containerId)
        {
            return services.AddCosmosContainer<TDatabase, TContainer>(false, containerId, null, null, null);
        }

        public static IServiceCollection AddCosmosContainerCreateIfNotExists<TDatabase, TContainer>(
            this IServiceCollection services, 
            string containerId, 
            string partitionKey, 
            Action<ContainerBuilder> containerSetup = null, 
            ThroughputProperties throughput = null)
        {
            return services.AddCosmosContainer<TDatabase, TContainer>(true, containerId, partitionKey, containerSetup, throughput);
        }

        private static IServiceCollection AddCosmosContainer<TDatabase, TContainer>(
            this IServiceCollection services, 
            bool allowCreate, 
            string containerId, 
            string partitionKey, 
            Action<ContainerBuilder> containerSetup, 
            ThroughputProperties throughput)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (containerId is null)
            {
                throw new ArgumentNullException(nameof(containerId));
            }

            if (allowCreate && partitionKey is null)
            {
                throw new ArgumentNullException(nameof(partitionKey));
            }

            if (!services.Any(d => d.ServiceType == typeof(CosmosDatabase<TDatabase>)))
            {
                throw new InvalidOperationException($"CosmosDatabase with type {typeof(CosmosDatabase<TDatabase>)} not added. Call {nameof(AddCosmosDatabase)} method first");
            }
           
            services.TryAddSingleton(sp =>
            {
                return new CosmosContainer<TContainer>(sp.GetRequiredService<CosmosDatabase<TDatabase>>(), allowCreate, containerId, partitionKey, containerSetup, throughput);
            });

            return services;
        }

        
    }
}
