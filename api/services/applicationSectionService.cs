using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;
using permaAPI.models.applications;

namespace permaAPI.services
{
    public interface IApplicationSectionService
    {
        public IList<permaAPI.models.applications.ApplicationSection> GetApplicationSections(int applicationId);
        public Boolean CompleteSection(int applicationId, int sectionId, Boolean isCompleted);
    }
    public class ApplicationSectionService : IApplicationSectionService
    {
        public IConfiguration Configuration { get; }
        public ApplicationSectionService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Boolean CompleteSection(int applicationId, int sectionId, Boolean isCompleted)
        {
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
if not exists (select * from applicationsectionresponse where applicationId=@applicationid and sectionId=@sectionid)
BEGIN
    insert into ApplicationSectionResponse (applicationId, sectionId, isCompleted)
        select @applicationId, @sectionId, @isCompleted
END
ELSE
BEGIN
    update applicationsectionresponse 
        set iscompleted = @iscompleted where applicationId=@applicationid and sectionId=@sectionid
end
     ";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);
                    cmd.Parameters.AddWithValue("@sectionId", sectionId);
                    cmd.Parameters.AddWithValue("@isCompleted", isCompleted);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            return true;
        }
        public IList<ApplicationSection> GetApplicationSections(int applicationId)
        {
            var applicationSections = new List<permaAPI.models.applications.ApplicationSection>();
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
 
SELECT applicationsectionId, SectionTitle, Description, SubTitle, isnull( ApplicationSectionResponse.isCompleted,0) [isCompleted]
FROM applicationSection
    LEFT JOIN applicationSectionResponse
         on applicationSection.ApplicationSectionId = ApplicationSectionResponse.sectionId
         and ApplicationSectionResponse.applicationId = @applicationid
WHERE applicationSectionId IN (
    SELECT ApplicationSectionID
    FROM ApplicationSectionApplicationTypes
    INNER JOIN Application ON Application.ApplicationTypeId = ApplicationSectionApplicationTypes.ApplicationTypeId
    and Application.ApplicationID=@ApplicationID    
)   AND ApplicationSectionID IN (
    Select ApplicationSectionID
    FROM Application INNER JOIN InsuredLineofCoverage on Application.applicationId=@applicationID
        and InsuredLineofCoverage.memberid = application.memberid
        INNER JOIN ApplicationSectionLinesofCoverage on ApplicationSectionLinesofCoverage.lineofcoverage = insuredlineofcoverage.lineofcoverage

)
AND ApplicationSection.ShowInNavigation=1
ORDER BY SortOrder      ";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);

                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var dataItem = new permaAPI.models.applications.ApplicationSection()
                        {
                            ApplicationSectionId = reader["applicationSectionId"].asInt(),
                            SectionTitle = reader["SectionTitle"].asString(),
                            Description = reader["Description"].asString(),
                            SubTitle = reader["SubTitle"].EmptyIfDBNullString(),
                            isCompleted = reader["isCompleted"].asBoolean()

                        };
                        applicationSections.Add(dataItem);
                    }
                }
            }
            return applicationSections;
        }
    }
}

