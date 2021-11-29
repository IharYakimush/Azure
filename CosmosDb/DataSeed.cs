using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;

namespace CosmosDb
{
    public static class DataSeed
    {
        const int ArticlesCount = 1000;
        const int AssetsPerArticle = 100;

        public static async Task Run(Database database)
        {
            DateTime now = DateTime.Now;

            // Delete the existing containers to prevent create item conflicts
            using (await database.GetContainer("articles").DeleteContainerStreamAsync()){ }
            using (await database.GetContainer("articles-with-assets").DeleteContainerStreamAsync()) { }
            using (await database.GetContainer("assets").DeleteContainerStreamAsync()) { }
            using (await database.GetContainer("assets-of-article").DeleteContainerStreamAsync()) { }
            using (await database.GetContainer("assets-with-article").DeleteContainerStreamAsync()) { }

            // We create a partitioned collection here which needs a partition key. Partitioned collections
            // can be created with very high values of provisioned throughput (up to Throughput = 250,000)
            // and used to store up to 250 GB of data. You can also skip specifying a partition key to create
            // single partition collections that store up to 10 GB of data.
            // For this demo, we create a collection to store SalesOrders. We set the partition key to the account
            // number so that we can retrieve all sales orders for an account efficiently from a single partition,
            // and perform transactions across multiple sales order for a single account number.             
            Container articles = await database.CreateContainerIfNotExistsAsync(new ContainerProperties("articles", partitionKeyPath: "/ArticleNumber"));
            Container articlesWithAssets = await database.CreateContainerIfNotExistsAsync(new ContainerProperties("articles-with-assets", partitionKeyPath: "/ArticleNumber"));
            Container assets = await database.CreateContainerIfNotExistsAsync(new ContainerProperties("assets", partitionKeyPath: "/id"));
            Container assetsOfArticle = await database.CreateContainerIfNotExistsAsync(new ContainerProperties("assets-of-article", partitionKeyPath: "/ArticleNumber"));
            Container assetsWithArticle = await database.CreateContainerIfNotExistsAsync(new ContainerProperties("assets-with-article", partitionKeyPath: "/ArticleNumber"));

            for (int i = 0; i < ArticlesCount; i++)
            {
                ArticleWithAssets articleWithAssets = new ArticleWithAssets
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ArticleNumber = $"an{i}",
                    ValidFrom = now.AddDays(i - ArticlesCount / 2),
                    ModelNumber = $"mn{i % 100}",
                    Assets = new List<Asset>()
                };

                Article article = articleWithAssets;

                ItemResponse<Article> create1 = await articles.CreateItemAsync(article, new PartitionKey(article.ArticleNumber));
                Console.WriteLine($"{create1.StatusCode} {create1.RequestCharge} {create1.Resource.ArticleNumber} articles");
                
                for (int j = 0; j < AssetsPerArticle; j++)
                {                    
                    AssetWithArticle assetWithArticle = new AssetWithArticle()
                    {
                        FileName = $"f-{i}-{j}",
                        Id = Guid.NewGuid().ToString("N"),
                        ValidTo = now.AddDays(j),                        
                        ArticleNumber = article.ArticleNumber
                    };

                    Asset asset = assetWithArticle;

                    ItemResponse<Asset> create3 = await assets.CreateItemAsync(asset, new PartitionKey(asset.Id));
                    Console.WriteLine($"{create3.StatusCode} {create3.RequestCharge} {create3.Resource.ArticleNumber} assets");

                    ItemResponse<AssetsOfArticle> create5 = await assetsOfArticle.CreateItemAsync(new AssetsOfArticle()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ArticleNumber = asset.ArticleNumber,
                        AssetId = asset.Id
                    }, new PartitionKey(asset.ArticleNumber));
                    Console.WriteLine($"{create5.StatusCode} {create5.RequestCharge} {create5.Resource.ArticleNumber} assets-of-article");

                    assetWithArticle.Article = article;

                    ItemResponse<AssetWithArticle> create4 = await assetsWithArticle.CreateItemAsync(assetWithArticle, new PartitionKey(assetWithArticle.ArticleNumber));
                    Console.WriteLine($"{create4.StatusCode} {create4.RequestCharge} {create4.Resource.ArticleNumber} assets-with-article");

                    assetWithArticle.Article = null;
                    articleWithAssets.Assets.Add(asset);
                }

                ItemResponse<ArticleWithAssets> create2 = await articlesWithAssets.CreateItemAsync(articleWithAssets, new PartitionKey(articleWithAssets.ArticleNumber));
                Console.WriteLine($"{create2.StatusCode} {create2.RequestCharge} {create2.Resource.ArticleNumber} articles-with-assets");
            }
        }
    }
}
