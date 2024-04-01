using System;
namespace permaAPI.models.applications
{
    public class ApplicationType
    {
        public ApplicationType()
        {
        }

        public int ApplicationTypeId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string NotifyEmails { get; set; }

        public enums.GroupType GroupType { get; set; }
        public int? AdminPermissionId { get; set; }
        public int? ReportType { get; set; }
    }
}

