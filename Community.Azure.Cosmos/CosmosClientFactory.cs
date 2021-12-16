using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Text;

namespace Community.Azure.Cosmos
{    
    public class CosmosClientFactory
    {
        internal const string DefaultCosmosClientId = "default";

        private readonly IServiceProvider serviceProvider;

        public CosmosClientFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        public CosmosClient GetCosmosClient(string id = DefaultCosmosClientId)
        {
            return GetCosmosClientLazy(id).Value;
        }

        public Lazy<CosmosClient> GetCosmosClientLazy(string id = DefaultCosmosClientId)
        {
            foreach (CosmosClientWrapper? item in this.serviceProvider.GetServices(typeof(CosmosClientWrapper)))
            {
                if (item?.Id == id)
                {
                    return item.Value;
                }
            }

            throw new InvalidOperationException($"CosmosClient with id {id} not registered");
        }
    }
}
