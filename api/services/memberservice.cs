using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;
using Microsoft.Graph;

public interface IMembersService
{
    int AddMember(Member member, string systemUser);
    public Boolean UpdateMemberDetails(MemberDetails memberDetails, string systemUser);
    IEnumerable<Member> GetAllMembers(string userIdentifier);
    MemberDetails GetMemberDetails(int memberId, string userIdentifier);

    IEnumerable<permaAPI.models.members.noteSummary> GetAllNotes(int memberId, string status);
    permaAPI.models.members.note GetNote(int memberId, int noteId);

    int SaveNote(int memberId, permaAPI.models.members.note note, string systemUser);

    IEnumerable<permaAPI.models.members.Attachment> GetMemberAttachments(int memberId, string status);
    permaAPI.models.members.Attachment GetMemberAttachment(int attachmentId, string systemUser);
    int SaveMemberAttachment(int memberId, permaAPI.models.members.Attachment attachment, string systemUser);


}
public class MembersService : IMembersService
{
    public IConfiguration Configuration { get; }
    public MembersService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    private IList<Member> _members;
    public MembersService()
    {
        _members = new List<Member>();
    }
    public int AddMember(Member member, string userIdentifier)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        string query = @"
            INSERT INTO Insured (memberName, memberNumber, memberStatus,
             organizationType, primaryContactId, parentMember, createdby, createddate, updatedby, updateddate) 
            VALUES (@memberName, @memberNumber, @memberStatus,
             @organizationType,@primaryContact  ,@parentMember, @systemuser, getdate(), @systemuser, getdate()); select @memberId=  scope_identity();
        ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@memberId", 0);
                cmd.Parameters["@memberId"].Direction = System.Data.ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@memberName", member.MemberName);
                cmd.Parameters.AddWithValue("@memberNumber", member.MemberNumber);
                cmd.Parameters.AddWithValue("@memberstatus", member.MemberStatus);
                cmd.Parameters.AddWithValue("@organizationType", member.OrganizationType);
                cmd.Parameters.AddWithValue("@systemUser", userIdentifier);
                int? primaryContactId = null;
                if (member.PrimaryContact != null)
                {
                    primaryContactId = member.PrimaryContact.ContactID;
                }
                cmd.Parameters.AddWithValue("@primaryContact", primaryContactId.DBNullIfNullInt());
                int? parentMemberId = null;
                if (member.ParentMember != null)
                {
                    parentMemberId = member.ParentMember.MemberId;
                }
                cmd.Parameters.AddWithValue("@parentMember", parentMemberId.DBNullIfNullInt());
                cmd.ExecuteNonQuery();
                member.MemberId = (int)cmd.Parameters["@memberId"].Value;
            }
        }
        return member.MemberId;
    }
    public IEnumerable<Member> GetAllMembers(string userIdentifier)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        _members = new List<Member>();
        string query = @"

 declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;

 Select insured.MemberId, isnull(insured.MemberNumber, '') MemberNumber, isnull(insured.MemberName,'') MemberName, 
        isnull(insured.MemberStatus, 'inactive') MemberStatus, isnull(insured.OrganizationType, 'Unknown') OrganizationType,
         Contact.ContactId, Contact.FirstName, Contact.LastName, Contact.Email,
         parentMember.memberID [parentMemberID], parentMember.memberName [parentMemberName]
         from insured 
        LEFT JOIN Contact on Insured.PrimaryContactId = Contact.ContactId
        LEFT JOIN insured [parentMember] on insured.parentMember = parentMember.memberId
