using System;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace CosmosDb
{
    class Program
    {
        private static readonly string databaseId = "samples";
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddJsonFile("appSettings.json", true)
                    .AddUserSecrets<Program>()
                    .Build();

            string endpoint = configuration["EndPointUrl"];
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentNullException("Please specify a valid endpoint in the appSettings.json");
            }

            string authKey = configuration["AuthorizationKey"];
            if (string.IsNullOrEmpty(authKey) || string.Equals(authKey, "Super secret key"))
            {
                throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json");
            }

            //Read the Cosmos endpointUrl and authorisationKeys from configuration
            //These values are available from the Azure Management Portal on the Cosmos Account Blade under "Keys"
            //NB > Keep these values in a safe & secure location. Together they provide Administrative access to your Cosmos account
            using (CosmosClient client = new CosmosClient(endpoint, authKey,new CosmosClientOptions() { ConnectionMode = ConnectionMode.Direct,AllowBulkExecution = true}))
            {
                //new CosmosClient("qwe", new DefaultAzureCredential());
                DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
                //await DataSeed2.Run(database);
                //await DataSeed.Run(database);
                await GetById2.Run(database);
                //await GetByArticleNumber.Run(database);
            }
        }
    }
}
