using System;

using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]

namespace FunctionApp
{

    public class ClassNamePlaceholder : KeyVaultSecretManager
    {
        public override bool Load(SecretProperties secret)
        {            
            return base.Load(secret);
        }

        public override string GetKey(KeyVaultSecret secret)
        {
            return base.GetKey(secret);
        }
    }
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var secretClient = new SecretClient(
                    new Uri($"https://testvault54646.vault.azure.net/"),
                    new DefaultAzureCredential());

            ConfigurationBuilder config = new ConfigurationBuilder();

            config.AddAzureKeyVault(secretClient, new AzureKeyVaultConfigurationOptions { Manager = new KeyVaultSecretManager(), ReloadInterval = TimeSpan.FromDays(1) });

            builder.Services.AddSingleton<IConfiguration>(config.Build());
        }
    }
}