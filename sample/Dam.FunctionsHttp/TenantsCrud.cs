using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dam.Model;
using Community.Azure.Cosmos;
using Dam.Model.WebModels;
using Microsoft.Azure.Cosmos;
using System.Threading;
using System.Collections.Generic;

namespace Dam.FunctionsHttp
{
    public class TenantsCrud
    {
        private readonly CosmosContainer<TenantIndexed> tenantsContainer;

        public TenantsCrud(CosmosContainer<TenantIndexed> tenantsContainer)
        {
            this.tenantsContainer = tenantsContainer ?? throw new ArgumentNullException(nameof(tenantsContainer));
        }

        [FunctionName("CreateTenant")]
        public async Task<IActionResult> CreateTenant(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tenants")] TenantCreate req,
            ILogger log, CancellationToken cancellationToken)
        {
            Tenant tenant = new Tenant() { Name = req.Name ?? $"t{DateTime.UtcNow.Ticks}" };

            try
            {
                ItemResponse<Tenant> result = await tenantsContainer.Value.CreateItemAsync(tenant, new PartitionKey(tenant.Type), cancellationToken: cancellationToken);

                log.LogInformation("TenantCreateOk", new Dictionary<string, object>() { { "StatusCode", result.StatusCode }, { "RequestCharge", result.RequestCharge } });

                OkObjectResult okObjectResult = new OkObjectResult(new TenantRead() { Id = result.Resource.Id });

                return okObjectResult;
            }
            catch (CosmosException cosmosException)
            {
                log.LogError(cosmosException, "TenantCreateError", new Dictionary<string, object>() { { "StatusCode", cosmosException.StatusCode } });

                throw;
            }
        }        
    }
}
