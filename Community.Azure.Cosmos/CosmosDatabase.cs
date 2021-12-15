using Microsoft.Azure.Cosmos;

namespace Community.Azure.Cosmos
{
    public class CosmosDatabase<TDatabase> : ICosmosDatabase
    {
        private Lazy<Database> database;

        private readonly Func<Database> dbFactory;
        internal CosmosDatabase(Lazy<CosmosClient> cosmosClient, string databaseId, ThroughputProperties throughput, bool allowCreate)
        {
            if (cosmosClient is null)
            {
                throw new ArgumentNullException(nameof(cosmosClient));
            }

            if (databaseId is null)
            {
                throw new ArgumentNullException(nameof(databaseId));
            }
            
            if (!allowCreate)
            {
                this.dbFactory = () => cosmosClient.Value.GetDatabase(databaseId);
            }
            else if (throughput is null)
            {
                this.dbFactory = () => cosmosClient.Value.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            }
            else
            {
                this.dbFactory = () => cosmosClient.Value.CreateDatabaseIfNotExistsAsync(databaseId, throughput).Result;
            }

            this.Reload();
        }

        public Database Value => this.database.Value;

        private void Reload()
        {
            this.database = new Lazy<Database>(dbFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }        
    }
}