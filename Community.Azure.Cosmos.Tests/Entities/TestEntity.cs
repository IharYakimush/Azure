using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Community.Azure.Cosmos.Tests.Entities
{
    public abstract class TestEntityBase
    {
        internal static readonly Random Random = new Random();

        public TestEntityBase(string id, string? pk)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Pk = pk ?? id;
        }

        public string Id { get; }
        public string Pk { get; }
    }

    public class TestEntity1 : TestEntityBase
    {

        public static TestEntity1 Create()
        {
            return new TestEntity1(Guid.NewGuid().ToString())
            {
                IntV1 = Random.Next(),
                Name1 = Guid.NewGuid().ToString(),
                Name2 = Guid.NewGuid().ToString(),
                Name3 = Guid.NewGuid().ToString(),
                Tags = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                Dv2 = 234.234M,
                OneToOne = TestEntity3.Create(),
                Childs = new List<TestEntity2>() { TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create(), TestEntity2.Create() }
            };
        }

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public TestEntity1(string name, string id, string pk) : base(id, pk)
        {
            Name = name;
        }

        public TestEntity1(string name) : base(Guid.NewGuid().ToString(), null)
        {
            Name = name;
        }

        public string Name { get; }

        public string? Name1 { get; set; }

        public string? Name2 { get; set; }

        public string? Name3 { get; set; }

        public int IntV1 { get; set; }

        public decimal Dv2 { get; set; }

        public ICollection<string> Tags { get; set; } = new List<string>();

        public ICollection<TestEntity2> Childs { get; set; } = new List<TestEntity2>();

        public TestEntity3? OneToOne { get; set; }

        public bool Bval1 { get; set; }

    }

    public class TestEntity2 : TestEntityBase
    {
        public static TestEntity2 Create()
        {
            return new TestEntity2()
            {
                IntV1 = Random.Next(),
                Name1 = Guid.NewGuid().ToString(),
                Name2 = Guid.NewGuid().ToString(),
                Name3 = Guid.NewGuid().ToString(),
                Tags = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                StatusCode = HttpStatusCode.Continue,
                Bval213 = false,
                Bval23 = true,
                Dv2 = 234.234M,
                Name = Guid.NewGuid().ToString()
            };
        }

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public TestEntity2(string id, string pk) : base(id, pk)
        {
        }

        public TestEntity2() : base(Guid.NewGuid().ToString(), null)
        {
            this.Name = Guid.NewGuid().ToString();
        }

        public string? Name { get; set; }
        public string? Name1 { get; set; }

        public string? Name2 { get; set; }

        public string? Name3 { get; set; }

        public bool Bval213 { get; set; }


        public int IntV1 { get; set; }

        public decimal Dv2 { get; set; }

        HttpStatusCode StatusCode { get; set; }

        public bool Bval23 { get; set; }

        public ICollection<string> Tags { get; set; } = new List<string>();
    }

    public class TestEntity3
    {
        public static TestEntity3 Create()
        {
            return new TestEntity3()
            {
                Name = Guid.NewGuid().ToString(),
                Bval1 = true,
                Dv2 = 3424.2354M,
                IntV1 = TestEntityBase.Random.Next(),
                Name1 = Guid.NewGuid().ToString(),
                Name2 = Guid.NewGuid().ToString(),
                Name3 = Guid.NewGuid().ToString(),
                Tags1 = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                Tags2 = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            };
        }
        public string? Name { get; set; }

        public string? Name1 { get; set; }

        public string? Name2 { get; set; }

        public string? Name3 { get; set; }

        public int IntV1 { get; set; }

        public decimal Dv2 { get; set; }

        public ICollection<string> Tags1 { get; set; } = new List<string>();

        public ICollection<string> Tags2 { get; set; } = new List<string>();


        public bool Bval1 { get; set; }
    }
}