WHERE (insured.isDeleted != 1 or insured.isDeleted is null) and (insured.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
 Order by MemberName";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {

                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    var member = new Member()
                    {
                        MemberId = reader.GetInt32(0),
                        MemberNumber = reader.GetString(1),
                        MemberName = reader.GetString(2),
                        MemberStatus = reader.GetString(3),
                        OrganizationType = reader.GetString(4)

                    };
                    int? parentMemberId = reader["parentMemberId"].NullifDBNullInt();
                    if (parentMemberId.HasValue)
                    {
                        member.ParentMember = new Member()
                        {
                            MemberId = parentMemberId.Value,
                            MemberName = reader["parentMemberName"].EmptyIfDBNullString()
                        };
                    }
                    var contactID = reader.GetValue(5);
                    if (contactID != System.DBNull.Value)
                    {
                        member.PrimaryContact = new Contact()
                        {
                            ContactID = reader.GetInt32(5),
                            FirstName = reader.GetString(6),
                            LastName = reader.GetString(7),
                            Email = reader.GetString(8)
                        };
                    }

                    _members.Add(member);
                }
            }
        }
        return _members;
    }

    public Boolean UpdateMemberDetails(MemberDetails memberDetails, string userIdentifier)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"  declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
 

 update insured 
        set MemberNumber=@memberNumber, memberName=@memberName, memberStatus=@memberStatus, organizationType=@organizationType, 
        companyName=@companyName, address1=@address1, address2=@address2, city=@city, state=@state,
        zip=@zip, phone=@phone, website=@website, parentMember=@parentMember, startDate=@startDate,
        stateofIncorporation=@stateofIncorporation, FEIN=@fein, primaryContactId=@primaryContactID, updatedBy=@systemUser,
        updatedDate=getdate(), notes=@notes, joinDate=@joinDate 
        WHERE memberId=@memberId and (insured.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 ) ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberDetails.MemberId);
                cmd.Parameters.AddWithValue("@memberNumber", memberDetails.MemberNumber.EmptyIfNullString(50));
                cmd.Parameters.AddWithValue("@memberName", memberDetails.MemberName.EmptyIfNullString(250));
                cmd.Parameters.AddWithValue("@memberStatus", memberDetails.MemberStatus.EmptyIfNullString(50));
                cmd.Parameters.AddWithValue("@organizationType", memberDetails.OrganizationType.EmptyIfNullString(100));
                cmd.Parameters.AddWithValue("@companyName", memberDetails.CompanyName.EmptyIfNullString(200));
                cmd.Parameters.AddWithValue("@address1", memberDetails.Address1.EmptyIfNullString(300));
                cmd.Parameters.AddWithValue("@address2", memberDetails.Address2.DBNullIfEmptyString(300));
                cmd.Parameters.AddWithValue("@city", memberDetails.City.EmptyIfNullString(200));
                cmd.Parameters.AddWithValue("@state", memberDetails.State.EmptyIfNullString(50));
                cmd.Parameters.AddWithValue("@zip", memberDetails.Zip.EmptyIfNullString(50));
                cmd.Parameters.AddWithValue("@phone", memberDetails.Phone.EmptyIfNullString(50));
                cmd.Parameters.AddWithValue("@website", memberDetails.Website.EmptyIfNullString(250));
                int? parentMemberId = null;
                if (memberDetails.ParentMember != null)
                {
                    parentMemberId = memberDetails.ParentMember.MemberId;
                }
                cmd.Parameters.AddWithValue("@parentmember", parentMemberId.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@startDate", memberDetails.StartDate.DBNullIfNullDate());
                cmd.Parameters.AddWithValue("@StateofIncorporation", memberDetails.StateofIncorporation);
                cmd.Parameters.AddWithValue("@FEIN", memberDetails.FEIN);
                int? primaryContactId = null;
                if (memberDetails.PrimaryContact != null)
                {
                    primaryContactId = memberDetails.PrimaryContact.ContactID;
                }
                cmd.Parameters.AddWithValue("@PrimaryContactID", primaryContactId.DBNullIfNullInt());
                cmd.Parameters.AddWithValue("@systemUser", userIdentifier);

                cmd.Parameters.AddWithValue("@Notes", memberDetails.Notes);
                cmd.Parameters.AddWithValue("@JoinDate", memberDetails.JoinDate.DBNullIfNullDate());
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                cmd.ExecuteNonQuery();
                this.UpdateLinesofCoverage(memberDetails.LinesofCoverage, memberDetails.MemberId);
            }
        }
        return true;
    }

    public void UpdateLinesofCoverage(List<String> linesofCoverage, int memberId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
delete from insuredLineofCoverage where memberid=@memberID; ";
        for (int i = 0; i < linesofCoverage.Count; i++)
        {
            query += $"INSERT INTO insuredLineofCoverage (LineofCoverage, MemberId) Values(@lineofCoverage${i}, @memberId)";
        }
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberId);
                for (int i = 0; i < linesofCoverage.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@lineofCoverage${i}", linesofCoverage[i]);
                }
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    public IList<String> GetMemberLinesofCoverage(int memberId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var LinesofCoverage = new List<String>();
        string query = @"
    SELECT LineofCoverage from InsuredLineofCoverage WHERE memberId=@memberId
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("found a line of coverage somehow");
                    LinesofCoverage.Add(reader["LineofCoverage"].EmptyIfDBNullString());
                }
            }
        }
        return LinesofCoverage;

    }
    public MemberDetails GetMemberDetails(int memberId, string userIdentifier)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        MemberDetails result = null;
        string query = @"
          declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;

  Select 
        MemberId, isnull(MemberNumber, '') MemberNumber, isnull(MemberName,'') MemberName,
         isnull(MemberStatus, 'inactive') MemberStatus, 
        isnull(OrganizationType, 'Unknown') OrganizationType, Contact.ContactId, Contact.FirstName, 
        Contact.LastName, Contact.Email , CompanyName, insured.Address1, 
        insured.Address2, insured.City, insured.State, insured.Zip, insured.Phone,
        Website, ParentMember, StartDate, StateOfIncorporation, FEIN, Notes, JoinDate
        from insured 
