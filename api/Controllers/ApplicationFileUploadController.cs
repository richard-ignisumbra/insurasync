using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/ApplicationFileUpload")]

    public class ApplicationFileUploadController : ControllerBase
    {
        private services.IApplicationFileService _fileApplicationService;
        private services.IApplicationElementsService _elementService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public ApplicationFileUploadController(services.IApplicationFileService appFileService, services.IApplicationElementsService elementService)
        {
            _fileApplicationService = appFileService;
            _elementService = elementService;
        }

        [HttpGet]
        [Route("{applicationId}/{elementId}/{rowId}")]
        public ActionResult<IEnumerable<string>> GetElementUploads(int applicationId, int elementId, int rowId)
        {
            var exampleResponse = new List<string>();
            return new ActionResult<IEnumerable<string>>(exampleResponse);
        }
        [HttpGet]
        [Route("{applicationId}/{elementId}/{rowId}/download")]
        public async Task<ActionResult<System.Uri>> GetFileDownload(int applicationId, int elementId, int rowId, string fileId)
        {
            /* Setup File Uploads */
            var config = this._fileApplicationService.GetConfig();
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
        new StorageSharedKeyCredential(config.AccountName, config.AccountKey);
            string blobUri = "https://" + config.AccountName + ".blob.core.windows.net";
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var blobServiceClient = new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
            var container = blobServiceClient.GetBlobContainerClient("memberuploads");
            await container.CreateIfNotExistsAsync();
            var blobClient = container.GetBlobClient(fileId);
            string storedPolicyName = null;
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = container.Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read |
                        BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                Console.WriteLine("SAS URI for blob is: {0}", sasUri);
                Console.WriteLine();

                return sasUri;
            }
            else
            {
                throw new Exception("error insufficient permissions");
            }
        }


        [HttpPost]
        [Route("{applicationId}/{elementId}/{rowId}")]
        public async Task<ActionResult<string>> SaveElement(List<IFormFile> files, int applicationId, int rowId, int elementId)
        {


            var combinedFiles = new List<models.uploads.fileUpload>();
            var elementResponse = _elementService.GetResponse(elementId, applicationId, rowId);
            if (elementResponse == null)
            {
                elementResponse = new models.applications.ApplicationElementResponse()
                {
                    ApplicationId = applicationId,
                    BitResponse = false,
                    CurrencyResponse = 0,
                    RowId = rowId,
                    ElementId = elementId
                };

            }
            else
            {
                try
                {
                    List<models.uploads.fileUpload> existingFiles = System.Text.Json.JsonSerializer.Deserialize<List<models.uploads.fileUpload>>(elementResponse.LongTextResponse);
                    combinedFiles.AddRange(existingFiles);
                }
                catch (Exception ex)
                {

                }
            }
            /* Setup File Uploads */
            var config = this._fileApplicationService.GetConfig();
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
        new StorageSharedKeyCredential(config.AccountName, config.AccountKey);
            string blobUri = "https://" + config.AccountName + ".blob.core.windows.net";
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var blobServiceClient = new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
            var container = blobServiceClient.GetBlobContainerClient("memberuploads");
            await container.CreateIfNotExistsAsync();


            foreach (var file in files)
            {
                string fileName = $"{applicationId.ToString("D3")}_{elementId.ToString("D3")}_{rowId.ToString("D3")}_{combinedFiles.Count.ToString("D3")}_{file.FileName}";
                var blob = container.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream())
                {
                    await blob.DeleteIfExistsAsync();
                    await blob.UploadAsync(stream);
                }
                combinedFiles.Add(new models.uploads.fileUpload() { FileId = fileName, FileName = file.FileName });
            }


            elementResponse.LongTextResponse = System.Text.Json.JsonSerializer.Serialize(combinedFiles);
            this._elementService.SaveApplicationResponse(elementResponse, applicationId, userId.Value);

            return elementResponse.LongTextResponse;
        }

    }

}