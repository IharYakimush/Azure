using Microsoft.Azure.Cosmos;

namespace Community.Azure.Cosmos
{
    public class CosmosDatabase<TDatabase> : ICosmosDatabase
    {
        private readonly Lazy<Database> database;

        internal CosmosDatabase(Lazy<CosmosClient> cosmosClient, string databaseId, ThroughputProperties? throughput, bool allowCreate)
        {
            if (cosmosClient is null)
            {
                throw new ArgumentNullException(nameof(cosmosClient));
            }

            if (databaseId is null)
            {
                throw new ArgumentNullException(nameof(databaseId));
            }

            Func<Database> dbFactory;

            if (!allowCreate)
            {
                dbFactory = () => cosmosClient.Value.GetDatabase(databaseId);
            }
            else if (throughput is null)
            {
                dbFactory = () => cosmosClient.Value.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            }
            else
            {
                dbFactory = () => cosmosClient.Value.CreateDatabaseIfNotExistsAsync(databaseId, throughput).Result;
            }

            this.database = new Lazy<Database>(dbFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Database Value => this.database.Value;       
    }
}