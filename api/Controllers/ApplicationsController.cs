using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using permaAPI.models.applications;
using static System.Net.Mime.MediaTypeNames;
using permaAPI.services;
using ClosedXML.Excel;
using System.Net.Http;
using System.Net;
using permaAPI.models.Requests;
namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/Applications")]
    public class ApplicationsController : ControllerBase
    {
        private IApplicationsService _applicationsService;
        private IApplicationTypeService _applicationTypeService;
        private EmailService _emailService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public ApplicationsController(IApplicationsService applicationsService, EmailService emailService, IApplicationTypeService applicationTypeService)
        {
            _applicationsService = applicationsService;
            _emailService = emailService;
            _applicationTypeService = applicationTypeService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<models.applications.ApplicationPreview>> GetApplicationPreview(int coverageYear, string status = "all", int? reportTypeId = null, string lineofCoverage = "all")
        {
            if (_applicationsService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            string email = "";
            try
            {
                email = User.FindFirst("emails").Value;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }


            var result = _applicationsService.GetApplications(coverageYear, userId.Value, status, reportTypeId, email, lineofCoverage).ToList();
            return result;
        }

        [HttpGet]
        [Route("filtered")]
        public ActionResult<IEnumerable<models.applications.ApplicationPreview>> GetApplicationPreviewFiltered(int coverageYear, string status = "all", int? applicationType = null, string filterQuarter = null, string filterMonth = null, string lineofCoverage = "all")
        {
            if (_applicationsService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            string email = "";
            try
            {
                email = User.FindFirst("emails").Value;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }


            var result = _applicationsService.GetApplicationsFiltered(coverageYear, userId.Value, status, applicationType, email, filterQuarter, filterMonth, lineofCoverage).ToList();
            return result;
        }
        [HttpGet]
        [Route("{id}")]
        public ActionResult<models.applications.Application> GetApplication(int id)
        {
            if (_applicationsService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _applicationsService.GetApplication(id, userId.Value);
            if (result.ApplicationStatus.ToLower() == "new")
            {

                result.ApplicationStatus = "Open";
                _applicationsService.SaveApplication(result, userId.Value);
            }


            return result;
        }

        [HttpPut]
        [Route("")]
        public ActionResult<int> CreateApplication(permaAPI.models.applications.NewApplication newApplication)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_applicationsService == null)
            {
                return NotFound();
            }
            var result = _applicationsService.CreateApplication(newApplication, userId.Value);
            return result;
        }
        [HttpPost]
        [Route("{applicationId}")]
        public ActionResult<string> UpdateApplication(permaAPI.models.applications.Application application, int applicationId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_applicationsService == null)
            {
                return NotFound();
            }
            application.ApplicationId = applicationId;
            var result = _applicationsService.SaveApplication(application, userId.Value);
            return result;
        }
        [HttpPost]
        [Route("{applicationId}/Submit")]
        public async Task<ActionResult<string>> SubmitApplication(int applicationId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_applicationsService == null)
            {
                return NotFound();
            }

            var app = _applicationsService.GetApplication(applicationId, userId.Value);
            app.ApplicationStatus = "Submitted";

            var result = _applicationsService.SaveApplication(app, userId.Value);
            await this._emailService.SendSubmittedNotification(app);


            return result;
        }
        [HttpPost]
        [Route("{coverageYear}/export")]
        public async Task<FileStreamResult> ExportApplications(int coverageYear, ExportApplicationsRequest applicationsRequest)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var report = _applicationTypeService.GetApplicationReport(applicationsRequest.reportId);
            if (report != null)
            {
                switch (report.ReportAction)
                {
                    case "annualExport":
                        return await this.GetAnnualReport(coverageYear, report.reportTypeId, applicationsRequest.status, userId.Value, applicationsRequest.filterMonth, applicationsRequest.filterQuarter, applicationsRequest.lineofCoverage, applicationsRequest.applicationType);
                    case "PayrollExport":
                        return await this.GetPayrollReport(coverageYear, report.reportTypeId, applicationsRequest.status, userId.Value, applicationsRequest.lineofCoverage, applicationsRequest.applicationType);
                    default:
                        throw new Exception("invalid report type");

                }
            }
            else { throw new Exception("missing report"); }


        }

        [HttpPost]
        [Route("{coverageYear}/Defaultexport")]
        public async Task<FileStreamResult> DefaultExportApplications(int coverageYear, DefaultExportApplicationsRequest applicationsRequest)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var defaultReport = _applicationTypeService.GetApplicationDefaultReport(applicationsRequest.reportTypeId);
            if (defaultReport != null)
            {
                switch (defaultReport.ReportAction)
                {
                    case "annualExport":
                        return await this.GetAnnualReport(coverageYear, defaultReport.reportTypeId, applicationsRequest.status, userId.Value, applicationsRequest.filterMonth, applicationsRequest.filterQuarter, applicationsRequest.lineofCoverage, applicationsRequest.applicationType);
                    case "PayrollExport":
                        return await this.GetPayrollReport(coverageYear, defaultReport.reportTypeId, applicationsRequest.status, userId.Value, applicationsRequest.lineofCoverage, applicationsRequest.applicationType);
                    default:
                        throw new Exception("invalid report type");

                }
            }
            else { throw new Exception("missing report"); }


        }




        private string GetValue(permaAPI.enums.ElementType elementType, permaAPI.models.ResponseSummary response)
        {
            string value = "";
            switch (elementType)
            {
                case permaAPI.enums.ElementType.Attachment:
                    value = response.LongTextResponse;
                    break;
                case permaAPI.enums.ElementType.Currency:
                    value = (response.CurrencyResponse.HasValue) ? response.CurrencyResponse.Value.ToString() : "";
                    break;
                case permaAPI.enums.ElementType.LargeText:
                    value = response.LongTextResponse;
                    break;
                case permaAPI.enums.ElementType.Checkbox:
                    value = response.LongTextResponse;
                    break;
                case permaAPI.enums.ElementType.Text:
                    value = response.TextResponse;
                    break;
                case permaAPI.enums.ElementType.Integer:
                    value = (response.IntResponse.HasValue) ? response.IntResponse.Value.ToString() : "";
                    break;
                case permaAPI.enums.ElementType.Table:
                case permaAPI.enums.ElementType.SingleSelect:
                    value = response.LongTextResponse;
                    break;
                case permaAPI.enums.ElementType.MultiSelect:
                    value = response.LongTextResponse;
                    break;
                case permaAPI.enums.ElementType.HTML:
                    value = response.LongTextResponse;
                    break;
                case permaAPI.enums.ElementType.Email:
                    value = response.TextResponse;
                    break;
                case permaAPI.enums.ElementType.Date:
                    value = (response.DateResponse.HasValue) ? response.DateResponse.Value.ToString() : "";
                    break;
                default:
                    break;


            }
            return value;
        }

        private async Task<FileStreamResult> GetAnnualReport(int coverageYear, int reportTypeId, string status, string userIdentifier, int? filterMonth, int? filterYear, string lineofCoverage, int? applicationType)
        {
            var results = _applicationsService.ExportNonTableData(coverageYear, status, reportTypeId, userIdentifier, filterMonth, filterYear, lineofCoverage, applicationType);
            var ms = new System.IO.MemoryStream();


            using (var workbook = new XLWorkbook())
            {
                string applicationName = "Renewal";
                if (results.MemberSummaries.Count > 0)
                {
                    applicationName = results.MemberSummaries[0].ApplicationName;
                }
                var worksheet = workbook.Worksheets.Add("Member Profile");
                worksheet.Cell("A1").Value = "PERMA";
                worksheet.Cell("F1").Value = "Updated:";
                worksheet.Cell("H2").Value = DateTime.Now.ToShortDateString();
                worksheet.Cell("B2").Value = "Statistical and";
                worksheet.Cell("B3").Value = "Exposure Information";
                worksheet.Cell("B6").Value = "Entity";
                worksheet.Cell("D5").Value = applicationName;
                worksheet.Cell("D6").Value = "Application";
                //6, 6 beginning cell

                int columnStart = 6;
                string currentExportCategory = "-1";
                for (int i = 0; i < results.ApplicationQuestions.Count; i++)
                {
                    worksheet.Cell(6, columnStart + i).Value = results.ApplicationQuestions[i].ColumnName;
                    if (results.ApplicationQuestions[i].ExportCategory != currentExportCategory)
                    {
                        currentExportCategory = results.ApplicationQuestions[i].ExportCategory;
                        worksheet.Cell(5, columnStart + i).Value = currentExportCategory;
                    }
                }
                int memberRowStart = 7;
                for (int rowCount = 0; rowCount < results.MemberSummaries.Count; rowCount++)
                {
                    var appSummary = results.MemberSummaries[rowCount];
                    worksheet.Cell(memberRowStart + rowCount, 2).Value = appSummary.MemberName;
                    for (int i = 0; i < results.ApplicationQuestions.Count; i++)
                    {
                        var question = results.ApplicationQuestions[i];
                        var answer = (from a in results.ApplicationResponses where a.ElementId == question.ElementId && a.RowId == 0 && a.ApplicationId == appSummary.ApplicationId select a).FirstOrDefault();
                        if (answer != null)
                        {
                            worksheet.Cell(memberRowStart + rowCount, columnStart + i).Value = this.GetValue(question.ElementType, answer);
                        }

                    }
                }
                // summaries
                int summaryRowStart = 8 + results.MemberSummaries.Count;
                worksheet.Range(worksheet.Cell(summaryRowStart, 2), worksheet.Cell(summaryRowStart, 3)).Merge();
                worksheet.Cell(summaryRowStart, 2).Value = "Totals";
                worksheet.Cell(summaryRowStart, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                int totalEnd = results.ApplicationQuestions.Count + 6;
                for (int totalcellIndex = 2; totalcellIndex < totalEnd; totalcellIndex++)
                {
                    worksheet.Cell(summaryRowStart, totalcellIndex).Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                }

                for (int i = 0; i < results.ApplicationQuestions.Count; i++)
                {
                    switch (results.ApplicationQuestions[i].ElementType)
                    {
                        case enums.ElementType.Currency:
                            var matchingValues = (from x in results.ApplicationResponses where x.ElementId == results.ApplicationQuestions[i].ElementId select x).ToList();
                            decimal sum = 0;
                            foreach (var item in matchingValues)
                            {
                                sum += (item.IntResponse.HasValue) ? item.IntResponse.Value : 0;
                            }
                            worksheet.Cell(summaryRowStart, columnStart + i).Value = sum;
                            break;
                        case enums.ElementType.Integer:
                            var matchingValues2 = (from x in results.ApplicationResponses where x.ElementId == results.ApplicationQuestions[i].ElementId select x).ToList();
                            int sum2 = 0;
                            foreach (var item in matchingValues2)
                            {
                                sum2 += (item.IntResponse.HasValue) ? item.IntResponse.Value : 0;
                            }
                            worksheet.Cell(summaryRowStart, columnStart + i).Value = sum2;
                            break;
                        default:
                            break;
                    }


                }

                workbook.SaveAs(ms);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                var array = ms.ToArray();



                string fileName = $"exportedData-{coverageYear}.xlsx";


                return File(new System.IO.MemoryStream(array), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

        }

        private async Task<FileStreamResult> GetPayrollReport(int coverageYear, int reportTypeId, string status, string userIdentifier, string lineofCoverage, int? applicationType)
        {
            var results = _applicationsService.ExportPayrollTableData(coverageYear, status, reportTypeId, userIdentifier, lineofCoverage, applicationType);
            var ms = new System.IO.MemoryStream();


            using var wbSource = new XLWorkbook("./reports/MemberPayroll_ByClass.xlsx");


            using (var outputWorkbook = new XLWorkbook())
            {

                if (results.Applications.Count == 0)
                {
                    var worksheet = outputWorkbook.AddWorksheet();
                    worksheet.Cell("A1").Value = "No applications returned";
                }

                foreach (var result in results.Applications)
                {



                    var worksheet = wbSource.Worksheet(1).CopyTo(outputWorkbook, "payroll-" + result.ApplicationId);

                    worksheet.Cell("E11").Value = result.memberName;
                    worksheet.Cell("E12").Value = result.CompletingName;
                    worksheet.Cell("E13").Value = result.Email;
                    //worksheet.Cell("E15").Value = result.Payroll;
                    //6, 6 beginning cell
                    foreach (var item in result.SafetyPayroll)
                    {
                        int row = 0;
                        switch (item.code)
                        {
                            case "7706":
                                row = 20;
                                break;
                            case "7707":
                                row = 21;
                                break;
                            case "7720":
                                row = 22;
                                break;
                            case "7722":
                                row = 23;
                                break;
                            case "8810":
                                row = 27;
                                break;
                            case "8871":
                                row = 28;
                                break;
                            case "9410":
                                row = 29;
                                break;
                            case "9420":
                                row = 30;
                                break;
                            case "7382":
                                row = 31;
                                break;
                            case "7429":
                                row = 32;
                                break;
                            case "7520":
                                row = 33;
                                break;
                            case "7539":
                                row = 34;
                                break;
                            case "7580":
                                row = 35;
                                break;
                            case "8742":
                                row = 36;
                                break;
                            case "9422":
                                row = 37;
                                break;
                            default:
                                break;
                        }
                        if (row > 0)
                        {
                            worksheet.Cell($"E{row}").Value = item.payroll;
                            worksheet.Cell($"F{row}").Value = item.numEmployees;
                        }

                    }
                    foreach (var item in result.NonSafetyPayroll)
                    {
                        int row = 0;
                        switch (item.code)
                        {
                            case "7706":
                                row = 20;
                                break;
                            case "7707":
                                row = 21;
                                break;
                            case "7720":
                                row = 22;
                                break;
                            case "7722":
                                row = 23;
                                break;
                            case "8810":
                                row = 27;
                                break;
                            case "8871":
                                row = 28;
                                break;
                            case "9410":
                                row = 29;
                                break;
                            case "9420":
                                row = 30;
                                break;
                            case "7382":
                                row = 31;
                                break;
                            case "7429":
                                row = 32;
                                break;
                            case "7520":
                                row = 33;
                                break;
                            case "7539":
                                row = 34;
                                break;
                            case "7580":
                                row = 35;
                                break;
                            case "8742":
                                row = 36;
                                break;
                            case "9422":
                                row = 37;
                                break;
                            default:
                                break;
                        }
                        if (row > 0)
                        {
                            worksheet.Cell($"E{row}").Value = item.payroll;
                            worksheet.Cell($"F{row}").Value = item.numEmployees;
                        }

                    }
                }



                outputWorkbook.SaveAs(ms);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                var array = ms.ToArray();



                string fileName = $"exportedData-{coverageYear}.xlsx";


                return File(new System.IO.MemoryStream(array), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }


}