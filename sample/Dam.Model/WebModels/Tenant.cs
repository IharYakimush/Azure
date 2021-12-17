using System.Text.Json.Serialization;

using static Dam.Model.TenantIndexed;

namespace Dam.Model.WebModels
{
    public class TenantCreate : EntityCreate
    {
        [JsonPropertyName(PropertyName.Name)]
        public string? Name { get; set; }
    }

    public class TenantRead : EntityRead
    {
        [JsonPropertyName(PropertyName.Name)]
        public string? Name { get; set; }
    }
}
