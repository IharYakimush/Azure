using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dam.Model;
using Community.Azure.Cosmos;
using Dam.Model.WebModels;
using Microsoft.Azure.Cosmos;
using System.Threading;
using System.Collections.Generic;
using AutoMapper;
using System.Text.Json;

using System.Linq;
using Microsoft.Azure.Cosmos.Linq;

namespace Dam.FunctionsHttp
{
    public class TenantsCrud
    {            
        private static readonly CosmosLinqSerializerOptions linqSerializerOptions = new CosmosLinqSerializerOptions() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };
        private readonly CosmosContainer<TenantIndexed> tenantsIndexedContainer;
        private readonly CosmosContainer<Tenant> tenantsContainer;
        private readonly IMapper mapper;

        public TenantsCrud(CosmosContainer<TenantIndexed> tenantsIndexedContainer, CosmosContainer<Tenant> tenantsContainer, IMapper mapper)
        {
            this.tenantsIndexedContainer = tenantsIndexedContainer ?? throw new ArgumentNullException(nameof(tenantsIndexedContainer));
            this.tenantsContainer = tenantsContainer ?? throw new ArgumentNullException(nameof(tenantsContainer));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [FunctionName("CreateTenant")]
        public async Task<IActionResult> CreateTenant(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tenants")] HttpRequest http,
            ILogger log, CancellationToken cancellationToken)
        {            
            TenantCreate req = await JsonSerializer.DeserializeAsync<TenantCreate>(http.Body, ObjectResponseBody<TenantRead>.SerializerOptions, cancellationToken);

            Tenant tenant = new Tenant(req.Name ?? $"t{DateTime.UtcNow.Ticks}");
            if (req.Tags != null)
            {
                foreach (var tag in req.Tags)
                {
                    tenant.Tags.Add(tag);
                }
            }
            try
            {                
                ItemResponse<Tenant> result = await tenantsIndexedContainer.Value.CreateItemAsync(tenant, new PartitionKey(tenant.Type), cancellationToken: cancellationToken);

                log.LogInformation("TenantIdexedCreateOk", new Dictionary<string, object>() { { "StatusCode", result.StatusCode }, { "RequestCharge", result.RequestCharge } });

                result = await tenantsContainer.Value.CreateItemAsync(tenant, new PartitionKey(tenant.Pk), cancellationToken: cancellationToken);

                log.LogInformation("TenantCreateOk", new Dictionary<string, object>() { { "StatusCode", result.StatusCode }, { "RequestCharge", result.RequestCharge } });

                //ObjectResult okObjectResult = new ObjectResult();                
                http.HttpContext.Response.Headers.TryAdd("ETag", result.ETag);

                return new ObjectResponseBody<TenantRead>(mapper.Map<TenantRead>(result.Resource), System.Net.HttpStatusCode.OK);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError(cosmosException, "TenantCreateError", new Dictionary<string, object>() { { "StatusCode", cosmosException.StatusCode } });

                throw;
            }
        }

        [FunctionName("GetTenants")]
        public async Task<IActionResult> GetTenants(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tenants")] HttpRequest http,
            ILogger log, CancellationToken cancellationToken)
        {
            IOrderedQueryable<TenantRead> query = this.tenantsIndexedContainer.Value.GetItemLinqQueryable<TenantRead>(linqSerializerOptions: linqSerializerOptions).OrderByDescending(t => t.CreatedOn);

            FeedIterator<TenantRead> iterator = query.ToFeedIterator();

            FeedResponse<TenantRead> result = await iterator.ReadNextAsync(cancellationToken);
            http.HttpContext.Response.Headers.TryAdd($"X-{nameof(result.Count)}", $"{result.Count}");
            http.HttpContext.Response.Headers.TryAdd($"X-{nameof(result.ContinuationToken)}", result.ContinuationToken);            
            return new ObjectResponseBody<TenantRead[]>(result.ToArray(), System.Net.HttpStatusCode.OK);
        }
    }
}
