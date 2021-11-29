using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace CosmosDb
{
    public class Collection
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CollectionWithItems : Collection
    {
        public List<string> Items { get; set; }
    }

    public class CollectionItem
    {
        public string Id { get; set; }

        public string CollectionId { get; set; }

        public string ArticleNumber { get; set; }
    }

    public class Article
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ArticleNumber { get; set; }
        public string ModelNumber { get; set; }
        public DateTime ValidFrom { get; set; }
    }

    public class ArticleWithAssets : Article
    {
        public List<Asset> Assets { get; set; }
    }

    public class Asset
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DateTime ValidTo { get; set; }

        public string FileName { get; set; }

        public string ArticleNumber { get; set; }
    }

    public class AssetWithArticle : Asset
    {
        public Article Article { get; set; }
    }

    public class AssetsOfArticle
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string AssetId { get; set; }

        public string ArticleNumber { get; set; }
    }
}
