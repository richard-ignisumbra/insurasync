using System;
using Microsoft.AspNetCore.Http;
namespace permaAPI.models.members
{
    public class Attachment
    {
        public int? AttachmentId { get; set; }
        public int MemberId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int PolicyPeriod { get; set; }
        public string CreatedBy { get; set; }
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string applicationUser { get; set; }
        public DateTime ModifiedByDate { get; set; }
        public string ModifiedBy { get; set; }
        public int ApplicationId { get; set; }
        public int RowId { get; set; }
        public int ElementId { get; set; }
        public Attachment()
        {
        }
    }

    public class AttachmentUpload : Attachment
    {
        public IFormFile FileUpload { get; set; }

        public AttachmentUpload()
        {
        }
    }
}

