using AutoMapper;

using System.Text.Json.Serialization;

using static Dam.Model.TenantIndexed;

namespace Dam.Model.WebModels
{
    public class TenantReadAutomap : Profile
    {
        public TenantReadAutomap()
        {
            CreateMap<Tenant, TenantRead>();
            CreateMap<TenantIndexed, TenantRead>();
        }
    }

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
