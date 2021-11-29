using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Cosmos;

using Newtonsoft.Json;

namespace CosmosDb
{    
    public static class CosmosExtensions
    {
        public static PartitionKey GetCosmosPartitionKey(this Entity2 entity)
        {
            return new PartitionKey(entity.PartitionKey);
        }
    }

    public abstract class Entity2
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type => this.TypeInternal;

        [JsonIgnore]
        protected abstract string TypeInternal { get; }

        [JsonProperty(PropertyName = "pk")]
        public virtual string PartitionKey => $"{this.Type}_{this.Id}";

        [JsonIgnore]
        protected virtual string PartitionKeyInternal => $"{this.Type}_{this.Id}";        
    }
    public class Article2 : Entity2
    {
        
        public string ModelNumber { get; set; }
        public DateTime ValidFrom { get; set; }
        protected override string TypeInternal => "article";
    }

    public class Asset2 : Entity2
    {
        public DateTime ValidTo { get; set; }

        public string FileName { get; set; }

        public string ArticleId { get; set; }

        protected override string TypeInternal => "asset";
    }

    public class AssetsOfArticle2 : Entity2
    {
        public string AssetId { get; set; }
        
        public string ArticleId { get; set; }

        protected override string TypeInternal => "assets-of-article";

        protected override string PartitionKeyInternal => this.ArticleId;
    }
}
