using System.Text.Json.Serialization;

using static Dam.Model.Entity;

namespace Dam.Model.WebModels
{
    public class EntityCreate
    {
        [JsonPropertyName(PropertyName.Tags)]
        public string[]? Tags { get; set; }
    }

    public class EntityRead : EntityCreate
    {
        [JsonPropertyName(PropertyName.Id)]
        public string? Id { get; set; }        

        [JsonPropertyName(PropertyName.CreatedOn)]
        public DateTime CreatedOn { get; set; }
    }
}