LEFT JOIN Contact on Insured.PrimaryContactId = contact.ContactId
WHERE (insured.isDeleted != 1 or insured.isDeleted is null) AND memberId=@memberId 
and (insured.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
 ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    var member = new MemberDetails()
                    {
                        MemberId = reader.GetInt32(0),
                        MemberNumber = reader.GetString(1),
                        MemberName = reader.GetString(2),
                        MemberStatus = reader.GetString(3),
                        OrganizationType = reader.GetString(4),
                        CompanyName = reader["companyName"].EmptyIfDBNullString(),
                        Address1 = reader["Address1"].EmptyIfDBNullString(),
                        Address2 = reader["Address2"].EmptyIfDBNullString(),
                        City = reader["city"].EmptyIfDBNullString(),
                        State = reader["state"].EmptyIfDBNullString(),
                        Zip = reader["zip"].EmptyIfDBNullString(),
                        Phone = reader["phone"].EmptyIfDBNullString(),
                        Website = reader["website"].EmptyIfDBNullString(),
                        StartDate = reader["startDate"].EmptyIfDBNullDate(),
                        StateofIncorporation = reader["StateofIncorporation"].EmptyIfDBNullString(),
                        FEIN = reader["fein"].EmptyIfDBNullString(),
                        Notes = reader["notes"].EmptyIfDBNullString(),
                        JoinDate = reader["joinDate"].EmptyIfDBNullDate()
                    };
                    var contactID = reader.GetValue(5);
                    if (contactID != System.DBNull.Value)
                    {
                        member.PrimaryContact = new Contact()
                        {
                            ContactID = reader.GetInt32(5),
                            FirstName = reader.GetString(6),
                            LastName = reader.GetString(7),
                            Email = reader.GetString(8)
                        };
                    }
                    var parentMemberId = reader["parentMember"].NullifDBNullInt();
                    if (parentMemberId.HasValue)
                    {
                        member.ParentMember = new Member()
                        {
                            MemberId = parentMemberId.Value
                        };
                    }
                    member.LinesofCoverage = this.GetMemberLinesofCoverage(memberId).ToList();

                    result = member;
                }
            }
        }
        return result;
    }

    public IEnumerable<permaAPI.models.members.noteSummary> GetAllNotes(int memberId, string status)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var noteSummaries = new List<permaAPI.models.members.noteSummary>();
        string query = @"
