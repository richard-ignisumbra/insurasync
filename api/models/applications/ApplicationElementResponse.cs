using System;
namespace permaAPI.models.applications
{
    public class ApplicationElementResponse
    {

        public int ApplicationId { get; set; }
        public int ElementId { get; set; }
        public int RowId { get; set; }
        public decimal? CurrencyResponse { get; set; }
        public string TextResponse { get; set; }
        public string LongTextResponse { get; set; }
        public int? IntResponse { get; set; }
        public Boolean? BitResponse { get; set; }
        public DateTime? DateResponse { get; set; }


        public ApplicationElementResponse()
        {


        }
    }
}

