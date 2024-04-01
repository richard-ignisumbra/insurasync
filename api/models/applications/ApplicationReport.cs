using System;
namespace permaAPI.models.applications
{
	public class ApplicationReport
	{
		public ApplicationReport()
		{
		}
		public int ReportId { get; set; }
		public string ReportName { get; set; }
		public string ReportDescription { get; set; }
		public string ReportAction { get; set; }
		public int reportTypeId { get; set; }
	}
}

