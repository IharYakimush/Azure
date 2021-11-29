using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDb
{
    public static class GetById2
    {
        public static async Task Run(Database database)
        {
            await AssetByIdAssets(database);
            Console.WriteLine();
            await AssetByArticleNumber(database);
            
        }

        public static async Task AssetByIdAssets(Database database)
        {
            Container assets = database.GetContainer("all");

            var requestOptions = new QueryRequestOptions() { PartitionKey = new PartitionKey("asset_f170706b06de4e9f9d5f8a6f0700ad78") };

            IQueryable<Asset2> query = assets.GetItemLinqQueryable<Asset2>().Where(a => a.Id == "f170706b06de4e9f9d5f8a6f0700ad78");
            Console.WriteLine(query);
            using FeedIterator<Asset2> feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleId} {result.Resource.First().Id}");
        }

        public static async Task AssetByArticleNumber(Database database)
        {
            Container assets = database.GetContainer("all");

            IQueryable<Asset2> query = assets.GetItemLinqQueryable<Asset2>().Where(a => a.ArticleId == "an0" && a.Type == "asset");
            Console.WriteLine(query);
            using FeedIterator<Asset2> feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleId} {result.Resource.First().Id}");
        }

        
    }
}
