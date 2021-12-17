using Azure.Core;
using Azure.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dam.Model.Configuration
{
    public static class ConfigServices
    {
        public static ConfigurationBuilder AddDamHostConfig(this ConfigurationBuilder builder, string appRootPath, string[] args, IConfiguration? configuration)
        {
            var dir = Directory.GetCurrentDirectory();

            builder
                .AddJsonFile(Path.Combine(appRootPath, "globalconfig.json"), false)
                .AddJsonFile(Path.Combine(appRootPath, "appconfig.json"), true)
                .AddEnvironmentVariables("DAM_");

            if (configuration != null)
            {
                builder.AddConfiguration(configuration);
            }

            builder.AddCommandLine(args);
#if DEBUG
            Dictionary<string, string> debugConf = new Dictionary<string, string>()
            {
                { "ConnectionStrings:AppConfig", "https://appconfig25245.azconfig.io"},
            };

            builder.AddInMemoryCollection(debugConf);
#endif
            return builder;
        }

        public static ConfigurationBuilder AddDamAppConfig(this ConfigurationBuilder builder)
        {
            var settings = builder.Build();
            var connection = settings.GetConnectionString("AppConfig");
            DefaultAzureCredential credential = DefaultTokenCredentialWithConfig(settings);

            builder.AddAzureAppConfiguration(options =>
                    options.ConfigureKeyVault(kv => kv.SetCredential(credential).SetSecretRefreshInterval(TimeSpan.FromDays(1)))
                    .Connect(new Uri(connection), credential));

            return builder;
        }        

        public static IServiceCollection AddAzureIdentity(this IServiceCollection services)
        {
            services.TryAddSingleton<TokenCredential>(sp =>
            {
                IConfiguration conf = sp.GetRequiredService<IConfiguration>();
                return DefaultTokenCredentialWithConfig(conf);
            });

            return services;
        }

        private static DefaultAzureCredential DefaultTokenCredentialWithConfig(IConfiguration configuration)
        {
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions();
            configuration.Bind(nameof(DefaultAzureCredentialOptions), options);

            return new DefaultAzureCredential(options);
        }
    }
}
