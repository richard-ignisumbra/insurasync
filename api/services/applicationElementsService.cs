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
    public interface IApplicationElementsService
    {
        public IList<permaAPI.models.applications.ApplicationElement> GetApplicationElements(int sectionId, int applicationId);
        public permaAPI.models.applications.ApplicationElement GetApplicationElement(int elementId, int applicationId, int rowId);
        public bool SaveApplicationResponse(models.applications.ApplicationElementResponse response, int applicationId, string userIdentifier);
        public IList<models.applications.ApplicationElementResponse> GetResponses(int sectionId, int applicationId);
        public models.applications.ApplicationElementResponse GetResponse(int elementId, int applicationId, int rowId);
    }
    public class ApplicationElementsService : IApplicationElementsService
    {
        public IConfiguration Configuration { get; }
        public ApplicationElementsService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IList<ApplicationElement> GetApplicationElements(int sectionId, int applicationId)
        {
            var applicationElements = new List<permaAPI.models.applications.ApplicationElement>();
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
  declare @memberId int;
Declare @isReadOnly bit =0;
select @memberId = memberid, @isReadOnly= case applicationstatus when 'Submitted' then 1 else 0 end   from application where ApplicationId = @applicationid;
 select 
ApplicationSectionElement.[ElementId],[LongText],[ShortName],[ElementType],[TableSectionId],[DecimalPrecision],[isRequired],ISNULL( ApplicationElementLabel.label, applicationSectionElement.label) [label],
ApplicationSectionElement.sourceLabels, ApplicationSectionElement.sourceValues, ApplicationSectionElement.MaxLength,
ApplicationSectionElement.allowNewRows, applicationSectionElement.width, applicationSectionElement.indentSpaces, @isReadOnly [isReadonly],
ApplicationSectionElement.sumValues
 from ApplicationSectionElement
    LEFT JOIN ApplicationElementLabel on ApplicationElementLabel.ElementId = ApplicationSectionElement.ElementId and ApplicationElementLabel.ApplicationId=@applicationId 
WHERE sectionID=@sectionId and (showAllLines= 1
 or ApplicationSectionElement.ElementId in
  ( select ApplicationElementLine.elementId from InsuredLineofCoverage 
    INNER JOIN ApplicationElementLine
     on InsuredLineofCoverage.LineofCoverage = ApplicationElementLine.lineId
    where InsuredLineofCoverage.MemberId  =@memberId)
 
 )
ORDER BY ApplicationSectionElement.SortOrder asc;
 
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);
                    cmd.Parameters.AddWithValue("@sectionId", sectionId);

                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var dataItem = new permaAPI.models.applications.ApplicationElement()
                        {
                            ElementId = reader["Elementid"].asInt(),
                            LongText = reader["longText"].asString(),
                            ShortName = reader["shortName"].asString(),
                            Readonly = reader["isReadonly"].asBoolean(),
                            ElementType = (enums.ElementType)reader["ElementType"].asInt(),
                            TableSectionId = reader["TableSectionId"].asNullableInt(),
                            MaxLength = reader["MaxLength"].asNullableInt(),
                            DecimalPrecision = reader["DecimalPrecision"].asNullableInt(),
                            IndentSpaces = reader["indentSpaces"].asNullableInt(),
                            isRequired = reader["isRequired"].asBoolean(),
                            AllowNewRows = reader["allowNewRows"].asBoolean(),
                            Label = reader["label"].asString(),
                            Width = reader["width"].asNullableInt(),
                            SumValues = reader["sumValues"].asBoolean()
                        };
                        var sourceValues = reader["sourceValues"].EmptyIfDBNullString();
                        var sourceLabels = reader["sourceLabels"].EmptyIfDBNullString();
                        if (!string.IsNullOrEmpty(sourceValues))
                        {
                            var Options = new List<models.ListValue>();
                            var splitValues = sourceValues.Split("||");

                            var splitLabels = sourceLabels.Split("||");
                            for (int i = 0; i < splitValues.Length; i++)
                            {
                                var fieldOption = new models.ListValue();
                                fieldOption.Label = splitValues[i];
                                if (splitLabels.Length > i && !String.IsNullOrEmpty(splitLabels[0]))
                                {
                                    fieldOption.Label = splitLabels[i];
                                }
                                fieldOption.Value = splitValues[i];
                                Options.Add(fieldOption);
                            }
                            dataItem.SelectOptions = Options;
                        }
                        applicationElements.Add(dataItem);
                    }
                }
            }
            return applicationElements;
        }



        public ApplicationElement GetApplicationElement(int elementId, int applicationId, int rowId)
        {
            permaAPI.models.applications.ApplicationElement applicationElement = null;
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
 select 
ApplicationSectionElement.[ElementId],[LongText],[ShortName],[ElementType],[TableSectionId],[DecimalPrecision],[isRequired],ISNULL( ApplicationElementLabel.label, applicationSectionElement.label) [label],
ApplicationSectionElement.sourceLabels, ApplicationSectionElement.sourceValues, ApplicationSectionElement.MaxLength,
ApplicationSectionElement.allowNewRows, applicationSectionElement.width,  applicationSectionElement.indentSpaces, applicationSectionElement.sumValues
 from ApplicationSectionElement
    LEFT JOIN ApplicationElementLabel on ApplicationElementLabel.ElementId = ApplicationSectionElement.ElementId and ApplicationElementLabel.ApplicationId=@applicationId 

WHERE elementId= @elementId and rowId=@rowId and applicationId=@applicationId
ORDER BY ApplicationSectionElement.SortOrder asc;
 
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);
                    cmd.Parameters.AddWithValue("@elementId", elementId);
                    cmd.Parameters.AddWithValue("@rowId", rowId);

                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        applicationElement = new permaAPI.models.applications.ApplicationElement()
                        {
                            ElementId = reader["Elementid"].asInt(),
                            LongText = reader["longText"].asString(),
                            ShortName = reader["shortName"].asString(),
                            ElementType = (enums.ElementType)reader["ElementType"].asInt(),
                            TableSectionId = reader["TableSectionId"].asNullableInt(),
                            MaxLength = reader["MaxLength"].asNullableInt(),
                            IndentSpaces = reader["indentSpaces"].asNullableInt(),
                            DecimalPrecision = reader["DecimalPrecision"].asNullableInt(),
                            isRequired = reader["isRequired"].asBoolean(),
                            AllowNewRows = reader["allowNewRows"].asBoolean(),
                            Label = reader["label"].asString(),
                            Width = reader["width"].asNullableInt(),
                            SumValues = reader["sumValues"].asBoolean()
                        };
                        var sourceValues = reader["sourceValues"].EmptyIfDBNullString();
                        var sourceLabels = reader["sourceLabels"].EmptyIfDBNullString();
                        if (!string.IsNullOrEmpty(sourceValues))
                        {
                            var Options = new List<models.ListValue>();
                            var splitValues = sourceValues.Split("||");

                            var splitLabels = sourceLabels.Split("||");
                            for (int i = 0; i < splitValues.Length; i++)
                            {
                                var fieldOption = new models.ListValue();
                                fieldOption.Label = splitValues[i];
                                if (splitLabels.Length > i && !String.IsNullOrEmpty(splitLabels[0]))
                                {
                                    fieldOption.Label = splitLabels[i];
                                }
                                fieldOption.Value = splitValues[i];
                                Options.Add(fieldOption);
                            }
                            applicationElement.SelectOptions = Options;
                        }

                    }
                }
            }
            return applicationElement;
        }

        public IList<models.applications.ApplicationElementResponse> GetResponses(int sectionId, int applicationId)
        {
            var applicationResponses = new List<permaAPI.models.applications.ApplicationElementResponse>();
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
 

SELECT 
[ApplicationId],[ElementId],[RowId],[CurrencyResponse],[TextResponse],[LongTextResponse],[IntResponse],[BitResponse],[DateResponse],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate]
FROM ApplicationElementResponse 
WHERE ApplicationId=@applicationId  and ElementId in (select  ElementId from ApplicationSectionElement where SectionId=@sectionid)
ORDER BY ElementId, RowId;
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);
                    cmd.Parameters.AddWithValue("@sectionId", sectionId);

                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var item = new models.applications.ApplicationElementResponse()
                        {
                            ElementId = reader["elementId"].asInt(),
                            RowId = reader["rowid"].asInt(),
                            CurrencyResponse = reader["currencyResponse"].asNullableDecimal(),
                            ApplicationId = applicationId,
                            BitResponse = reader["bitresponse"].asNullableBool(),
                            DateResponse = reader["dateresponse"].asNullableDate(),
                            IntResponse = reader["intresponse"].asNullableInt(),
                            LongTextResponse = reader["longtextresponse"].EmptyIfDBNullString(),
                            TextResponse = reader["textresponse"].EmptyIfDBNullString()
                        };
                        applicationResponses.Add(item);
                    }
                }
            }
            return applicationResponses;
        }


        public models.applications.ApplicationElementResponse GetResponse(int elementId, int applicationId, int rowId)
        {
            models.applications.ApplicationElementResponse response = null;
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
 

SELECT 
[ApplicationId],[ElementId],[RowId],[CurrencyResponse],[TextResponse],[LongTextResponse],[IntResponse],[BitResponse],[DateResponse],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate]
FROM ApplicationElementResponse 
WHERE ApplicationId=@applicationId   and rowId = @rowId and elementId=@elementId
ORDER BY ElementId, RowId;
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);
                    cmd.Parameters.AddWithValue("@rowId", rowId);
                    cmd.Parameters.AddWithValue("@elementId", elementId);
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        response = new models.applications.ApplicationElementResponse()
                        {
                            ElementId = reader["elementId"].asInt(),
                            RowId = reader["rowid"].asInt(),
                            CurrencyResponse = reader["currencyResponse"].asNullableDecimal(),
                            ApplicationId = applicationId,
                            BitResponse = reader["bitresponse"].asNullableBool(),
                            DateResponse = reader["dateresponse"].asNullableDate(),
                            IntResponse = reader["intresponse"].asNullableInt(),
                            LongTextResponse = reader["longtextresponse"].EmptyIfDBNullString(),
                            TextResponse = reader["textresponse"].EmptyIfDBNullString()
                        };

                    }
                }
            }
            return response;
        }

        public bool SaveApplicationResponse(models.applications.ApplicationElementResponse response, int applicationId, string userIdentifier)
        {
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
    if not exists (SELECT * FROM ApplicationElementResponse WHERE applicationID=@ApplicationID and elementID= @elementID and    RowID=@rowID)
BEGIN
    INSERT INTO ApplicationElementResponse(
    applicationID, ElementID, RowID, CurrencyResponse, TextResponse, LongTextResponse, IntResponse, BitResponse, DateResponse,
CreatedBy, CreatedDate,UpdatedBy, UpdatedDate )
    VALUES (
 @applicationID, @ElementID, @RowID, @CurrencyResponse, @TextResponse, @LongTextResponse, @IntResponse, @BitResponse, @DateResponse,
@UserIdentity, getdate(),@userIdentity, getdate())
END
ELSE
BEGIN
    UPDATE ApplicationElementResponse
       SET UpdatedBy=@UserIdentity, updateddate=getdate(),
    currencyResponse=@currencyResponse, textresponse=@textresponse, longtextresponse=@longtextresponse,
    intresponse=@intresponse, bitresponse=@bitresponse, dateresponse=@dateresponse
    WHERE applicationID=@ApplicationID and elementID= @elementID and    RowID=@rowID
END
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@applicationId", applicationId);

                    cmd.Parameters.AddWithValue("@elementID", response.ElementId);
                    cmd.Parameters.AddWithValue("@rowID", response.RowId);
                    cmd.Parameters.AddWithValue("@currencyResponse", response.CurrencyResponse.DBNullIfNullDecimal());
                    cmd.Parameters.AddWithValue("@textresponse", response.TextResponse.DBNullIfEmptyString(500));
                    cmd.Parameters.AddWithValue("@longtextresponse", response.LongTextResponse.DBNullIfEmptyString(2500));
                    cmd.Parameters.AddWithValue("@intresponse", response.IntResponse.DBNullIfNullInt());
                    cmd.Parameters.AddWithValue("@bitresponse", response.BitResponse.DBNullIfNullBoolean());
                    cmd.Parameters.AddWithValue("@dateresponse", response.DateResponse.DBNullIfNullDate());
                    cmd.Parameters.AddWithValue("@useridentity", userIdentifier);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

            }


            return true;
        }
    }
}


