using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.UserSecrets;

using Xunit;
using System;

namespace Community.Azure.Cosmos.IntegrationTests
{
    public class CreateDbAndContainer
    {
        private static readonly ServiceProvider Sp;
        static CreateDbAndContainer()
        {            
            ServiceCollection services = new ServiceCollection();

            services.AddCosmosClient(new CosmosClientBuilder(Conf.Value.GetConnectionString("CosmosDb")));

            services.AddCosmosDatabaseCreateIfNotExists<Databases.Db1>("azt1");
            services.AddCosmosDatabase<Databases.Db2>("azt2");
            services.AddCosmosDatabaseCreateIfNotExists<Databases.Db3>("dam");
            services.AddCosmosDatabase<Databases.Db4>("dam");

            //services.AddCosmosContainerCreateIfNotExists<Databases.Dd1, Containers.Cnt1>("test1", "/pk");
            Sp = services.BuildServiceProvider();
        }
        [Fact]
        public void Test1()
        {

        }
    }
}