using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;

namespace CosmosDb
{
    public static class DataSeed2
    {
        const int ArticlesCount = 10000;
        const int AssetsPerArticle = 100;

        private static void PrintCreateResult<T>(Task<ItemResponse<T>> response)where T:Entity2
        {
            ItemResponse<T> result = response.Result;
            Console.WriteLine($"{result.StatusCode} {result.Resource.PartitionKey} {result.RequestCharge}");
        }

        public static async Task Run(Database database)
        {
            DateTime now = DateTime.Now;

            // Delete the existing containers to prevent create item conflicts
            using (await database.GetContainer("all").DeleteContainerStreamAsync()){ }            
          
            Container all = await database.CreateContainerIfNotExistsAsync(new ContainerProperties("all", partitionKeyPath: "/pk"));
            
            for (int i = 0; i < ArticlesCount; i++)
            {
                List<Task> tasks = new List<Task>(AssetsPerArticle * 2 + 1);

                Article2 article = new Article2
                {
                    Id = $"an{i}",
                    ValidFrom = now.AddDays(i - ArticlesCount / 2),
                    ModelNumber = $"mn{i % 100}",                    
                };

                tasks.Add(all.CreateItemAsync(article, article.GetCosmosPartitionKey()));
                
                for (int j = 0; j < AssetsPerArticle; j++)
                {                    
                    Asset2 asset = new Asset2()
                    {
                        FileName = $"filename-{i}-{j}",
                        Id = Guid.NewGuid().ToString("N"),
                        ValidTo = now.AddDays(j),                        
                        ArticleId = article.Id
                    };

                    tasks.Add(all.CreateItemAsync(asset, asset.GetCosmosPartitionKey()));

                    var rel = new AssetsOfArticle2()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ArticleId = article.Id,
                        AssetId = asset.Id
                    };

                    tasks.Add(all.CreateItemAsync(rel, rel.GetCosmosPartitionKey()));
                }

                await Task.WhenAll(tasks);
                Console.WriteLine(article.Id);
            }
        }
    }
}
