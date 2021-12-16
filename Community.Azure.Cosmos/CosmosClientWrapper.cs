using Microsoft.Azure.Cosmos;

using System;
using System.Collections.Generic;
using System.Text;

namespace Community.Azure.Cosmos
{
    internal class CosmosClientWrapper
    {
        private readonly Lazy<CosmosClient> cosmosClient;

        public CosmosClientWrapper(Lazy<CosmosClient> cosmosClient, string id)
        {
            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public Lazy<CosmosClient> Value => this.cosmosClient;
        public string Id { get; }
    }
}
