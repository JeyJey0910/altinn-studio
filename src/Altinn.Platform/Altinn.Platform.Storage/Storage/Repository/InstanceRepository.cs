using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Configuration;
using Altinn.Platform.Storage.Helpers;
using Altinn.Platform.Storage.Interface.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Altinn.Platform.Storage.Repository
{
    /// <summary>
    /// Repository operations for application instances.
    /// </summary>
    public class InstanceRepository : IInstanceRepository
    {        
        private readonly Uri collectionUri;
        private readonly string databaseId;
        private readonly string collectionId = "instances";
        private readonly string partitionKey = "/instanceOwner/partyId";
        private static DocumentClient _client;
        private readonly AzureCosmosSettings _cosmosettings;
        private readonly ILogger<InstanceRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceRepository"/> class
        /// </summary>
        /// <param name="cosmosettings">the configuration settings for cosmos database</param>
        /// <param name="logger">the logger</param>
        public InstanceRepository(IOptions<AzureCosmosSettings> cosmosettings, ILogger<InstanceRepository> logger)
        {
            this.logger = logger;

            var database = new CosmosDatabaseHandler(cosmosettings.Value);

            _client = database.CreateDatabaseAndCollection(collectionId);
            collectionUri = database.CollectionUri;
            Uri databaseUri = database.DatabaseUri;
            databaseId = database.DatabaseName;

            DocumentCollection documentCollection = database.CreateDocumentCollection(collectionId, partitionKey);

            _client.CreateDocumentCollectionIfNotExistsAsync(
                databaseUri,
                documentCollection).GetAwaiter().GetResult();

            _client.OpenAsync();                    
        }

        /// <inheritdoc/>
        public async Task<Instance> Create(Instance instance)
        {
            PreProcess(instance);

            ResourceResponse<Document> createDocumentResponse = await _client.CreateDocumentAsync(collectionUri, instance);
            Document document = createDocumentResponse.Resource;
            Instance instanceStored = JsonConvert.DeserializeObject<Instance>(document.ToString());

            PostProcess(instanceStored);

            return instanceStored;
        }

        /// <inheritdoc/>
        public async Task<bool> Delete(Instance item)
        {
            PreProcess(item);

            Uri uri = UriFactory.CreateDocumentUri(databaseId, collectionId, item.Id);

            _ = await _client
                .DeleteDocumentAsync(
                    uri.ToString(),
                    new RequestOptions { PartitionKey = new PartitionKey(item.InstanceOwner.PartyId) });

            return true;
        }

        /// <inheritdoc/>
        public async Task<InstanceQueryResponse> GetInstancesOfApplication(
            Dictionary<string, StringValues> queryParams,
            string continuationToken,
            int size)
        {
            InstanceQueryResponse queryResponse = new InstanceQueryResponse();

            FeedOptions feedOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = true,
                MaxItemCount = size,
            };

            if (continuationToken != null)
            {
                feedOptions.RequestContinuation = continuationToken;
            }

            IQueryable<Instance> queryBuilder = _client.CreateDocumentQuery<Instance>(collectionUri, feedOptions);

            try
            {
                queryBuilder = BuildQueryFromParameters(queryParams, queryBuilder);
            }
            catch (Exception e)
            {
                queryResponse.Exception = e.Message;
                return queryResponse;
            }

            try
            {
                IDocumentQuery<Instance> documentQuery = queryBuilder.AsDocumentQuery();

                FeedResponse<Instance> feedResponse = await documentQuery.ExecuteNextAsync<Instance>();
                if (!feedResponse.Any())
                {
                    queryResponse.Count = 0;
                    queryResponse.TotalHits = 0;

                    return queryResponse;
                }

                string nextContinuationToken = feedResponse.ResponseContinuation;

                logger.LogInformation($"continuation token: {nextContinuationToken}");

                // this migth be expensive
                feedOptions.RequestContinuation = null;
                int totalHits = queryBuilder.Count();
                queryResponse.TotalHits = totalHits;

                List<Instance> instances = feedResponse.ToList<Instance>();

                PostProcess(instances);

                queryResponse.Instances = instances;
                queryResponse.ContinuationToken = nextContinuationToken;
                queryResponse.Count = instances.Count;
            }
            catch (Exception e)
            {
                logger.LogError("error: {e}");
                queryResponse.Exception = e.Message;
            }

            return queryResponse;
        }

        private IQueryable<Instance> BuildQueryFromParameters(Dictionary<string, StringValues> queryParams, IQueryable<Instance> queryBuilder)
        {
            foreach (KeyValuePair<string, StringValues> param in queryParams)
            {
                string queryParameter = param.Key;
                StringValues queryValues = param.Value;

                foreach (string queryValue in queryValues)
                {
                    switch (queryParameter)
                    {
                        case "size":
                            // handled outside this, it is a valid parameter.
                            break;

                        case "continuationToken":
                            // handled outside this method, it is a valid parameter.
                            break;

                        case "appId":
                            queryBuilder = queryBuilder.Where(i => i.AppId == queryValue);
                            break;

                        case "org":
                            queryBuilder = queryBuilder.Where(i => i.Org == queryValue);
                            break;

                        case "instanceOwner.partyId":
                            queryBuilder = queryBuilder.Where(i => i.InstanceOwner.PartyId == queryValue);
                            break;

                        case "lastChanged":
                            queryBuilder = QueryBuilderForLastChangedDateTime(queryBuilder, queryValue);
                            break;

                        case "dueBefore":
                            queryBuilder = QueryBuilderForDueBefore(queryBuilder, queryValue);
                            break;

                        case "visibleAfter":
                            queryBuilder = QueryBuilderForVisibleAfter(queryBuilder, queryValue);
                            break;

                        case "created":
                            queryBuilder = QueryBuilderForCreated(queryBuilder, queryValue);
                            break;

                        case "process.currentTask":
                            string currentTaskId = queryValue;
                            queryBuilder = queryBuilder.Where(i => i.Process.CurrentTask.ElementId == currentTaskId);
                            break;

                        case "process.isComplete":
                            bool isComplete = bool.Parse(queryValue);
                            if (isComplete)
                            {
                                queryBuilder = queryBuilder.Where(i => i.Process.Ended != null);
                            }
                            else
                            {
                                queryBuilder = queryBuilder.Where(i => i.Process.Ended == null);
                            }

                            break;

                        case "process.ended":
                            queryBuilder = QueryBuilderForEnded(queryBuilder, queryValue);
                            break;

                        case "appOwner.labels":
                            foreach (string label in queryValue.Split(","))
                            {
                                queryBuilder = queryBuilder.Where(i => i.AppOwner.Labels.Contains(label));
                            }

                            break;

                        default:
                            throw new ArgumentException($"Unknown query parameter: {queryParameter}");
                    }
                }
            }

            return queryBuilder;
        }

        // Limitations in queryBuilder.Where interface forces me to duplicate the datetime methods
        private IQueryable<Instance> QueryBuilderForDueBefore(IQueryable<Instance> queryBuilder, string queryValue)
        {
            DateTime dateValue;

            if (queryValue.StartsWith("gt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.DueBefore > dateValue);
            }

            if (queryValue.StartsWith("gte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.DueBefore >= dateValue);
            }

            if (queryValue.StartsWith("lt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.DueBefore < dateValue);
            }

            if (queryValue.StartsWith("lte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.DueBefore <= dateValue);
            }

            if (queryValue.StartsWith("eq:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.DueBefore == dateValue);
            }

            dateValue = ParseDateTimeIntoUtc(queryValue);
            return queryBuilder.Where(i => i.DueBefore == dateValue);
        }

        // Limitations in queryBuilder.Where interface forces me to duplicate the datetime methods
        private IQueryable<Instance> QueryBuilderForLastChangedDateTime(IQueryable<Instance> queryBuilder, string queryValue)
        {
            DateTime dateValue;

            if (queryValue.StartsWith("gt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.LastChanged > dateValue);
            }

            if (queryValue.StartsWith("gte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.LastChanged >= dateValue);
            }

            if (queryValue.StartsWith("lt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.LastChanged < dateValue);
            }

            if (queryValue.StartsWith("lte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.LastChanged <= dateValue);
            }

            if (queryValue.StartsWith("eq:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.LastChanged == dateValue);
            }

            dateValue = ParseDateTimeIntoUtc(queryValue);
            return queryBuilder.Where(i => i.LastChanged == dateValue);
        }

        // Limitations in queryBuilder.Where interface forces me to duplicate the datetime methods
        private IQueryable<Instance> QueryBuilderForEnded(IQueryable<Instance> queryBuilder, string queryValue)
        {
            DateTime dateValue;

            if (queryValue.StartsWith("gt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.Process.Ended > dateValue);
            }

            if (queryValue.StartsWith("gte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.Process.Ended >= dateValue);
            }

            if (queryValue.StartsWith("lt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.Process.Ended < dateValue);
            }

            if (queryValue.StartsWith("lte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.Process.Ended <= dateValue);
            }

            if (queryValue.StartsWith("eq:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.Process.Ended == dateValue);
            }

            dateValue = ParseDateTimeIntoUtc(queryValue);
            return queryBuilder.Where(i => i.Process.Ended == dateValue);
        }

        // Limitations in queryBuilder.Where interface forces me to duplicate the datetime methods
        private IQueryable<Instance> QueryBuilderForCreated(IQueryable<Instance> queryBuilder, string queryValue)
        {
            DateTime dateValue;

            if (queryValue.StartsWith("gt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.Created > dateValue);
            }

            if (queryValue.StartsWith("gte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.Created >= dateValue);
            }

            if (queryValue.StartsWith("lt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.Created < dateValue);
            }

            if (queryValue.StartsWith("lte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.Created <= dateValue);
            }

            if (queryValue.StartsWith("eq:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.Created == dateValue);
            }

            dateValue = ParseDateTimeIntoUtc(queryValue);
            return queryBuilder.Where(i => i.Created == dateValue);
        }

        // Limitations in queryBuilder.Where interface forces me to duplicate the datetime methods
        private IQueryable<Instance> QueryBuilderForVisibleAfter(IQueryable<Instance> queryBuilder, string queryValue)
        {
            DateTime dateValue;

            if (queryValue.StartsWith("gt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.VisibleAfter > dateValue);
            }

            if (queryValue.StartsWith("gte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.VisibleAfter >= dateValue);
            }

            if (queryValue.StartsWith("lt:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.VisibleAfter < dateValue);
            }

            if (queryValue.StartsWith("lte:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(4));
                return queryBuilder.Where(i => i.VisibleAfter <= dateValue);
            }

            if (queryValue.StartsWith("eq:"))
            {
                dateValue = ParseDateTimeIntoUtc(queryValue.Substring(3));
                return queryBuilder.Where(i => i.VisibleAfter == dateValue);
            }

            dateValue = ParseDateTimeIntoUtc(queryValue);
            return queryBuilder.Where(i => i.VisibleAfter == dateValue);
        }

        private static DateTime ParseDateTimeIntoUtc(string queryValue)
        {
            return DateTimeHelper.ParseAndConvertToUniversalTime(queryValue);
        }

        /// <inheritdoc/>
        public async Task<Instance> GetOne(string instanceId, int instanceOwnerPartyId)
        {
            string cosmosId = InstanceIdToCosmosId(instanceId);
            Uri uri = UriFactory.CreateDocumentUri(databaseId, collectionId, cosmosId);

            Instance instance = await _client
                .ReadDocumentAsync<Instance>(
                    uri,
                    new RequestOptions { PartitionKey = new PartitionKey(instanceOwnerPartyId.ToString()) });

            PostProcess(instance);

            return instance;
        }

        /// <inheritdoc/>
        public async Task<List<Instance>> GetInstancesOfInstanceOwner(int instanceOwnerPartyId)
        {
            string instanceOwnerPartyIdString = instanceOwnerPartyId.ToString();

            FeedOptions feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(instanceOwnerPartyIdString),
                MaxItemCount = 100,
            };

            IQueryable<Instance> filter = _client
                .CreateDocumentQuery<Instance>(collectionUri, feedOptions)
                .Where(i => i.InstanceOwner.PartyId == instanceOwnerPartyIdString);

            IDocumentQuery<Instance> query = filter.AsDocumentQuery<Instance>();

            FeedResponse<Instance> feedResponse = await query.ExecuteNextAsync<Instance>();

            List<Instance> instances = feedResponse.ToList<Instance>();

            PostProcess(instances);

            return instances;
        }

        /// <inheritdoc/>
        public async Task<List<Instance>> GetInstancesInStateOfInstanceOwner(int instanceOwnerPartyId, string instanceState)
        {
            List<Instance> instances = new List<Instance>();
            string instanceOwnerPartyIdString = instanceOwnerPartyId.ToString();

            FeedOptions feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(instanceOwnerPartyIdString)
            };

            IQueryable<Instance> filter = null;

            if (instanceState.Equals("active"))
            {
                filter = _client.CreateDocumentQuery<Instance>(collectionUri, feedOptions)
                        .Where(i => i.InstanceOwner.PartyId == instanceOwnerPartyIdString)
                        .Where(i => (!i.VisibleAfter.HasValue || i.VisibleAfter <= DateTime.UtcNow))
                        .Where(i => !i.Status.SoftDeleted.HasValue)
                        .Where(i => !i.Status.HardDeleted.HasValue)
                        .Where(i => !i.Status.Archived.HasValue);
            }
            else if (instanceState.Equals("deleted"))
            {
                filter = _client.CreateDocumentQuery<Instance>(collectionUri, feedOptions)
                        .Where(i => i.InstanceOwner.PartyId == instanceOwnerPartyIdString)
                        .Where(i => i.Status.SoftDeleted.HasValue)
                        .Where(i => i.Status.HardDeleted.HasValue);
            }
            else if (instanceState.Equals("archived"))
            {
                filter = _client.CreateDocumentQuery<Instance>(collectionUri, feedOptions)
                       .Where(i => i.InstanceOwner.PartyId == instanceOwnerPartyIdString)
                       .Where(i => i.Status.Archived.HasValue)
                       .Where(i => !i.Status.SoftDeleted.HasValue)
                       .Where(i => !i.Status.HardDeleted.HasValue);
            }
            else
            {
                return instances;
            }

            IDocumentQuery<Instance> query = filter.AsDocumentQuery<Instance>();

            FeedResponse<Instance> feedResponse = await query.ExecuteNextAsync<Instance>();

            instances = feedResponse.ToList<Instance>();

            PostProcess(instances);

            return instances;
        }

        /// <inheritdoc/>
        public async Task<Instance> Update(Instance item)
        {
            PreProcess(item);

            ResourceResponse<Document> createDocumentResponse = await _client
                .ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, item.Id), item);
            Document document = createDocumentResponse.Resource;
            Instance instance = JsonConvert.DeserializeObject<Instance>(document.ToString());

            PostProcess(instance);

            return instance;
        }

        /// <summary>
        /// Converts the instanceId (id) of the instance from {instanceOwnerPartyId}/{instanceGuid} to {instanceGuid} to use as id in cosmos.
        /// </summary>
        /// <param name="instance">the instance to preprocess</param>
        private void PreProcess(Instance instance)
        {
            instance.Id = InstanceIdToCosmosId(instance.Id);
        }

        /// <summary>
        /// Converts the instanceId (id) of the instance from {instanceGuid} to {instanceOwnerPartyId}/{instanceGuid} to be used outside cosmos.
        /// </summary>
        /// <param name="instance">the instance to preprocess</param>
        private void PostProcess(Instance instance)
        {
            instance.Id = $"{instance.InstanceOwner.PartyId}/{instance.Id}";
        }

        /// <summary>
        /// Preprosesses a list of instances.
        /// </summary>
        /// <param name="instances">the list of instances</param>
        private void PostProcess(List<Instance> instances)
        {
            instances.ForEach(i => PostProcess(i));
        }

        /// <summary>
        /// An instanceId should follow this format {int}/{guid}.
        /// Cosmos does not allow / in id.
        /// But in some old cases instanceId is just {guid}.
        /// </summary>
        /// <param name="instanceId">the id to convert to cosmos</param>
        /// <returns>the guid of the instance</returns>
        private string InstanceIdToCosmosId(string instanceId)
        {
            string cosmosId = instanceId;

            if (instanceId != null && instanceId.Contains("/"))
            {
                cosmosId = instanceId.Split("/")[1];
            }

            return cosmosId;
        }
    }
}
