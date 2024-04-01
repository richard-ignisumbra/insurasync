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
using System.Net.Http;

namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/MemberFileUpload")]
    public class MemberFileUploadController : ControllerBase
    {
        private services.IApplicationFileService _fileApplicationService;
        private IMembersService _memberService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public MemberFileUploadController(services.IApplicationFileService appFileService, IMembersService memberService)
        {
            _fileApplicationService = appFileService;
            _memberService = memberService;

        }
        [HttpGet]
        [Route("{attachmentId}/download")]
        public async Task<ActionResult<System.Uri>> GetFileDownload(int attachmentId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var attachment = this._memberService.GetMemberAttachment(attachmentId, userId.Value);
            if (attachment == null)
            {
                throw new Exception("invalid attachment");
            }

            /* Setup File Uploads */
            var config = this._fileApplicationService.GetConfig();
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
        new StorageSharedKeyCredential(config.AccountName, config.AccountKey);
            string blobUri = "https://" + config.AccountName + ".blob.core.windows.net";

            var blobServiceClient = new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
            var container = blobServiceClient.GetBlobContainerClient("memberattachments");
            await container.CreateIfNotExistsAsync();
            var blobClient = container.GetBlobClient(attachment.FileName);
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
        [Route("{attachmentId}/changeStatus")]
        public ActionResult<Boolean> ChangeStatus(int attachmentId, string status)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var attachment = this._memberService.GetMemberAttachment(attachmentId, userId.Value);
            if (attachment == null)
            {
                throw new Exception("invalid attachment");
            }
            attachment.Status = status;
            this._memberService.SaveMemberAttachment(attachment.MemberId, attachment, userId.Value);

            return true;
        }


        [HttpPost]
        [Route("{memberId}/{categoryId}/{policyPeriod}/{status}")]
        public async Task<ActionResult<IEnumerable<String>>> SaveAttachments([FromForm] List<IFormFile> files, int memberId, int categoryId, string categoryTitle, int policyPeriod, string fileDescription, string status)
        {
            var fileNames = new List<String>();



            /* Setup File Uploads */
            var config = this._fileApplicationService.GetConfig();
            Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
        new StorageSharedKeyCredential(config.AccountName, config.AccountKey);
            string blobUri = "https://" + config.AccountName + ".blob.core.windows.net";
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var blobServiceClient = new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
            var container = blobServiceClient.GetBlobContainerClient("memberattachments");
            await container.CreateIfNotExistsAsync();


            foreach (var file in files)
            {
                var attachment = new models.members.Attachment();
                attachment.FileName = file.FileName;
                attachment.CategoryId = categoryId;
                attachment.CategoryTitle = categoryTitle;
                attachment.FileDescription = fileDescription;
                attachment.PolicyPeriod = policyPeriod;
                attachment.Status = status;

                attachment.AttachmentId = this._memberService.SaveMemberAttachment(memberId, attachment, userId.Value);

                string fileName = $"{memberId}/{attachment.AttachmentId}/{file.FileName}";

                var blob = container.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blob.DeleteIfExistsAsync();
                    await blob.UploadAsync(stream);
                }
                fileNames.Add(fileName);
            }
            return new ActionResult<IEnumerable<string>>(fileNames); ;
        }

    }

}