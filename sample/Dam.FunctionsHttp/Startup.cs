using Dam.FunctionsHttp;
using Dam.Model.Configuration;
using Dam.Model.CosmosDb;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Dam.FunctionsHttp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddConfig();

            builder.Services.AddCosmosDb();
        }
    }
}
