using Community.Azure.Cosmos.Tests.Entities;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

using System.Net;
using System.Threading.Tasks;

using Xunit;

namespace Community.Azure.Cosmos.IntegrationTests
{
    public class Containers
    {
        public class Cnt1 { }
        public class Cnt2 { }
        public class Cnt3 { }
        public class Cnt4 { }

        private static readonly ServiceProvider Sp;
        static Containers()
        {
            ServiceCollection services = new();

            Databases.AddDatabases(services);

            services.AddCosmosContainer<Databases.Db4, Cnt1>("notExisting");
            services.AddCosmosContainer<Databases.Db4, Cnt2>(true, (sp) => new ContainerProperties("new-test-cnt", "/pk"));
            services.AddCosmosContainer<Databases.Db4, Cnt3>(true, (sp) => new ContainerProperties("existing-test-cnt", "/pk"));
            services.AddCosmosContainer<Databases.Db4, Cnt4>(false, (sp) => new ContainerProperties() { Id = "existing-test-cnt" });

            Sp = services.BuildServiceProvider();
        }

        [Fact]
        public async Task CreateAndDelete()
        {
            CosmosContainer<Cnt2> db = Sp.GetRequiredService<CosmosContainer<Cnt2>>();

            TestEntity1 item = TestEntity1.Create();

            var result = await db.Value.CreateItemAsync(item, new PartitionKey(item.Pk));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            CosmosContainer<Cnt2> db2 = Sp.GetRequiredService<CosmosContainer<Cnt2>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);

            var del = await db2.Value.DeleteContainerAsync();
            Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

            item = TestEntity1.Create();
            CosmosException exception = await Assert.ThrowsAsync<CosmosException>(() => db.Value.CreateItemAsync(item, new PartitionKey(item.Pk)));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task GetNotExisting()
        {
            CosmosContainer<Cnt1> db = Sp.GetRequiredService<CosmosContainer<Cnt1>>();

            CosmosException exception = await Assert.ThrowsAsync<CosmosException>(() =>
            {
                TestEntity1 item = TestEntity1.Create();
                return db.Value.CreateItemAsync(item, new PartitionKey(item.Pk));
            });

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);

            CosmosContainer<Cnt1> db2 = Sp.GetRequiredService<CosmosContainer<Cnt1>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);
        }

        [Fact]
        public async Task GetExisting()
        {
            CosmosContainer<Cnt4> db = Sp.GetRequiredService<CosmosContainer<Cnt4>>();

            TestEntity1 item = TestEntity1.Create();

            var result = await db.Value.CreateItemAsync(item, new PartitionKey(item.Pk));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            CosmosContainer<Cnt4> db2 = Sp.GetRequiredService<CosmosContainer<Cnt4>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);
        }

        [Fact]
        public async Task GetOrCreateExisting()
        {
            CosmosContainer<Cnt3> db = Sp.GetRequiredService<CosmosContainer<Cnt3>>();

            TestEntity1 item = TestEntity1.Create();

            var result = await db.Value.CreateItemAsync(item, new PartitionKey(item.Pk));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            CosmosContainer<Cnt3> db2 = Sp.GetRequiredService<CosmosContainer<Cnt3>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);
        }
    }
}
