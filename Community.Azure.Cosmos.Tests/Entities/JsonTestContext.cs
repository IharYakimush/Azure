using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Community.Azure.Cosmos.Tests.Entities
{
    [JsonSourceGenerationOptions(
#if DEBUG
        WriteIndented = true,
#endif        
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
        )]
    [JsonSerializable(typeof(TestEntity1), TypeInfoPropertyName ="En1")]
    [JsonSerializable(typeof(TestEntity2))]
    [JsonSerializable(typeof(TestEntity3))]
    [JsonSerializable(typeof(TestEntity1[]), TypeInfoPropertyName = "En1Array")]
    [JsonSerializable(typeof(TestEntity2[]))]
    [JsonSerializable(typeof(TestEntity3[]))]
    public partial class JsonTestContext : JsonSerializerContext
    {
    }
}
