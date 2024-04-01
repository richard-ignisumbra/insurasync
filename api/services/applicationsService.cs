using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;

public interface IApplicationsService
{
    public permaAPI.models.applications.Application GetApplication(int applicationId, string userIdentifier);
    public int CreateApplication(permaAPI.models.applications.NewApplication newApplication, string systemUser);
    public IList<permaAPI.models.applications.ApplicationPreview> GetApplications(int coverageYear, string userIdentifier, string status, int? reportType, string email, string lineofCoverage);
    public IList<permaAPI.models.applications.ApplicationPreview> GetApplicationsFiltered(int coverageYear, string userIdentifier, string status, int? applicationType, string email, string filterQuarter, string filterMonth, string lineofCoverage);
    public string SaveApplication(permaAPI.models.applications.Application application, string systemUser);
    public permaAPI.models.ExportedData ExportNonTableData(int coverageyear, string status, int? reportTypeId, string userIdentifier, int? filterMonth, int? filterQuarter, string lineofCoverage, int? applicationType);
    public permaAPI.models.PayrollExport ExportPayrollTableData(int coverageyear, string status, int? reportTypeId, string userIdentifier, string lineofCoverage, int? applicationType);
}
public class ApplicationsService : IApplicationsService
{
    public IConfiguration Configuration { get; }
    public ApplicationsService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public int CreateApplication(permaAPI.models.applications.NewApplication newApplication, string systemUser)
    {

        string connectionString = this.Configuration.GetConnectionString("permaportal");
        int newApplicationId = 0;
        string query = @"INSERT INTO application
            (applicationTypeId, memberId, applicationName, coverageYear, PeriodQuarter, PeriodMonth,
            applicationStatus, dueDate, CompletedDate, CompletedBy, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
            VALUES (@applicationTypeId, @memberId, @applicationName, @coverageYear, @PeriodQuarter, @PeriodMonth,
            @applicationStatus, @dueDate, null, null, @systemUser, getdate(), @systemUser, getdate());
            select @applicationid= Scope_identity();
                
            ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@applicationId", System.Data.SqlDbType.Int));
                cmd.Parameters["@applicationId"].Direction = System.Data.ParameterDirection.InputOutput;
                cmd.Parameters["@applicationId"].Value = System.DBNull.Value;
                conn.Open();
                cmd.Parameters.AddWithValue("@applicationTypeId", newApplication.ApplicationTypeId);
                cmd.Parameters.AddWithValue("@MemberId", newApplication.MemberId);
                cmd.Parameters.AddWithValue("@PeriodQuarter", newApplication.PeriodQuarter.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@PeriodMonth", newApplication.PeriodMonth.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@ApplicationName", newApplication.ApplicationName.EmptyIfDBNullString(250));
                cmd.Parameters.AddWithValue("@CoverageYear", newApplication.CoverageYear);
                cmd.Parameters.AddWithValue("@ApplicationStatus", newApplication.ApplicationStatus.EmptyIfDBNullString(50));
                cmd.Parameters.AddWithValue("@DueDate", newApplication.DueDate);
                cmd.Parameters.AddWithValue("@SystemUser", systemUser);

                cmd.ExecuteNonQuery();
                newApplicationId = cmd.Parameters["@applicationid"].Value.asInt();
            }
        }
        return newApplicationId;
    }
    public IList<permaAPI.models.applications.ApplicationPreview> GetApplications(int coverageYear, string userIdentifier, string status, int? reportType, string email, string lineofCoverage)
    {
        var applicationPreviews = new List<permaAPI.models.applications.ApplicationPreview>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
  if not exists (select * from contact  where email = @email and useridentifier=@useridentifier)
begin
	update contact set useridentifier = @useridentifier where email = @email
end;
    declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
 

SELECT   application.ApplicationId, application.ApplicationTypeId, member.MemberId, member.MemberName, applicationName,
            coverageYear, applicationStatus, DueDate, completedDate, completedBy,
            applicationType.ApplicationType,
            firstSection.ApplicationSectionId [currentSectionId], prevApplication.ApplicationId [PreviousApplicationid], application.periodQuarter, application.periodMonth
            FROM application 
            inner JOIN insured [member] on application.MemberId = member.MemberId
            LEFT JOIN applicationType ON application.applicationTypeId = applicationtype.applicationTypeId
            outer apply (
                SELECT top 1 applicationsectionId 
FROM applicationSection 
 where  ApplicationSection.ShowInNavigation=1 and applicationsection.ApplicationSectionId in (
     select applicationsectionid from ApplicationSectionApplicationTypes where applicationtypeid = application.applicationtypeid
 )
        
 ORDER BY SortOrder  
) [firstSection]
    OUTER APPLY(
                SELECT applicationId from application [prevApp] where application.MemberId = prevApp.MemberId and prevapp.CoverageYear = application.CoverageYear -1 and application.applicationtypeid	 = prevApp.applicationtypeid
            ) [prevApplication]

  where coverageYear=@coverageYear and (@status = 'all' or application.ApplicationStatus = @status) and (@reportType is null or applicationType.ReportType = @reportType)
     and (member.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
        and (@lineofcoverage='all' or  member.MemberId in (select memberid from insuredlineofcoverage where insuredlineofcoverage.LineofCoverage =  @lineofcoverage))
                Order by ApplicationName    ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@coverageYear", coverageYear);
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@reportType", reportType.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@lineofCoverage", lineofCoverage);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationPreview()
                    {
                        ApplicationId = reader["applicationId"].asInt(),
                        CoverageYear = reader["coverageYear"].asInt(),
                        ApplicationTypeId = reader["applicationTypeId"].asInt(),
                        MemberId = reader["memberId"].asInt(),
                        Member = reader["membername"].asString(),
                        ApplicationName = reader["applicationName"].asString(),
                        ApplicationStatus = reader["applicationStatus"].asString(),
                        ApplicationType = reader["ApplicationType"].asString(),
                        PeriodMonth = reader["periodMonth"].asNullableInt(),
                        PeriodQuarter = reader["periodquarter"].asNullableInt(),
                        DueDate = reader["dueDate"].asDate(),
                        CompletedDate = reader["completedDate"].asNullableDate(),
                        CurrentSectionId = reader["currentSectionid"].asNullableInt(),
                        PreviousApplicationId = reader["previousApplicationId"].asNullableInt(),

                    };
                    applicationPreviews.Add(dataItem);
                }
            }
        }
        return applicationPreviews;
    }
    public IList<permaAPI.models.applications.ApplicationPreview> GetApplicationsFiltered(int coverageYear, string userIdentifier, string status, int? applicationType, string email, string filterQuarter, string filterMonth, string lineofCoverage)
    {
        var applicationPreviews = new List<permaAPI.models.applications.ApplicationPreview>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
   if not exists (select * from contact  where email = @email and useridentifier=@useridentifier)
begin
	update contact set useridentifier = @useridentifier where email = @email
end;
    declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
 

SELECT   application.ApplicationId, application.ApplicationTypeId, member.MemberId, member.MemberName, applicationName,  
            coverageYear, applicationStatus, DueDate, completedDate, completedBy,
            applicationType.ApplicationType,
            firstSection.ApplicationSectionId [currentSectionId], prevApplication.ApplicationId [PreviousApplicationid],
        application.periodquarter, application.periodmonth
            FROM application 
            inner JOIN insured [member] on application.MemberId = member.MemberId
            LEFT JOIN applicationType ON application.applicationTypeId = applicationtype.applicationTypeId
            outer apply (
                SELECT top 1 applicationsectionId 
FROM applicationSection 
 where  ApplicationSection.ShowInNavigation=1 and applicationsection.ApplicationSectionId in (
     select applicationsectionid from ApplicationSectionApplicationTypes where applicationtypeid = application.applicationtypeid
 )
        
 ORDER BY SortOrder  
) [firstSection]
    OUTER APPLY(
                SELECT applicationId from application [prevApp] where application.MemberId = prevApp.MemberId and prevapp.CoverageYear = application.CoverageYear -1 and application.applicationtypeid	 = prevApp.applicationtypeid  AND prevApp.applicationName =REPLACE(
        application.applicationName, 
        RIGHT(CONVERT(VARCHAR, application.CoverageYear), 2) + '/' + RIGHT(CONVERT(VARCHAR, application.CoverageYear + 1), 2),
        RIGHT(CONVERT(VARCHAR, application.CoverageYear - 1), 2) + '/' + RIGHT(CONVERT(VARCHAR, application.CoverageYear), 2)
    )            ) [prevApplication]

  where coverageYear=@coverageYear and (@status = 'all' or application.ApplicationStatus = @status)
and (@applicationType is null or applicationType.applicationtypeid = @applicationType)
    and (@filterquarter Is null OR (application.PeriodQuarter = @filterquarter or @filterquarter = 0 ) )
  and (@filtermonth Is null OR (application.PeriodMonth = @filtermonth or @filtermonth = 0 ) )
     and (member.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
   and (@lineofcoverage='all' or  member.MemberId in (select memberid from insuredlineofcoverage where insuredlineofcoverage.LineofCoverage =  @lineofcoverage))
                Order by ApplicationName      ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@coverageYear", coverageYear);
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@applicationType", applicationType.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@lineofCoverage", lineofCoverage);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.Parameters.Add(new SqlParameter("@filtermonth", System.Data.SqlDbType.NVarChar));
                cmd.Parameters.Add(new SqlParameter("@filterquarter", System.Data.SqlDbType.NVarChar));
                cmd.Parameters["@filtermonth"].Value = filterMonth.DBNullIfEmptyString(50);
                cmd.Parameters["@filterquarter"].Value = filterQuarter.DBNullIfEmptyString(50);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationPreview()
                    {
                        ApplicationId = reader["applicationId"].asInt(),
                        CoverageYear = reader["coverageYear"].asInt(),
                        ApplicationTypeId = reader["applicationTypeId"].asInt(),
                        MemberId = reader["memberId"].asInt(),
                        Member = reader["membername"].asString(),
                        ApplicationName = reader["applicationName"].asString(),
                        ApplicationStatus = reader["applicationStatus"].asString(),
                        ApplicationType = reader["ApplicationType"].asString(),
                        PeriodMonth = reader["periodMonth"].asNullableInt(),
                        PeriodQuarter = reader["periodquarter"].asNullableInt(),
                        DueDate = reader["dueDate"].asDate(),
                        CompletedDate = reader["completedDate"].asNullableDate(),
                        CurrentSectionId = reader["currentSectionid"].asNullableInt(),
                        PreviousApplicationId = reader["previousApplicationId"].asNullableInt()
                    };
                    applicationPreviews.Add(dataItem);
                }
            }
        }
        return applicationPreviews;
    }

    public permaAPI.models.applications.Application GetApplication(int applicationId, string userIdentifier)
    {
        permaAPI.models.applications.Application application = null;
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
  
  declare @isAdmin bit = 0;
select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
  SELECT   application.ApplicationId, application.ApplicationTypeId, application.MemberId, applicationName,
            coverageYear, applicationStatus, DueDate, completedDate, completedBy, insured.membername,
            applicationtype.applicationtype, prevApplication.ApplicationId [previousapplicationid],
            case applicationstatus when 'Submitted' then 1 else 0 end  as ReadOnly,
            firstSection.ApplicationSectionId [currentSectionId], periodmonth, periodquarter
            FROM application
            INNER JOIN INSURED on application.memberid = insured.memberid
            INNER JOIN ApplicationType on application.ApplicationTypeId = applicationtype.ApplicationTypeId
  outer apply (
                SELECT top 1 applicationsectionId 
FROM applicationSection 
 where  ApplicationSection.ShowInNavigation=1 and applicationsection.ApplicationSectionId in (
     select applicationsectionid from ApplicationSectionApplicationTypes where applicationtypeid = application.applicationtypeid
 )
        
 ORDER BY SortOrder  
) [firstSection]
             OUTER APPLY(
                SELECT applicationId from application [prevApp] where application.MemberId = prevApp.MemberId and prevapp.CoverageYear = application.CoverageYear -1
            ) [prevApplication]
where application.applicationId=@applicationId 

        and (INSURED.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
         ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@applicationId", applicationId);
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    application = new permaAPI.models.applications.Application()
                    {
                        ApplicationId = reader["applicationId"].asInt(),
                        CoverageYear = reader["coverageYear"].asInt(),
                        ApplicationTypeId = reader["applicationTypeId"].asInt(),
                        MemberId = reader["memberId"].asInt(),
                        ApplicationName = reader["applicationName"].asString(),
                        ApplicationStatus = reader["applicationStatus"].asString(),
                        DueDate = reader["dueDate"].asDate(),
                        Readonly = (reader["ReadOnly"].asInt() == 1) ? true : false,
                        CompletedDate = reader["completedDate"].asNullableDate(),
                        MemberName = reader["membername"].EmptyIfDBNullString(),
                        ApplicationType = reader["applicationType"].asString(),
                        CurrentSectionId = reader["currentSectionid"].asNullableInt(),
                        PreviousApplicationId = reader["previousapplicationId"].asNullableInt(),
                        PeriodMonth = reader["periodMonth"].asNullableInt(),
                        PeriodQuarter = reader["periodquarter"].asNullableInt(),
                    };

                }
            }
        }
        return application;
    }
    public string SaveApplication(permaAPI.models.applications.Application application, string userIdentifier)
    {
        string result = permaAPI.enums.SaveResult.Failure;


        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
declare @isAdmin bit = 0;
select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
update application
        SET applicationTypeID=@applicationTypeID,
    memberId=@memberId,
    applicationName=@applicationName,
    coverageYear=@coverageYear,
    applicationStatus=@applicationStatus,
    dueDate=@dueDate,
    completedDate=@completedDate,
    completedby=@completedBy,
    updatedby=@systemUser,
    updatedDate=getdate()
WHERE applicationid=@applicationId     and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
      
    ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@applicationId", System.Data.SqlDbType.Int));
                cmd.Parameters["@applicationId"].Value = application.ApplicationId;
                cmd.Parameters.AddWithValue("@applicationTypeId", application.ApplicationTypeId);
                cmd.Parameters.AddWithValue("@MemberId", application.MemberId);
                cmd.Parameters.AddWithValue("@ApplicationName", application.ApplicationName.EmptyIfDBNullString(250));
                cmd.Parameters.AddWithValue("@CoverageYear", application.CoverageYear);
                cmd.Parameters.AddWithValue("@ApplicationStatus", application.ApplicationStatus.EmptyIfDBNullString(50));
                cmd.Parameters.AddWithValue("@DueDate", application.DueDate.DBNullIfNullDate());
                cmd.Parameters.AddWithValue("@CompletedDate", application.CompletedDate.DBNullIfNullDate());
                cmd.Parameters.AddWithValue("@completedby", application.CompletedBy.EmptyIfDBNullString(50));
                cmd.Parameters.AddWithValue("@SystemUser", userIdentifier);
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                var reader = cmd.ExecuteReader();
                result = permaAPI.enums.SaveResult.Success;
            }
        }
        return result;
    }
    public permaAPI.models.PayrollExport ExportPayrollTableData(int coverageyear, string status, int? reportTypeId, string userIdentifier, string lineofCoverage, int? applicationType)
    {
        var results = new permaAPI.models.PayrollExport();
        var apps = new List<permaAPI.models.ApplicationPayroll>();

        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
 
 declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
select insured.MemberId, application.ApplicationId, insured.MemberName, application.ApplicationName, application.periodmonth, application.periodquarter
from Application inner join Insured on application.MemberId = insured.MemberId
    INNER JOIN applicationType on application.applicationTypeId = applicationType.applicationTypeId
where coverageyear = @coverageyear
 and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
    AND (@status='all' or Application.Applicationstatus=@status)
    AND (@ReportType is null or applicationtype.reporttype = @reportType)
and (@applicationType is null or application.applicationtypeid = @applicationType) 
  and (@lineofcoverage='all' or  insured.MemberId in (select memberid from insuredlineofcoverage where insuredlineofcoverage.LineofCoverage =  @lineofcoverage))
order by insured.MemberName

  
 

SELECT  ApplicationElementResponse.applicationid,ApplicationSectionElement.ElementId , ApplicationElementResponse.rowid, ApplicationElementResponse.CurrencyResponse, ApplicationElementResponse.TextResponse, ApplicationElementResponse.LongTextResponse, ApplicationElementResponse.IntResponse, ApplicationElementResponse.BitResponse, ApplicationElementResponse.DateResponse
FROM applicationSection
 INNER JOIN ApplicationSectionElement on ApplicationSectionElement.SectionId = applicationsection.ApplicationSectionId
 and isnull(ApplicationSectionElement.hideFromExport,0) = 0  
 INNER JOIN ApplicationElementResponse on ApplicationElementResponse.ElementId = ApplicationSectionElement.elementid
 INNER JOIN Application on application.ApplicationId = ApplicationElementResponse.ApplicationId and application.CoverageYear = @coverageyear   
   INNER JOIN applicationType on application.applicationTypeId = applicationType.applicationTypeId
WHERE applicationSectionId IN (
    SELECT  distinct ApplicationSectionID
    FROM ApplicationSectionApplicationTypes
    INNER JOIN Application ON Application.ApplicationTypeId = ApplicationSectionApplicationTypes.ApplicationTypeId
    and Application.ApplicationID  in (
        select   application.ApplicationId
         from Application  
          where coverageyear = @coverageyear   
  )  

)
AND ApplicationSection.ShowInNavigation=1 and ApplicationSectionElement.ElementType not in (9, 6)  and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
            AND (@status='all' or Application.Applicationstatus=@status)
            AND (@ReportType is null or applicationtype.reporttype = @reportType)
order by Application.ApplicationId, ApplicationSectionElement.SortOrder asc;


select tablesectionid, sourceValues from  applicationsectionelement  where tablesectionid in (37,36);


SELECT  ApplicationElementResponse.applicationid, ApplicationSectionElement.sectionid, ApplicationSectionElement.ElementId , ApplicationElementResponse.rowid, ApplicationElementResponse.CurrencyResponse, ApplicationElementResponse.TextResponse, ApplicationElementResponse.LongTextResponse, ApplicationElementResponse.IntResponse, ApplicationElementResponse.BitResponse, ApplicationElementResponse.DateResponse
FROM applicationSection
 INNER JOIN ApplicationSectionElement on ApplicationSectionElement.SectionId = applicationsection.ApplicationSectionId
 and isnull(ApplicationSectionElement.hideFromExport,0) = 0  
 INNER JOIN ApplicationElementResponse on ApplicationElementResponse.ElementId = ApplicationSectionElement.elementid
 INNER JOIN Application on application.ApplicationId = ApplicationElementResponse.ApplicationId and application.CoverageYear = @coverageyear   
   INNER JOIN applicationType on application.applicationTypeId = applicationType.applicationTypeId
WHERE applicationSectionId IN (
    SELECT  distinct ApplicationSectionID
    FROM ApplicationSectionApplicationTypes
    INNER JOIN Application ON Application.ApplicationTypeId = ApplicationSectionApplicationTypes.ApplicationTypeId
    and Application.ApplicationID  in (
        select   application.ApplicationId
         from Application  
          where coverageyear = @coverageyear )  

	)
AND ApplicationSection.ApplicationSectionId in (37,36) and ApplicationSectionElement.ElementType not in (9, 6) 
 and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
            AND (@status='all' or Application.Applicationstatus=@status)
            AND (@ReportType is null or applicationtype.reporttype = @reportType)
order by Application.ApplicationId,   ApplicationSectionElement.SortOrder asc;

 
    ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@coverageyear", System.Data.SqlDbType.Int));
                cmd.Parameters["@coverageYear"].Value = coverageyear;
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@reportType", reportTypeId.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@lineofCoverage", lineofCoverage);
                cmd.Parameters.AddWithValue("@applicationType", applicationType.DBNullIfNullInt());
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    permaAPI.models.ApplicationPayroll summary = new permaAPI.models.ApplicationPayroll();
                    summary.ApplicationId = reader["applicationId"].asInt();
                    summary.memberName = reader["memberName"].asString();
                    summary.periodMonth = reader["periodmonth"].asNullableInt();
                    summary.periodQuarter = reader["periodquarter"].asNullableInt();
                    summary.SafetyPayroll = new List<permaAPI.models.PayrollRow>();
                    summary.NonSafetyPayroll = new List<permaAPI.models.PayrollRow>();
                    apps.Add(summary);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    var applicationId = reader["applicationId"].asInt();
                    var application = (from a in apps where a.ApplicationId == applicationId select a).FirstOrDefault();
                    if (application != null)
                    {
                        var elementId = reader["elementid"].asInt();
                        switch (elementId)
                        {
                            case 834:
                                application.memberName = reader["textResponse"].asString();
                                break;
                            case 835:
                                application.CompletingName = reader["textResponse"].asString();
                                break;
                            case 836:
                                application.Email = reader["textResponse"].asString();
                                break;
                            case 837:
                                application.Payroll = reader["currencyResponse"].asDecimal();
                                break;
                            default:
                                break;
                        }

                    }

                }
                reader.NextResult();
                while (reader.Read())
                {
                    var tableSectionId = reader["tableSectionId"].asInt();
                    var sourceValues = reader["sourceValues"].asString();
                    var labels = sourceValues.Split("||");
                    int rowId = 0;
                    foreach (var label in labels)
                    {
                        if (label.Trim() != "")
                        {
                            var classInfo = label.Split("-");

                            var code = classInfo[0].Trim();
                            var classification = classInfo[1].Trim();

                            foreach (var app in apps)
                            {
                                if (tableSectionId == 36)
                                {
                                    app.SafetyPayroll.Add(new permaAPI.models.PayrollRow()
                                    {
                                        classification = classification,
                                        code = code,
                                        Row = rowId
                                    });

                                }
                                if (tableSectionId == 37)
                                {
                                    app.NonSafetyPayroll.Add(new permaAPI.models.PayrollRow()
                                    {
                                        classification = classification,
                                        code = code,
                                        Row = rowId
                                    });
                                }
                            }
                            rowId++;
                        }

                    }


                }
                reader.NextResult();
                while (reader.Read())
                {
                    int applicationId = reader["applicationId"].asInt();
                    int rowId = reader["rowid"].asInt();
                    int elementId = reader["elementId"].asInt();
                    var application = (from app in apps where app.ApplicationId == applicationId select app).FirstOrDefault();
                    int sectionId = reader["sectionid"].asInt();
                    if (application != null)
                    {

                        if (sectionId == 37)
                        {

                            var nonSafetyPayroll = (from p in application.NonSafetyPayroll where p.Row == rowId select p).FirstOrDefault();
                            if (nonSafetyPayroll != null)
                            {
                                switch (elementId)
                                {

                                    case 850:
                                        var response = reader["CurrencyResponse"].asNullableDecimal();
                                        var value = (response.HasValue) ? response.Value : 0;
                                        nonSafetyPayroll.payroll = value;
                                        break;
                                    case 851:
                                        var responseInt = reader["IntResponse"].asNullableInt();
                                        var valueInt = (responseInt.HasValue) ? responseInt.Value : 0;
                                        nonSafetyPayroll.numEmployees = valueInt;
                                        break;
                                    default:
                                        break;
                                }
                            }

                        }
                        if (sectionId == 36)
                        {
                            var SafetyPayroll = (from p in application.SafetyPayroll where p.Row == rowId select p).FirstOrDefault();
                            if (SafetyPayroll != null)
                            {
                                switch (elementId)
                                {
                                    case 841:
                                        var response = reader["CurrencyResponse"].asNullableDecimal();
                                        var value = (response.HasValue) ? response.Value : 0;

                                        SafetyPayroll.payroll = value;
                                        break;
                                    case 842:
                                        var responseInt = reader["IntResponse"].asNullableInt();
                                        var valueInt = (responseInt.HasValue) ? responseInt.Value : 0;

                                        SafetyPayroll.numEmployees = valueInt;
                                        break;

                                    default:
                                        break;
                                }
                            }

                        }


                    }
                }
            }
        }
        results.Applications = apps;
        return results;
    }




    public permaAPI.models.ExportedData ExportNonTableData(int coverageyear, string status, int? reportTypeId, string userIdentifier, int? filterMonth, int? filterQuarter, string lineofCoverage, int? applicationType)
    {
        var results = new permaAPI.models.ExportedData();
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
 
declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
select insured.MemberId, application.ApplicationId, insured.MemberName, application.ApplicationName, application.periodmonth, application.periodquarter
from Application inner join Insured on application.MemberId = insured.MemberId
    INNER JOIN applicationType on application.applicationTypeId = applicationType.applicationTypeId
where coverageyear = @coverageyear
 and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
    AND (@status='all' or Application.Applicationstatus=@status)
and (@filterQuarter is null or application.periodQuarter = @filterquarter)
and (@filtermonth is null or application.periodMonth = @filtermonth)
and (@applicationType is null or application.applicationtypeid = @applicationType) 
    AND (@ReportType is null or applicationtype.reporttype = @reportType)
  and (@lineofcoverage='all' or  insured.MemberId in (select memberid from insuredlineofcoverage where insuredlineofcoverage.LineofCoverage =  @lineofcoverage))
order by insured.MemberName

 
SELECT ApplicationSectionElement.ElementId, 
   case when ApplicationSectionElement.ShortName = '' then ApplicationSectionElement.Label else 
    ApplicationSectionElement.ShortName  end  columnName,
   ApplicationSectionElement.elementtype, ApplicationSectionElement.originalSource,
   exportcategory.exportcategory
FROM applicationSection
 INNER JOIN ApplicationSectionElement on ApplicationSectionElement.SectionId = applicationsection.ApplicationSectionId   and isnull(ApplicationSectionElement.hideFromExport,0) = 0  
 LEFT JOIN exportcategory on ApplicationSectionElement.exportcategoryid = exportcategory.id
WHERE applicationSectionId IN (
    SELECT  distinct ApplicationSectionID
    FROM ApplicationSectionApplicationTypes
    INNER JOIN Application ON Application.ApplicationTypeId = ApplicationSectionApplicationTypes.ApplicationTypeId
  INNER JOIN applicationType on application.applicationTypeId = applicationType.applicationTypeId
    and Application.ApplicationID  in (
        select   application.ApplicationId
         from Application  
          where coverageyear = @coverageyear   and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
            AND (@status='all' or Application.Applicationstatus=@status)
and (@filterQuarter is null or application.periodQuarter = @filterquarter)
and (@filtermonth is null or application.periodMonth = @filtermonth)
            AND (@ReportType is null or applicationtype.reporttype = @reportType)
  )  
)
AND ApplicationSection.ShowInNavigation=1 and ApplicationSectionElement.ElementType not in (9, 6)

  order by ExportCategory.sortindex,  applicationSection.SortOrder,  ApplicationSectionElement.SortOrder   
 

SELECT  ApplicationElementResponse.applicationid,ApplicationSectionElement.ElementId , ApplicationElementResponse.rowid, ApplicationElementResponse.CurrencyResponse, ApplicationElementResponse.TextResponse, ApplicationElementResponse.LongTextResponse, ApplicationElementResponse.IntResponse, ApplicationElementResponse.BitResponse, ApplicationElementResponse.DateResponse
FROM applicationSection
 INNER JOIN ApplicationSectionElement on ApplicationSectionElement.SectionId = applicationsection.ApplicationSectionId
 and isnull(ApplicationSectionElement.hideFromExport,0) = 0  
 INNER JOIN ApplicationElementResponse on ApplicationElementResponse.ElementId = ApplicationSectionElement.elementid
 INNER JOIN Application on application.ApplicationId = ApplicationElementResponse.ApplicationId and application.CoverageYear = @coverageyear   
   INNER JOIN applicationType on application.applicationTypeId = applicationType.applicationTypeId
WHERE applicationSectionId IN (
    SELECT  distinct ApplicationSectionID
    FROM ApplicationSectionApplicationTypes
    INNER JOIN Application ON Application.ApplicationTypeId = ApplicationSectionApplicationTypes.ApplicationTypeId
    and Application.ApplicationID  in (
        select   application.ApplicationId
         from Application  
          where coverageyear = @coverageyear   
  )  

)
AND ApplicationSection.ShowInNavigation=1 and ApplicationSectionElement.ElementType not in (9, 6)  and (application.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
            AND (@status='all' or Application.Applicationstatus=@status)
and (@filterQuarter is null or application.periodQuarter = @filterquarter)
and (@filtermonth is null or application.periodMonth = @filtermonth)
            AND (@ReportType is null or applicationtype.reporttype = @reportType)
order by Application.ApplicationId,   ApplicationSectionElement.SortOrder   
  
    ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@coverageyear", System.Data.SqlDbType.Int));
                cmd.Parameters["@coverageYear"].Value = coverageyear;
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@reportType", reportTypeId.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@filterQuarter", filterQuarter.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@filterMonth", filterMonth.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@lineofCoverage", lineofCoverage);
                cmd.Parameters.AddWithValue("@applicationType", applicationType.DBNullIfNullInt());
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    permaAPI.models.MemberSummary summary = new permaAPI.models.MemberSummary();
                    summary.MemberId = reader["memberid"].asInt();
                    summary.ApplicationId = reader["applicationid"].asInt();
                    summary.MemberName = reader["membername"].EmptyIfDBNullString();
                    summary.ApplicationName = reader["applicationname"].EmptyIfDBNullString();
                    summary.PeriodMonth = reader["periodmonth"].NullifDBNullInt();
                    summary.PeriodQuarter = reader["periodquarter"].NullifDBNullInt();

                    results.MemberSummaries.Add(summary);

                }
                reader.NextResult();
                while (reader.Read())
                {
                    permaAPI.models.ApplicationQuestion question = new permaAPI.models.ApplicationQuestion();
                    question.ElementId = reader["elementid"].asInt();
                    question.ColumnName = reader["columnname"].asString();
                    question.ElementType = (permaAPI.enums.ElementType)reader["elementtype"].asInt();
                    question.OriginalSource = reader["originalsource"].EmptyIfDBNullString();
                    question.ExportCategory = reader["exportCategory"].EmptyIfDBNullString();
                    results.ApplicationQuestions.Add(question);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    permaAPI.models.ResponseSummary response = new permaAPI.models.ResponseSummary();
                    response.ElementId = reader["elementid"].asInt();
                    response.RowId = reader["rowid"].asInt();
                    response.CurrencyResponse = reader["currencyresponse"].asNullableDecimal();
                    response.TextResponse = reader["textresponse"].EmptyIfDBNullString();
                    response.LongTextResponse = reader["longtextresponse"].EmptyIfDBNullString();
                    response.IntResponse = reader["intresponse"].asNullableInt();
                    response.BitResponse = reader["bitresponse"].asNullableBool();
                    response.DateResponse = reader["dateresponse"].asNullableDate();
                    response.ApplicationId = reader["applicationId"].asInt();
                    results.ApplicationResponses.Add(response);


                }
            }
        }
        return results;
    }



}