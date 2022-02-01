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
        private static readonly Dictionary<IServiceCollection, HashSet<string>> RegisteredClients = new();
        /// <summary>
        /// Add <see cref="CosmosClient"/> with id and <see cref="CosmosClientFactory"/> to access client by it's id.        
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="builder">The cosmos client configuration.</param>
        /// <param name="clientId">The cosmos client id. Will be used to get client from factory. Should be unique.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">In case of cosmos client with given id already added.</exception>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, Func<IServiceProvider, CosmosClientBuilder> builder, string clientId = CosmosClientFactory.DefaultCosmosClientId)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (!RegisteredClients.TryGetValue(services, out var clients))
            {
                clients = new HashSet<string>();
                RegisteredClients.Add(services, clients);
            }

            if (clients.Contains(clientId))
            {
                throw new InvalidOperationException($"CosmosClient with id {clientId} already registered");
            }

            clients.Add(clientId);

            services.TryAddSingleton(sp => new CosmosClientFactory(sp));

            services.AddSingleton(sp => new CosmosClientWrapper(new Lazy<CosmosClient>(() => builder(sp).Build(), LazyThreadSafetyMode.ExecutionAndPublication), clientId));
            
            return services;
        }

        /// <summary>
        /// Add <see cref="CosmosDatabase{TDatabase}"/> to service collection as a <see cref="ServiceLifetime.Singleton"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TDatabase">Type to reference database</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="createIfNotExists">Whether to create database if it not exists.</param>
        /// <param name="databaseId">The database id</param>
        /// <param name="throughput">Optional. The <see cref="ThroughputProperties"/> for the newly created database.</param>
        /// <param name="clientId">The cosmos client id to work with database. Should be registered in advance by executing <see cref="AddCosmosClient"/> method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">In case of cosmos client with given id not added in advance.</exception>
        public static IServiceCollection AddCosmosDatabase<TDatabase>(
            this IServiceCollection services,
            bool createIfNotExists,
            string databaseId,
            ThroughputProperties? throughput = null,
            string clientId = CosmosClientFactory.DefaultCosmosClientId)
        {
            return services.AddCosmosDatabase<TDatabase>(createIfNotExists, (sp) => databaseId, (sp) => throughput, clientId);
        }

        /// <summary>
        /// Add <see cref="CosmosDatabase{TDatabase}"/> to service collection as a <see cref="ServiceLifetime.Singleton"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TDatabase">Type to reference database</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="createIfNotExists">Whether to create database if it not exists.</param>
        /// <param name="databaseId">The database id</param>
        /// <param name="throughput">Optional. The <see cref="ThroughputProperties"/> for the newly created database.</param>
        /// <param name="clientId">The cosmos client id to work with database. Should be registered in advance by executing <see cref="AddCosmosClient"/> method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">In case of cosmos client with given id not added in advance.</exception>

        public static IServiceCollection AddCosmosDatabase<TDatabase>(
            this IServiceCollection services,
            bool createIfNotExists,
            Func<IServiceProvider, string> databaseId,
            Func<IServiceProvider, ThroughputProperties?>? throughputProperties = null,
            string clientId = CosmosClientFactory.DefaultCosmosClientId)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (databaseId is null)
            {
                throw new ArgumentNullException(nameof(databaseId));
            }

            if (RegisteredClients.TryGetValue(services, out var clients) && clients.Contains(clientId))
            {
                services.TryAddSingleton(sp =>
                {
                    Lazy<CosmosClient> client = sp.GetRequiredService<CosmosClientFactory>().GetCosmosClientLazy(clientId);
                    return new CosmosDatabase<TDatabase>(client, databaseId(sp), throughputProperties?.Invoke(sp), createIfNotExists);
                });
            }
            else
            {
                throw new InvalidOperationException($"CosmosClien with id {clientId} not registered. Call {nameof(AddCosmosClient)} method first");
            };

            return services;
        }

        /// <summary>
        /// Add <see cref="CosmosContainer{TContainer}"/> to service collection as a <see cref="ServiceLifetime.Singleton"/> if the service type hasn't already been registered. Container should be created in advance.
        /// </summary>
        /// <typeparam name="TDatabase">Type to reference database</typeparam>
        /// <typeparam name="TContainer">Type to reference container</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="containerId">The container/collection id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCosmosContainer<TDatabase, TContainer>(
            this IServiceCollection services,
            string containerId)
        {
            return services.AddCosmosContainer<TDatabase, TContainer>(false, sp => new ContainerProperties(containerId, "/any"));
        }

        /// <summary>
        /// Add <see cref="CosmosContainer{TContainer}"/> to service collection as a <see cref="ServiceLifetime.Singleton"/> if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TDatabase">Type to reference database</typeparam>
        /// <typeparam name="TContainer">Type to reference container</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="createIfNotExists">Whether to create container if it not exists.</param>
        /// <param name="containerProperties">The container properties.</param>
        /// <param name="throughputProperties">The <see cref="ThroughputProperties"/> for the newly created container.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
