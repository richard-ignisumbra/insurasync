
namespace permaAPI.models.applications
{
    public class NewApplication
    {
        public int ApplicationTypeId { get; set; }
        public int MemberId { get; set; }
        public string ApplicationName { get; set; }
        public int CoverageYear { get; set; }
        public string ApplicationStatus { get; set; }
        public System.DateTime? DueDate { get; set; }
        public System.DateTime? CompletedDate { get; set; }
        public string CompletedBy { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodQuarter { get; set; }
    }
}