SELECT InsuredNote.categoryId, InsuredNote.Subject, InsuredNote.Status, InsuredNote.createdBy, InsuredNote.createdDate, InsuredNote.noteId, Notecategory.categoryTitle FROM InsuredNote
INNER JOIN NoteCategory on InsuredNote.categoryId = NoteCategory.categoryId WHERE (InsuredNote.status <> 'deleted' AND (@Status is Null or InsuredNote.status = @Status)) AND InsuredNote.memberId=@memberId ORDER BY InsuredNote.createddate DESC

  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.Add(new SqlParameter("@Status", System.Data.SqlDbType.VarChar));
                if (!string.IsNullOrEmpty(status))
                {
                    cmd.Parameters["@status"].Value = status;
                }
                else
                {
                    cmd.Parameters["@status"].Value = System.DBNull.Value;
                }

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var note = new permaAPI.models.members.noteSummary()
                    {
                        CategoryId = reader["categoryId"].asInt(),
                        CategoryTitle = reader["categoryTitle"].asString(),
                        Subject = reader["subject"].asString(),
                        Status = reader["status"].asString(),
                        CreatedBy = reader["createdBy"].asString(),
                        CreatedDate = reader["createdDate"].asDate(),
                        NoteId = reader["noteId"].asInt()
                    };

                    noteSummaries.Add(note);
                }
            }
        }
        return noteSummaries;
    }
    public permaAPI.models.members.note GetNote(int memberId, int noteId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        permaAPI.models.members.note note = null;
        string query = @"
SELECT InsuredNote.categoryId, InsuredNote.Subject, InsuredNote.Status, InsuredNote.content, InsuredNote.createdBy, InsuredNote.createdDate, InsuredNote.noteId, Notecategory.categoryTitle FROM InsuredNote
INNER JOIN NoteCategory on InsuredNote.categoryId = NoteCategory.categoryId WHERE (InsuredNote.status <> 'deleted'  ) AND InsuredNote.memberId=@memberId and InsuredNote.NoteId = @noteId

  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.AddWithValue("@noteId", noteId);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    note = new permaAPI.models.members.note()
                    {
                        CategoryId = reader["categoryId"].asInt(),
                        CategoryTitle = reader["categoryTitle"].asString(),
                        Subject = reader["subject"].asString(),
                        Status = reader["status"].asString(),
                        CreatedBy = reader["createdBy"].asString(),
                        CreatedDate = reader["createdDate"].asDate(),
                        NoteId = reader["noteId"].asInt(),
                        Content = reader["content"].asString()
                    };


                }
            }
        }
        return note;
    }

    public int SaveNote(int memberId, permaAPI.models.members.note note, string systemUser)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
declare @createdBy varchar(100);
select @createdBy = firstName + ' ' + lastname from contact where useridentifier=@systemUser;
 IF (@noteID IS NULL)
BEGIN
    INSERT INTO InsuredNote (categoryId, subject, content, status, createdBy, createdDate, memberId, applicationUser, modifiedBy, modifiedDate)
    VALUES (@categoryId, @subject, @content, @status, @createdBy, getdate(), @memberId, @systemUser, @systemUser, getDate());
select @noteID = Scope_identity();
END
ELSE
BEGIN
    UPDATE InsuredNote SET categoryId = @categoryId, subject=@subject, content=@content, status=@status, modifiedBy=@systemUser, modifiedDate=getdate() WHERE noteId=@NoteId
END
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@noteID", System.Data.SqlDbType.Int));
                cmd.Parameters["@NoteId"].Value = note.NoteId.DBNullIfNullInt();
                cmd.Parameters["@noteid"].Direction = System.Data.ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.AddWithValue("@categoryId", note.CategoryId);
                cmd.Parameters.AddWithValue("@subject", note.Subject);
                cmd.Parameters.AddWithValue("@content", note.Content);
                cmd.Parameters.AddWithValue("@status", note.Status);


                cmd.Parameters.AddWithValue("@systemUser", systemUser);

                conn.Open();
                cmd.ExecuteNonQuery();
                note.NoteId = cmd.Parameters["@noteId"].Value.asInt();
            }
        }
        return note.NoteId.Value;
    }
    public IEnumerable<permaAPI.models.members.Attachment> GetMemberAttachments(int memberId, string status)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var attachments = new List<permaAPI.models.members.Attachment>();
        string query = @"
