using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace Community.Azure.Cosmos
{
    public class CosmosContainer<TContainer> 
    {
        private readonly Lazy<Container> container;
        internal CosmosContainer(ICosmosDatabase cosmosDatabase, bool allowCreate, ContainerProperties containerProperties, ThroughputProperties? throughput)
        {
            if (cosmosDatabase is null)
            {
                throw new ArgumentNullException(nameof(cosmosDatabase));
            }

            if (containerProperties is null)
            {
                throw new ArgumentNullException(nameof(containerProperties));
            }

            if (containerProperties.Id is null)
            {
                throw new ArgumentNullException(nameof(containerProperties.Id));
            }

            Func<Container> containerFactory;

            if (!allowCreate)
            {
                containerFactory = () => cosmosDatabase.Value.GetContainer(containerProperties.Id);
            }
            else 
            {
                if (containerProperties.PartitionKeyPath is null)
                {
                    throw new ArgumentNullException(nameof(containerProperties.PartitionKeyPath));
                }

                containerFactory = () =>
                {
                    if (throughput == null)
                    {
                        return cosmosDatabase.Value.CreateContainerIfNotExistsAsync(containerProperties).Result;
                    }
                    else
                    {
                        return cosmosDatabase.Value.CreateContainerIfNotExistsAsync(containerProperties, throughput).Result;
                    }
                };
            }

            container = new Lazy<Container>(containerFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Container Value => container.Value;        
    }
}