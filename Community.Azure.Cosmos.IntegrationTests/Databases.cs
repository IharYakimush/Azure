using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Community.Azure.Cosmos.IntegrationTests
{
    public class Databases
    {
        public class Db1 { }
        public class Db2 { }
        public class Db3 { }
        public class Db4 { }

        private static readonly ServiceProvider Sp;
        static Databases()
        {
            ServiceCollection services = new();

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddUserSecrets(typeof(Databases).Assembly);

            services.TryAddSingleton<IConfiguration>(configurationBuilder.Build());

            services.AddCosmosClient(sp=> new CosmosClientBuilder(sp.GetRequiredService<IConfiguration>().GetConnectionString("CosmosDb")));

            services.AddCosmosDatabase<Db1>(true, "azt1");
            services.AddCosmosDatabase<Db2>(false, "azt2");
            services.AddCosmosDatabase<Db3>(true, "dam");
            services.AddCosmosDatabase<Db4>(false, "dam");

            //services.AddCosmosContainerCreateIfNotExists<Databases.Dd1, Containers.Cnt1>("test1", "/pk");
            Sp = services.BuildServiceProvider();
        }

        [Fact]
        public async Task CreateAndDelete()
        {
            CosmosDatabase<Db1> db = Sp.GetRequiredService<CosmosDatabase<Db1>>();

            DatabaseResponse result = await db.Value.ReadAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            CosmosDatabase<Db1> db2 = Sp.GetRequiredService<CosmosDatabase<Db1>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);

            var del = await db2.Value.DeleteAsync();
            Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

            CosmosException exception = await Assert.ThrowsAsync<CosmosException>(() => db.Value.ReadAsync());

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task GetNotExisting()
        {
            CosmosDatabase<Db2> db = Sp.GetRequiredService<CosmosDatabase<Db2>>();

            CosmosException exception = await Assert.ThrowsAsync<CosmosException>(() => db.Value.ReadAsync());
            
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);

            CosmosDatabase<Db2> db2 = Sp.GetRequiredService<CosmosDatabase<Db2>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);
        }

        [Fact]
        public async Task GetExisting()
        {
            CosmosDatabase<Db4> db = Sp.GetRequiredService<CosmosDatabase<Db4>>();

            DatabaseResponse result = await db.Value.ReadAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            CosmosDatabase<Db4> db2 = Sp.GetRequiredService<CosmosDatabase<Db4>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);
        }

        [Fact]
        public async Task GetOrCreateExisting()
        {
            CosmosDatabase<Db3> db = Sp.GetRequiredService<CosmosDatabase<Db3>>();

            DatabaseResponse result = await db.Value.ReadAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            CosmosDatabase<Db3> db2 = Sp.GetRequiredService<CosmosDatabase<Db3>>();

            Assert.Same(db, db2);
            Assert.Same(db.Value, db2.Value);
        }
    }
}
