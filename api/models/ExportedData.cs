using System;
using System.Collections.Generic;
namespace permaAPI.models
{
    public class ExportedData
    {
        public ExportedData()
        {
            this.MemberSummaries = new List<MemberSummary>();
            this.ApplicationQuestions = new List<ApplicationQuestion>();
            this.ApplicationResponses = new List<ResponseSummary>();
        }
        public int Coverageyear { get; set; }

        public IList<MemberSummary> MemberSummaries { get; set; }
        public IList<ApplicationQuestion> ApplicationQuestions { get; set; }
        public IList<ResponseSummary> ApplicationResponses { get; set; }

    }

    public class MemberSummary
    {
        public int MemberId { get; set; }
        public int ApplicationId { get; set; }
        public string MemberName { get; set; }
        public string ApplicationName { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodQuarter { get; set; }
    }

    public class ApplicationQuestion
    {
        public int ElementId { get; set; }
        public string ColumnName { get; set; }
        public enums.ElementType ElementType { get; set; }
        public string OriginalSource { get; set; }
        public string ExportCategory { get; set; }
    }
    public class ResponseSummary
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

    }

    public class PayrollExport
    {
        public IList<ApplicationPayroll> Applications { get; set; }


    }
    public class ApplicationPayroll
    {
        public int ApplicationId { get; set; }
        public string memberName { get; set; }
        public string CompletingName { get; set; }
        public string Email { get; set; }
        public decimal Payroll { get; set; }
        public int? periodQuarter { get; set; }
        public int? periodMonth { get; set; }
        public IList<PayrollRow> SafetyPayroll { get; set; }

        public IList<PayrollRow> NonSafetyPayroll { get; set; }

    }
    public class PayrollRow
    {
        public int Row { get; set; }
        public string code { get; set; }
        public string classification { get; set; }
        public decimal payroll { get; set; }
        public int numEmployees { get; set; }
    }
}

