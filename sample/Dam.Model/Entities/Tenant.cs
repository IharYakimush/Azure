using Dam.Model.Entities;

using System.Text.Json.Serialization;

namespace Dam.Model
{
    public class TenantIndexed : Entity
    {
        public const string TypeName = "tenant";

        public new class PropertyName : Entity.PropertyName
        {
            public const string Name = "name";
        }

        public new class PropertyPath : Entity.PropertyName
        {
            public const string Name = "/" + PropertyName.Name;
        }

        [JsonConstructor]
        public TenantIndexed(string id, string name, DateTime createdOn, string type, string? pk, ICollection<string>? tags) : base(id, createdOn, type, pk, tags)
        {            
            GuidHelper.ValidateId(id, 16);
            this.ValidateType(TypeName);
            ValidateName(name);
            this.Name = name;
        }

        [JsonPropertyName(PropertyName.Name)]
        public string Name { get; }

        public static void ValidateName(string? name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(name), name.Length, ">50");
            }
        }
    }

    public class Tenant : TenantIndexed
    {
        [JsonConstructor]
        public Tenant(string id, string name, DateTime createdOn, string type, string? pk, ICollection<string>? tags) : base(id, name, createdOn, type, pk, tags)
        {
        }

        public Tenant(string name) : base(GuidHelper.GenerateId(16), name, DateTime.UtcNow, TypeName, null, null) { }        
    }
}