using System;
using System.Collections.Generic;

using Dam.Model;

using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Dam.FunctionsHttp
{
    public static class OnTenantCreate
    {
        [FunctionName("OnTenantCreate")]
        public static void Run([CosmosDBTrigger(
            databaseName: "dam",
            collectionName: "tenants",
            ConnectionStringSetting = "CosmosDb",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "sync_tenant_and_inexed")]IReadOnlyList<Document> input,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {                
                log.LogInformation("Documents modified " + input.Count);
                foreach (Document item in input)
                {
                    log.LogInformation(item.Id);
                    string name = item.GetPropertyValue<string>(Tenant.PropertyName.Name);
                    log.LogInformation(name);

                    if (name.StartsWith("exc_"))
                    {
                        throw new InvalidOperationException("test");
                    }
                }
            }
        }
    }
}