Declare @Attachments Table
(
ApplicationId INT,
CoverageYear INT,
FileDescription varchar(max), 
CreatedDate datetime,
systemUser varchar(500),
CreatedBy varchar(500),
FilesJSON varchar(max),
elementId int,
rowId int
);

INSERT INTO @Attachments
select  application.ApplicationId, application.CoverageYear, case when ApplicationSectionElement.ShortName is null then applicationsectionelement.longtext when ApplicationSectionElement.ShortName = '' then applicationsectionelement.longtext else ApplicationSectionElement.ShortName end  ,  applicationElementResponse.createddate	,applicationElementResponse.CreatedBy 	, contact.FirstName + ' ' + contact.LastName,  LongTextResponse, applicationsectionelement.ElementId, applicationelementresponse.rowid	  from application INNER JOIN applicationElementResponse on applicationElementResponse.ApplicationId = application.ApplicationId INNER JOIN ApplicationSectionElement on ApplicationSectionElement.elementtype	= 10 and applicationElementResponse.ElementId = ApplicationSectionElement.ElementId INNER JOIN contact on contact.userIdentifier = applicationelementresponse.CreatedBy	
 WHERE longtextresponse is not null and memberId=@memberID;

SELECT Attachment.attachmentId, 1 [AttachmentType],  attachment.categoryId, attachment.policyPeriod,   attachment.FileName [originalFilename] , cast( attachment.memberID as varchar(500)) + '/' + cast( attachment.attachmentID as varchar(500)) + '/' +  attachment.FileName [FileName],    attachment.status, attachment.applicationUser, attachment.createdDate, attachment.CreatedBy, attachmentcategory.CategoryTitle, attachment.FileDescription, 0 [applicationid],0 [elementid],0[rowid]  FROM Attachment INNER JOIN AttachmentCategory ON Attachment.categoryId = attachmentcategory.CategoryId
WHERE attachment.memberid = @memberID and (attachment.status <> 'deleted' and (@status is null or attachment.status = @status)) 
UNION
SELECT  0 [attachmentId], 2 [AttachmentType], 0 [categoryId], J.coverageYear [PolicyPeriod],  JSON_VALUE(value, '$.FileName') [originalFilename],  JSON_VALUE(value, '$.FileId') [FileName], 'Active' [Status], J.SystemUser, j.CreatedDate, J.createdBy, 'Application' [Category], J.FileDescription, J.applicationId, J.elementid, j.rowid  from @attachments J CROSS APPLY OPENJSON (J.FilesJSON) as AttsData WHERE   ( (@status is null or 'active' = @status)) 
Order by  createdDate desc  


  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.Add(new SqlParameter("@Status", System.Data.SqlDbType.VarChar));
                if (!string.IsNullOrEmpty(status))
                {
                    cmd.Parameters["@status"].Value = status;
                }
                else
                {
                    cmd.Parameters["@status"].Value = System.DBNull.Value;
                }

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var attachemnt = new permaAPI.models.members.Attachment()
                    {
                        AttachmentId = reader["attachmentId"].asInt(),
                        applicationUser = reader["applicationUser"].asString(),
                        CategoryId = reader["categoryId"].asInt(),
                        CreatedBy = reader["createdBy"].asString(),
                        CreatedDate = reader["createdDate"].asDate(),
                        FileName = reader["fileName"].asString(),
                        OriginalFileName = reader["originalFileName"].asString(),
                        Status = reader["status"].asString(),
                        MemberId = memberId,
                        PolicyPeriod = reader["policyPeriod"].asInt(),
                        CategoryTitle = reader["categoryTitle"].asString(),
                        FileDescription = reader["FileDescription"].asString(),
                        ApplicationId = reader["ApplicationId"].asInt(),
                        ElementId = reader["ElementId"].asInt(),
                        RowId = reader["rowId"].asInt()

                    };

                    attachments.Add(attachemnt);
                }
            }
        }
        return attachments;
    }

    public int SaveMemberAttachment(int memberId, permaAPI.models.members.Attachment attachment, string systemUser)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var attachments = new List<permaAPI.models.members.Attachment>();
        string query = @"
