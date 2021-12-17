using Dam.Model.Entities.Collections;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Dam.Model
{
    public abstract class Entity
    {
        public class PropertyName
        {
            public const string Id = "id";
            public const string Type = "type";
            public const string Active = "active";
            public const string CreatedOn = "createdOn";
            public const string Tags = "tags";
            public const string Pk = "pk";
        }

        public class PropertyPath
        {
            public const string Id = "/" + PropertyName.Id;
            public const string Type = "/" + PropertyName.Type;
            public const string Active = "/" + PropertyName.Active;
            public const string CreatedOn = "/" + PropertyName.CreatedOn;
            public const string Tags = "/" + PropertyName.Tags;
            public const string Pk = "/" + PropertyName.Pk;
        }

        private readonly StringUniqueCollection tags;
        protected Entity(string id, DateTime createdOn, string type, string? pk, ICollection<string>? tags)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.CreatedOn = createdOn;
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Pk = pk ?? id;
            this.tags = new StringUniqueCollection(8, 64, tags);
        }

        [JsonPropertyName(PropertyName.Id)]
        public string Id { get;  }

        [JsonPropertyName(PropertyName.Type)]
        public string Type { get; }

        [JsonPropertyName(PropertyName.Active)]
        public bool Active { get; set; } = true;

        [JsonPropertyName(PropertyName.CreatedOn)]
        public DateTime CreatedOn { get; } = DateTime.UtcNow;

        [JsonPropertyName(PropertyName.Tags)]
        public ICollection<string> Tags => this.tags;

        [JsonPropertyName(PropertyName.Pk)]
        public string Pk { get; }       

        public override string ToString()
        {
            return $"({this.Type}|{this.Id})";
        }
        
        public void ValidateType(string type)
        {
            if (this.Type != type)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
        }                      
    }
}
