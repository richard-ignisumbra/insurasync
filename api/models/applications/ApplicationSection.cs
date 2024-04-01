using System;
namespace permaAPI.models.applications
{
    public class ApplicationSection
    {
        public ApplicationSection()
        { }
        public int ApplicationSectionId { get; set; }
        public string SectionTitle { get; set; }
        public string Description { get; set; }
        public Boolean isActive { get; set; }
        public int SortOrder { get; set; }
        public string SubTitle { get; set; }
        public Boolean isCompleted { get; set; }

    }
}


