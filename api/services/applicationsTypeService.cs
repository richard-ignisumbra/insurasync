using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;

public interface IApplicationTypeService
{
    public IList<permaAPI.models.applications.ApplicationType> GetApplicationTypes();
    public IList<permaAPI.models.applications.ApplicationReportType> GetApplicationReportTypes();
    public IList<permaAPI.models.applications.ApplicationReport> GetApplicationReports(int reportTypeId);
    public permaAPI.models.applications.ApplicationReport GetApplicationReport(int reportId);
    public permaAPI.models.applications.ApplicationReport GetApplicationDefaultReport(int reportTypeId);
}
public class ApplicationTypeService : IApplicationTypeService
{
    public IConfiguration Configuration { get; }
    public ApplicationTypeService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IList<permaAPI.models.applications.ApplicationReport> GetApplicationReports(int reportTypeId)
    {
        var applicationReports = new List<permaAPI.models.applications.ApplicationReport>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");


        string query = @"
 SELECT TOP 1000 reportid, reportName, reportDescription, reportTypeId, reportAction FROM dbo.ApplicationReport WHERE reporttypeid=@reportTypeId order by reportName;          ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("reportTypeId", reportTypeId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationReport()
                    {
                        ReportAction = reader["reportaction"].asString(),
                        ReportDescription = reader["reportDescription"].asString(),
                        ReportId = reader["reportId"].asInt(),
                        ReportName = reader["reportName"].asString()
                    };
                    applicationReports.Add(dataItem);
                }

            }
        }

        return applicationReports;
    }

    public permaAPI.models.applications.ApplicationReport GetApplicationReport(int reportId)
    {
        var applicationReports = new List<permaAPI.models.applications.ApplicationReport>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");


        string query = @"
 SELECT TOP 1000 reportid, reportName, reportDescription, reportTypeId, reportAction FROM dbo.ApplicationReport WHERE reportId=@reportId order by reportName;          ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("reportId", reportId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationReport()
                    {
                        ReportAction = reader["reportaction"].asString(),
                        ReportDescription = reader["reportDescription"].asString(),
                        ReportId = reader["reportId"].asInt(),
                        ReportName = reader["reportName"].asString(),
                        reportTypeId = reader["reportTypeId"].asInt()

                    };
                    applicationReports.Add(dataItem);
                }

            }
        }

        return applicationReports[0];
    }
    public permaAPI.models.applications.ApplicationReport GetApplicationDefaultReport(int reportTypeId)
    {
        var applicationReports = new List<permaAPI.models.applications.ApplicationReport>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");


        string query = @"
 SELECT TOP 1 reportid, reportName, reportDescription, reportTypeId, reportAction FROM dbo.ApplicationReport WHERE reporttypeid=@reportTypeId order by reportName;          ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("reportTypeId", reportTypeId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationReport()
                    {
                        ReportAction = reader["reportaction"].asString(),
                        ReportDescription = reader["reportDescription"].asString(),
                        ReportId = reader["reportId"].asInt(),
                        ReportName = reader["reportName"].asString(),
                        reportTypeId = reader["reportTypeId"].asInt()

                    };
                    applicationReports.Add(dataItem);
                }

            }
        }

        return applicationReports[0];
    }
    public IList<permaAPI.models.applications.ApplicationType> GetApplicationTypes()
    {
        var applicationTypes = new List<permaAPI.models.applications.ApplicationType>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
SELECT TOP 1000 ApplicationTypeId, ApplicationType, Description, NotifyEmails, adminPermissionId, ReportType FROM dbo.ApplicationType order by ApplicationType;
          ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {


                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationType()
                    {
                        ApplicationTypeId = reader["applicationTypeId"].asInt(),
                        Type = reader["ApplicationType"].asString(),
                        AdminPermissionId = reader["AdminPermissionId"].asInt(),
                        NotifyEmails = reader["NotifyEmails"].asString(),
                        ReportType = reader["ReportType"].asNullableInt()
                    };
                    applicationTypes.Add(dataItem);
                }
            }
        }
        return applicationTypes;
    }


    public IList<permaAPI.models.applications.ApplicationReportType> GetApplicationReportTypes()
    {
        var applicationReportTypes = new List<permaAPI.models.applications.ApplicationReportType>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
select reportType, Name, Description from ReportType ORDER BY Name          ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {


                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dataItem = new permaAPI.models.applications.ApplicationReportType()
                    {
                        Description = reader["description"].asString(),
                        Name = reader["name"].asString(),
                        ReportType = reader["reportType"].asInt()
                    };
                    applicationReportTypes.Add(dataItem);
                }
            }
        }
        return applicationReportTypes;
    }

}