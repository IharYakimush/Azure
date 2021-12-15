using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace Community.Azure.Cosmos
{
    public class CosmosContainer<TContainer> 
    {
        private readonly Func<Container> containerFactory;
        private Lazy<Container> container;
        internal CosmosContainer(ICosmosDatabase cosmosDatabase, bool allowCreate, string id, string partitionKey, Action<ContainerBuilder> containerSetup, ThroughputProperties throughput)
        {
            if (cosmosDatabase is null)
            {
                throw new ArgumentNullException(nameof(cosmosDatabase));
            }

            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (!allowCreate)
            {
                this.containerFactory = () => cosmosDatabase.Value.GetContainer(id);
            }
            else 
            {
                if (partitionKey is null)
                {
                    throw new ArgumentNullException(nameof(partitionKey));
                }

                this.containerFactory = () =>
                {
                    ContainerBuilder builder = new ContainerBuilder(cosmosDatabase.Value, id, partitionKey);
                    containerSetup?.Invoke(builder);

                    if (throughput == null)
                    {
                        return builder.CreateIfNotExistsAsync().Result;
                    }
                    else
                    {
                        return builder.CreateIfNotExistsAsync(throughput).Result;
                    }
                };
            }            

            this.Reload();
        }

        public Container Value => this.container.Value;

        private void Reload()
        {
            this.container = new Lazy<Container>(this.containerFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }
}