using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Models;
using AltinnCore.Common.Clients;
using AltinnCore.Common.Services.Interfaces;
using AltinnCore.ServiceLibrary.Models;
using AltinnCore.ServiceLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AltinnCore.Common.Services.Implementation
{
    /// <inheritdoc/>
    public class PDFSI : IPDF
    {
        private ILogger _logger;
        private HttpClient _pdfClient;
        private HttpClient _storageClient;

        private IData _dataService;
        private IRepository _repositoryService;
        private IRegister _registerService;
        private JsonSerializer _camelCaseSerializer;
        private string pdfElementType = "ref-data-as-pdf";
        private string pdfFileName = "receipt.pdf";

        /// <summary>
        /// Creates a new instance of the <see cref="PDFSI"/> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="httpClientAccessor">The http client accessor</param>
        /// <param name="dataService">The data service</param>
        /// <param name="repositoryService">The repository service</param>
        /// <param name="registerService">The register service</param>
        public PDFSI(ILogger<PrefillSI> logger, IHttpClientAccessor httpClientAccessor, IData dataService, IRepository repositoryService, IRegister registerService)
        {
            _logger = logger;
            _pdfClient = httpClientAccessor.PdfClient;
            _storageClient = httpClientAccessor.StorageClient;
            _dataService = dataService;
            _repositoryService = repositoryService;
            _registerService = registerService;
            _camelCaseSerializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        /// <inheritdoc/>
        public async Task GenerateAndStoreReceiptPDF(Instance instance, UserContext userContext)
        {
            string app = instance.AppId.Split("/")[1];
            string org = instance.Org;
            int instanceOwnerId = int.Parse(instance.InstanceOwnerId);
            Guid instanceGuid = Guid.Parse(instance.Id.Split("/")[1]);
            Guid defaultDataElementGuid = Guid.Parse(instance.Data.Find(element => element.ElementType.Equals("default"))?.Id);
            Stream dataStream = await _dataService.GetBinaryData(org, app, instanceOwnerId, instanceGuid, defaultDataElementGuid);
            byte[] dataAsBytes = new byte[dataStream.Length];
            dataStream.Read(dataAsBytes);
            string encodedXml = System.Convert.ToBase64String(dataAsBytes);

            PDFContext pdfContext = new PDFContext
            {
                Data = encodedXml,
                FormLayout = JObject.Parse(_repositoryService.GetJsonFormLayout(org, app)),
                TextResources = _repositoryService.GetServiceTexts(org, app),
                Party = await _registerService.GetParty(instanceOwnerId),
                UserParty = userContext.Party,
                Instance = instance
            };

            Stream pdfContent;
            try
            {
                pdfContent = await GeneratePDF(pdfContext);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Could not generate pdf for {instance.Id}, failed with message {exception.Message}");
                return;
            }

            try
            {
               await StorePDF(pdfContent, instance);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Could not store pdf for {instance.Id}, failed with message {exception.Message}");
                return;
            }
            finally
            {
                pdfContent.Dispose();
            }
        }

        private async Task<Stream> GeneratePDF(PDFContext pdfContext)
        {
            using (HttpContent data = new StringContent(JObject.FromObject(pdfContext, _camelCaseSerializer).ToString(), Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await _pdfClient.PostAsync("generate", data);
                response.EnsureSuccessStatusCode();
                Stream pdfContent = await response.Content.ReadAsStreamAsync();
                return pdfContent;
            }
        }

        private async Task<DataElement> StorePDF(Stream pdfStream, Instance instance)
        {
            using (StreamContent content = new StreamContent(pdfStream))
            {
                return await _dataService.InsertBinaryData(
                    instance.Org,
                    instance.AppId.Split("/")[1],
                    int.Parse(instance.InstanceOwnerId),
                    Guid.Parse(instance.Id.Split("/")[1]),
                    pdfElementType,
                    pdfFileName,
                    content);
            }
        }
    }
}