declare @User varchar(500)
select @User = FirstName + ' ' + LastName from contact where useridentifier = @userIdentifier

  if (@attachmentID is NULL)
BEGIN
    INSERT INTO Attachment (memberId, categoryId, policyPeriod, createdBy, FileName,  CreatedDate, Status, ApplicationUser, ModifiedDate, ModifiedBy, FileDescription)
    SELECT @MemberId, @categoryID, @policyPeriod, @User,  @FileName, getDate(), @Status, @userIdentifier, getDate(), @user, @FileDescription; Select @attachmentID = Scope_Identity();
 
END
ELSE
BEGIN
    UPDATE Attachment Set categoryID=@CategoryID, policyPeriod=@policyPeriod,  Status=@Status, ModifiedDate=getdate(), ModifiedBy=@User
WHERE AttachmentID=@attachmentID
END

  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@attachmentId", System.Data.SqlDbType.Int));
                cmd.Parameters["@attachmentID"].Direction = System.Data.ParameterDirection.InputOutput;
                cmd.Parameters["@attachmentID"].Value = attachment.AttachmentId.DBNullIfNullInt();
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.AddWithValue("@categoryID", attachment.CategoryId);
                cmd.Parameters.AddWithValue("@policyPeriod", attachment.PolicyPeriod);
                cmd.Parameters.AddWithValue("@FileName", attachment.FileName);
                cmd.Parameters.AddWithValue("@fileDescription", attachment.FileDescription.EmptyIfNullString(500));
                cmd.Parameters.AddWithValue("@status", attachment.Status);
                cmd.Parameters.AddWithValue("@userIdentifier", systemUser);
                conn.Open();
                cmd.ExecuteNonQuery();
                attachment.AttachmentId = cmd.Parameters["@attachmentID"].Value.asInt();

            }
        }
        return attachment.AttachmentId.Value;
    }

    public permaAPI.models.members.Attachment GetMemberAttachment(int attachmentId, string systemUser)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        permaAPI.models.members.Attachment attachment = null;
        string query = @"
   declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;

SELECT Attachment.attachmentId, 1 [AttachmentType],  attachment.categoryId, attachment.policyPeriod,   attachment.FileName [originalFilename] , cast( attachment.memberID as varchar(500)) + '/' + cast( attachment.attachmentID as varchar(500)) + '/' +  attachment.FileName [FileName],    attachment.status, attachment.applicationUser, attachment.createdDate, attachment.CreatedBy, attachmentcategory.CategoryTitle, attachment.FileDescription, 0 [applicationid],0 [elementid],0[rowid], attachment.memberid  FROM Attachment INNER JOIN AttachmentCategory ON Attachment.categoryId = attachmentcategory.CategoryId
WHERE attachment.attachmentId=@attachmentID
 and (attachment.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )


  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@attachmentId", attachmentId);
                cmd.Parameters.AddWithValue("@userIdentifier", systemUser);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    attachment = new permaAPI.models.members.Attachment()
                    {
                        AttachmentId = reader["attachmentId"].asInt(),
                        applicationUser = reader["applicationUser"].asString(),
                        CategoryId = reader["categoryId"].asInt(),
                        CreatedBy = reader["createdBy"].asString(),
                        CreatedDate = reader["createdDate"].asDate(),
                        FileName = reader["fileName"].asString(),
                        OriginalFileName = reader["originalFileName"].asString(),
                        Status = reader["status"].asString(),
                        MemberId = reader["memberId"].asInt(),
                        PolicyPeriod = reader["policyPeriod"].asInt(),
                        CategoryTitle = reader["categoryTitle"].asString(),
                        FileDescription = reader["FileDescription"].asString(),
                        ApplicationId = reader["ApplicationId"].asInt(),
                        ElementId = reader["ElementId"].asInt(),
                        RowId = reader["rowId"].asInt()

                    };

                }
            }
        }
        return attachment;

    }
}