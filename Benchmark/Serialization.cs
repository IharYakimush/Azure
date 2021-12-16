using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Community.Azure.Cosmos.Tests.Entities;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace Benchmark
{
    public class Serialization
    {        
        private readonly CosmosSerializer cosmosSerializer = new CosmosClientBuilder("https://qwe.qwe", "cXdl")
            .WithSerializerOptions(new CosmosSerializationOptions() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
            .Build().ClientOptions.Serializer;

        private readonly TestEntity1 testEntity = TestEntity1.Create();

        private readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private byte[] data;

        public Serialization()
        {
            MemoryStream ms = new MemoryStream();
            JsonSerializer.Serialize(ms, new[] { testEntity, testEntity, testEntity, testEntity, testEntity, testEntity, testEntity, testEntity, testEntity }, defaultOptions);
            ms.Seek(0, SeekOrigin.Begin);
            data = ms.ToArray();
        }

        [Benchmark]
        public void SerReflection()
        {
            using MemoryStream ms = new MemoryStream();

            JsonSerializer.Serialize(ms, testEntity, defaultOptions);
        }

        [Benchmark]
        public void SerOptions()
        {
            using MemoryStream ms = new MemoryStream();

            JsonSerializer.Serialize(ms, testEntity, JsonTestContext.Default.En1);
        }

        [Benchmark]
        public void SerContext()
        {
            using MemoryStream ms = new MemoryStream();

            JsonSerializer.Serialize(ms, testEntity, typeof(TestEntity1), JsonTestContext.Default);
        }

        [Benchmark]
        public void SerCosmosDefault()
        {
            using MemoryStream ms = new MemoryStream();

            cosmosSerializer.ToStream(testEntity);
        }

        [Benchmark]
        public TestEntity1[] DesReflection()
        {
            return JsonSerializer.Deserialize<TestEntity1[]>(new MemoryStream(data), defaultOptions);
        }

        [Benchmark]
        public TestEntity1[] DesOptions()
        {
            return JsonSerializer.Deserialize<TestEntity1[]>(new MemoryStream(data), JsonTestContext.Default.En1Array);
        }

        [Benchmark]
        public TestEntity1[] DesContext()
        {
            return (TestEntity1[])JsonSerializer.Deserialize(new MemoryStream(data), typeof(TestEntity1[]), JsonTestContext.Default);
        }

        [Benchmark]
        public TestEntity1[] DesCosmosDefault()
        {
            return cosmosSerializer.FromStream<TestEntity1[]>(new MemoryStream(data));
        }
    }
}
