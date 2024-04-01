using System;
namespace permaAPI.models.Requests
{
    public class ExportApplicationsRequest
    {
        public int reportId { get; set; }
        public string status { get; set; } = "all";
        public string lineofCoverage { get; set; } = "all";
        public int? filterQuarter { get; set; }
        public int? filterMonth { get; set; }
        public int? applicationType { get; set; }
    }
}

