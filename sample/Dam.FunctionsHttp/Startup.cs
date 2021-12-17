using Dam.FunctionsHttp;
using Dam.Model;
using Dam.Model.Configuration;
using Dam.Model.CosmosDb;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Dam.FunctionsHttp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {            
            builder.Services.AddAutoMapper(typeof(Entity));

            AddFunctionsConfig(builder);

            builder.Services.AddCosmosDb();
        }

        public static void AddFunctionsConfig(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IConfiguration>(sp =>
            {
                FunctionsHostBuilderContext context = builder.GetContext();

                ConfigurationBuilder cb = new ConfigurationBuilder().AddDamHostConfig(context.ApplicationRootPath, Array.Empty<string>(), context.Configuration).AddDamAppConfig();

                return cb.Build();
            });
        }
    }
}
