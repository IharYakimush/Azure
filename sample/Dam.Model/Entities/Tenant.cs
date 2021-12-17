using Dam.Model.Entities;

using System.Text.Json.Serialization;

namespace Dam.Model
{
    public class TenantIndexed : Entity
    {
        public const string TypeName = "tenant";

        public class PropertyName : Entity.PropertyName
        {
            public const string Name = "name";
        }

        public class PropertyPath : Entity.PropertyName
        {
            public const string Name = "/" + PropertyName.Name;
        }

        [JsonConstructor]
        public TenantIndexed(string id, DateTime createdOn, string type, string? pk, ICollection<string>? tags) : base(id, createdOn, type, pk, tags)
        {
            GuidHelper.ValidateId(id, 16);
            this.ValidateType(TypeName);
        }

        public virtual string? Name { get; set; }
    }

    public class Tenant : TenantIndexed
    {
        [JsonConstructor]
        public Tenant(string id, string name, DateTime createdOn, string type, string? pk, ICollection<string>? tags) : base(id, createdOn, type, pk, tags)
        {
        }

        public Tenant() : base(GuidHelper.GenerateId(16), DateTime.UtcNow, TypeName, null, null) { }        
    }
}