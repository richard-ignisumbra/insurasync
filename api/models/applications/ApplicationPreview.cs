
namespace permaAPI.models.applications
{
    public class ApplicationPreview
    {
        public int ApplicationId { get; set; }
        public int ApplicationTypeId { get; set; }
        public string ApplicationType { get; set; }
        public int MemberId { get; set; }
        public string Member { get; set; }
        public string ApplicationName { get; set; }
        public int CoverageYear { get; set; }
        public string ApplicationStatus { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodQuarter { get; set; }
        public System.DateTime? DueDate { get; set; }
        public System.DateTime? CompletedDate { get; set; }
        public string CompletedBy { get; set; }
        public int? CurrentSectionId { get; set; }
        public int? PreviousApplicationId { get; set; }
    }
}
