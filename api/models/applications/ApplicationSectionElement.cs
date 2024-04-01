using System;
namespace permaAPI.models.applications
{
    public class ApplicationSectionElement
    {
        public ApplicationSectionElement()
        {
        }
        public int ElementId { get; set; }
        public string LongText { get; set; }
        public string ShortName { get; set; }
        public enums.ElementType ElementType { get; set; }
        public int? TableSectionId { get; set; }
        public int? DecimalPrecision { get; set; }
        public Boolean IsRequired { get; set; }
    }
}
