using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDb
{
    public static class GetByArticleNumber
    {
        public static async Task Run(Database database)
        {
            await AssetByArticleNumberAssets(database);
            Console.WriteLine();
            await AssetByArticleNumberArticleWithAssets(database);
        }

        public static async Task AssetByArticleNumberAssets(Database database)
        {
            Container assets = database.GetContainer("assets");

            IQueryable<Asset> query = assets.GetItemLinqQueryable<Asset>().Where(a => a.ArticleNumber == "an0");
            Console.WriteLine(query);
            using FeedIterator<Asset> feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleNumber} {result.Resource.First().Id}");
        }

        public static async Task AssetByArticleNumberArticleWithAssets(Database database)
        {
            Container arwa = database.GetContainer("articles-with-assets");

            var query = arwa.GetItemLinqQueryable<ArticleWithAssets>().Where(ar => ar.ArticleNumber == "an0").SelectMany(ar => ar.Assets);
            Console.WriteLine(query);
            using var feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleNumber} {result.Resource.First().Id}");
        }       
    }
}
