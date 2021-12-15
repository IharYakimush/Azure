using Microsoft.Azure.Cosmos;

namespace Community.Azure.Cosmos
{
    internal class CosmosClientCollection : IDisposable
    {
        public const string DefaultId = "default";

        internal Dictionary<string,Lazy<CosmosClient>> Items = new Dictionary<string, Lazy<CosmosClient>>();

        public CosmosClient GetClient(string clientId = DefaultId)
        {
            return this.Items[clientId]?.Value ?? throw new InvalidOperationException($"CosmosClient with id {clientId} not registered");
        }

        void IDisposable.Dispose()
        {
            foreach (var item in Items)
            {
                if (item.Value.IsValueCreated)
                {
                    item.Value.Value.Dispose();
                }
            }
        }
    }
}
