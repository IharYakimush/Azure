using Dam.FunctionsHttp;
using Dam.Model;
using Dam.Model.Configuration;
using Dam.Model.CosmosDb;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Dam.FunctionsHttp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {            
            builder.Services.AddAutoMapper(typeof(Entity));

            builder.Services.AddConfig();

            builder.Services.AddCosmosDb();
        }
    }
}
