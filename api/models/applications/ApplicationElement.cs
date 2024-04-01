using System;
namespace permaAPI.models.applications
{
    public class ApplicationElement
    {
        public ApplicationElement()
        {
        }

        public int ElementId { get; set; }
        public string LongText { get; set; }
        public string ShortName { get; set; }
        public enums.ElementType ElementType { get; set; }
        public int? TableSectionId { get; set; }
        public int? DecimalPrecision { get; set; }
        public Boolean isRequired { get; set; }
        public int? MaxLength { get; set; }
        public int? IndentSpaces { get; set; }
        public Boolean AllowNewRows { get; set; }
        public Boolean Readonly { get; set; }
        public int? Width { get; set; }
        public string Label { get; set; }
        public Boolean SumValues { get; set; }
        public System.Collections.Generic.List<models.ListValue> SelectOptions { get; set; }
        public System.Collections.Generic.List<applications.ApplicationElementResponse> Responses { get; set; }
        public System.Collections.Generic.List<applications.ApplicationElementResponse> PreviousResponses { get; set; }
    }
}

