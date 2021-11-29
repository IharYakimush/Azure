using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDb
{
    public static class GetById
    {
        public static async Task Run(Database database)
        {
            await AssetByIdAssets(database);
            Console.WriteLine();
            await AssetByIdArticleWithAssets(database);
            Console.WriteLine();
            await AssetByIdArticleWithAssetsPk(database);
            Console.WriteLine();
            await AssetByIdArticleWithAssetsFilterByPk(database);
        }

        public static async Task AssetByIdAssets(Database database)
        {
            Container assets = database.GetContainer("assets");

            IQueryable<Asset> query = assets.GetItemLinqQueryable<Asset>().Where(a => a.Id == "a6e824c3683b42a58831af853928621d");
            Console.WriteLine(query);
            using FeedIterator<Asset> feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleNumber} {result.Resource.First().Id}");
        }

        public static async Task AssetByIdArticleWithAssets(Database database)
        {
            Container arwa = database.GetContainer("articles-with-assets");

            var query = arwa.GetItemLinqQueryable<ArticleWithAssets>().SelectMany(ar => ar.Assets).Where(a => a.Id == "a6e824c3683b42a58831af853928621d");
            Console.WriteLine(query);
            using var feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleNumber} {result.Resource.First().Id}");
        }

        public static async Task AssetByIdArticleWithAssetsPk(Database database)
        {
            Container arwa = database.GetContainer("articles-with-assets");

            var requestOptions = new QueryRequestOptions() { PartitionKey = new PartitionKey("an0") };

            var query = arwa.GetItemLinqQueryable<ArticleWithAssets>(requestOptions: requestOptions).SelectMany(ar => ar.Assets).Where(a => a.Id == "a6e824c3683b42a58831af853928621d");
            Console.WriteLine(query);
            using var feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleNumber} {result.Resource.First().Id}");
        }

        public static async Task AssetByIdArticleWithAssetsFilterByPk(Database database)
        {
            Container arwa = database.GetContainer("articles-with-assets");

            var query = arwa.GetItemLinqQueryable<ArticleWithAssets>()
                .Where(ar => ar.ArticleNumber == "an0")
                .SelectMany(ar => ar.Assets)
                .Where(a => a.Id == "a6e824c3683b42a58831af853928621d");

            Console.WriteLine(query);
            using var feed = query.ToFeedIterator();
            var result = await feed.ReadNextAsync();

            Console.WriteLine($"{result.RequestCharge} {result.Resource.Count()} {result.Resource.First().ArticleNumber} {result.Resource.First().Id}");
        }
    }
}
