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

public interface IContactsService
{
    public IEnumerable<Contact> GetAllContacts();
    public IEnumerable<Contact> GetMemberContacts(int contactId, string userIdentifier);
    public ContactDetails GetContact(int contactId);
    public int AddContact(NewContact contact, string systemUser);
    public bool UpdateContact(ContactDetails contact, string systemUser);
    public IEnumerable<Member> GetContactMembers(int contactId);
    public bool SaveContactMembers(int contactId, IList<int> memberIds);
    public bool AddContactToMember(int contactId, int memberId);
    public bool MakeContactprimary(int contactId, int memberId);
    public bool DeleteContactFromMember(int contactId, int memberId);
    public bool isUserAdmin(string userIdentifier);
    public bool makeUserAdmin(int contactId);
    public permaAPI.models.UserProfile GetUserProfile(string userIdentifier);

}
public class ContactsService : IContactsService
{
    public IConfiguration Configuration { get; }
    public ContactsService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IEnumerable<Contact> GetAllContacts()
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var _contacts = new List<Contact>();
        string query = @"SELECT TOP (1000) [ContactId]
      ,[Salutation]
      ,[FirstName]
      , Email
      ,[LastName]
      ,contacttype
    
  FROM [dbo].[contact]
WHERE (contact.isDeleted != 1 or contact.isDeleted is null) Order by lastname ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    var contact = new Contact()
                    {
                        ContactID = reader["contactId"].asInt(),
                        FirstName = reader["firstName"].EmptyIfDBNullString(),
                        LastName = reader["lastName"].EmptyIfDBNullString(),
                        Email = reader["email"].EmptyIfDBNullString(),
                        ContactType = reader["contactType"].EmptyIfDBNullString()
                    };
                    _contacts.Add(contact);
                }
            }
        }
        return _contacts;
    }

    public permaAPI.models.UserProfile GetUserProfile(string userIdentifier)
    {

        permaAPI.models.UserProfile profile = new permaAPI.models.UserProfile();
        profile.isAdmin = false;

        string connectionString = this.Configuration.GetConnectionString("permaportal");
        string query = @"
declare @contactid int;
select @contactid = ContactId from contact where userIdentifier=@useridentifier;
select contact.contactid from contact inner join userPermission on contact.ContactId = userPermission.contactId where userIdentifier =@userIdentifier

select memberId from InsuredContact where ContactId=@contactid and (isdeleted is null or isdeleted = 0)
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    profile.isAdmin = true;
                }
                reader.NextResult();
                var memberList = new List<int>();
                while (reader.Read())
                {
                    memberList.Add(reader.GetInt32(0));
                }
                profile.MemberIds = memberList;
            }
        }

        return profile;
    }
    public bool isUserAdmin(string userIdentifier)
    {
        bool isAdmin = false;
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        string query = @"
select contact.contactid from contact inner join userPermission on contact.ContactId = userPermission.contactId where userIdentifier =@userIdentifier
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    isAdmin = true;
                }
            }
        }

        return isAdmin;
    }

    public IEnumerable<Contact> GetMemberContacts(int memberId, string userIdentifier)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var _contacts = new List<Contact>();
        string query = @"
 declare @isAdmin bit = 0;
 select @isAdmin = 1 from contact inner join userPermission on contact.ContactId = userPermission.contactId
 where contact.userIdentifier = @userIdentifier;
 
  declare @primarycontactid int 
select @primarycontactid = primarycontactid from insured where memberid = @memberid;
SELECT TOP (1000) [ContactId]
      ,[Salutation]
      ,[FirstName]
      , Email
      ,[LastName]
      ,contacttype
    , jobtitle
    , case when @primarycontactid = contactid then 1 else 0 end [isPrimaryContact]
    
  FROM [dbo].[contact]
WHERE (contact.isDeleted != 1 or contact.isDeleted is null) 
    and contact.contactid in 
        (select contactid from insuredcontact where memberid=@memberId and (isdeleted = 0 or isdeleted is null)
        and (insuredcontact.MemberId in (select InsuredContact.MemberId from InsuredContact inner join contact on InsuredContact.ContactId = contact.ContactId and contact.userIdentifier = @userIdentifier) or @isAdmin=1 )
        )  
 Order by lastname

";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@memberid", memberId);
                cmd.Parameters.AddWithValue("@userIdentifier", userIdentifier);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var contact = new Contact()
                    {
                        ContactID = reader["contactId"].asInt(),
                        FirstName = reader["firstName"].EmptyIfDBNullString(),
                        LastName = reader["lastName"].EmptyIfDBNullString(),
                        Email = reader["email"].EmptyIfDBNullString(),
                        ContactType = reader["contactType"].EmptyIfDBNullString(),
                        Jobtitle = reader["jobtitle"].EmptyIfDBNullString(),
                        isPrimaryContact = (reader["isPrimaryContact"].asInt() > 0) ? true : false
                    };
                    _contacts.Add(contact);
                }
            }
        }
        return _contacts;
    }
    public ContactDetails GetContact(int contactId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        ContactDetails contact = null;
        string query = @"
 SELECT [ContactId]
       ,[InsuredId]
      ,[Salutation]
      ,[FirstName]
      ,[MiddleName]
      ,[LastName]
      ,[JobTitle]
      ,[ContactType]
      ,[BusinessPhone]
      ,[HomePhone]
      ,[MobilePhone]
      ,[Fax]
      ,[Email]
      ,[AddressType]
      ,[Address1]
      ,[Address2]
      ,[City]
      ,[State]
      ,[Zip]
      ,[Phone]
      ,[Description]
      ,[isBoardMember]
      ,[isExecutiveCommitteeMember]
      ,[isDeleted]
      , userIdentifier
      , cast((case when userIdentifier is not null then 1 else 0 end) as bit)   [isPortalUser]
      ,  cast((case when exists (select * from userPermission where contactid=@contactid) then 1 else 0 end) as bit) [isAdmin]
  FROM [dbo].[contact]
  WHERE contactid=@contactiD";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@contactid", contactId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    contact = new ContactDetails()
                    {
                        ContactID = reader["contactId"].asInt(),
                        FirstName = reader["firstName"].EmptyIfDBNullString(),
                        LastName = reader["lastName"].EmptyIfDBNullString(),
                        Email = reader["email"].EmptyIfDBNullString(),
                        ContactType = reader["contactType"].EmptyIfDBNullString(),
                        Salutation = reader["Salutation"].EmptyIfDBNullString(),
                        MiddleName = reader["middlename"].EmptyIfDBNullString(),
                        JobTitle = reader["jobtitle"].EmptyIfDBNullString(),
                        BusinessPhone = reader["BusinessPhone"].EmptyIfDBNullString(),
                        HomePhone = reader["HomePhone"].EmptyIfDBNullString(),
                        MobilePhone = reader["MobilePhone"].EmptyIfDBNullString(),
                        Fax = reader["Fax"].EmptyIfDBNullString(),
                        AddressType = reader["AddressType"].EmptyIfDBNullString(),
                        Address1 = reader["Address1"].EmptyIfDBNullString(),
                        Address2 = reader["Address2"].EmptyIfDBNullString(),
                        City = reader["City"].EmptyIfDBNullString(),
                        State = reader["State"].EmptyIfDBNullString(),
                        Zip = reader["Zip"].EmptyIfDBNullString(),
                        Phone = reader["Phone"].EmptyIfDBNullString(),
                        Description = reader["Description"].EmptyIfDBNullString(),
                        isBoardMember = reader["isBoardMember"].asBoolean(),
                        isExecutiveCommitteeMember = reader["isExecutiveCommitteeMember"].asBoolean(),
                        isApplicationUser = reader["isPortalUser"].asBoolean(),
                        isPermaAdmin = reader["isAdmin"].asBoolean(),
                        userIdentifier = reader["userIdentifier"].EmptyIfDBNullString()
                    };
                };
                if (contact != null)
                {
                    contact.Members = this.GetContactMembers(contact.ContactID).ToList();
                }
            }
        }
        return contact;
    }

    public int AddContact(NewContact contact, string systemUser)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"INSERT INTO contact
            (FirstName, LastName, Email, ContactType, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
            VALUES (@FirstName, @LastName, @Email, @ContactType, @systemUser, getdate(), @systemUser, getdate());
            select @contactid= Scope_identity();
                INSERT INTO InsuredContact (memberId,   ContactID) values (@memberID, @contactID)
            ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@contactId", System.Data.SqlDbType.Int));
                cmd.Parameters["@contactid"].Direction = System.Data.ParameterDirection.InputOutput;
                cmd.Parameters["@contactid"].Value = System.DBNull.Value;
                conn.Open();
                cmd.Parameters.AddWithValue("@FirstName", contact.FirstName.EmptyIfDBNullString());
                cmd.Parameters.AddWithValue("@LastName", contact.LastName.EmptyIfDBNullString());
                cmd.Parameters.AddWithValue("@Email", contact.Email.EmptyIfDBNullString());
                cmd.Parameters.AddWithValue("@ContactType", contact.ContactType.EmptyIfDBNullString());
                cmd.Parameters.AddWithValue("@SystemUser", systemUser);
                cmd.Parameters.AddWithValue("@memberId", contact.InitialMemberID);
                cmd.ExecuteNonQuery();
                contact.ContactID = cmd.Parameters["@contactid"].Value.asInt();
            }
        }
        return contact.ContactID;
    }
    public bool UpdateContact(ContactDetails contact, string userIdentifier)
    {

        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @" UPDATE contact
                SET FirstName=@FirstName, LastName=@LastName,
                Email=@Email, ContactType=@ContactType,
                 MiddleName=@MiddleName, Salutation=@Salutation,
                 JobTitle=@JobTitle, 
                 BusinessPhone=@BusinessPhone, HomePhone=@HomePhone,
                 MobilePhone=@MobilePhone, Fax=@Fax, AddressType=@Addresstype,
                 Address1=@Address2, City=@City, State=@State, Zip=@Zip,
                 Phone=@Phone, Description=@Description, IsBoardMember=@IsBoardMember,
                 isExecutiveCommitteeMember=@isExecutiveCommitteeMember,
                 updatedby=@systemuser, updateddate=getdate(),
                userIdentifier=@userIdentifier
                 WHERE contactID=@ContactID;
            ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@ContactID", contact.ContactID);
                cmd.Parameters.AddWithValue("@FirstName", contact.FirstName.EmptyIfNullString(100));
                cmd.Parameters.AddWithValue("@LastName", contact.LastName.EmptyIfNullString(100));
                cmd.Parameters.AddWithValue("@Email", contact.Email.EmptyIfNullString(100));
                cmd.Parameters.AddWithValue("@ContactType", contact.ContactType.EmptyIfNullString(50));
                cmd.Parameters.Add(new SqlParameter("@Salutation", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@MiddleName", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@address1", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@address2", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@fax", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@JobTitle", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@description", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@phone", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@city", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@MobilePhone", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@HomePhone", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@Zip", System.Data.SqlDbType.VarChar));
                cmd.Parameters.Add(new SqlParameter("@BusinessPhone", System.Data.SqlDbType.VarChar));
                cmd.Parameters["@Salutation"].Value = contact.Salutation.EmptyIfNullString(100);
                cmd.Parameters["@JobTitle"].Value = contact.JobTitle.EmptyIfNullString(100);
                cmd.Parameters["@MiddleName"].Value = contact.MiddleName.EmptyIfNullString(100);
                cmd.Parameters["@BusinessPhone"].Value = contact.BusinessPhone.EmptyIfNullString(50);
                cmd.Parameters["@MobilePhone"].Value = contact.MobilePhone.EmptyIfNullString(50);
                cmd.Parameters["@HomePhone"].Value = contact.HomePhone.EmptyIfNullString(50);
                cmd.Parameters["@Fax"].Value = contact.Fax.EmptyIfNullString(50);
                cmd.Parameters["@Address1"].Value = contact.Address1.EmptyIfNullString(200);
                cmd.Parameters["@Address2"].Value = contact.Address2.EmptyIfNullString(200);
                cmd.Parameters["@City"].Value = contact.City.EmptyIfNullString(200);
                cmd.Parameters["@Phone"].Value = contact.Phone.EmptyIfNullString(50);
                cmd.Parameters["@Zip"].Value = contact.Zip.EmptyIfNullString(50);
                cmd.Parameters["@Description"].Value = contact.Description.EmptyIfNullString(1000);

                cmd.Parameters.AddWithValue("@AddressType", contact.AddressType.EmptyIfNullString(50));

                cmd.Parameters.AddWithValue("@State", contact.State.EmptyIfNullString(50));

                cmd.Parameters.AddWithValue("@IsBoardMember", contact.isBoardMember);
                cmd.Parameters.AddWithValue("@isExecutiveCommitteeMember", contact.isExecutiveCommitteeMember);
                cmd.Parameters.AddWithValue("@userIdentifier", contact.userIdentifier.DBNullIfEmptyString(200));
                cmd.Parameters.AddWithValue("@SystemUser", userIdentifier);
                cmd.ExecuteNonQuery();
                var memberIds = (from members in contact.Members select members.MemberId).ToList();
                this.SaveContactMembers(contact.ContactID, memberIds);
            }
            return true;
        }
    }

    public bool makeUserAdmin(int contactId)
    {

        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
Insert into Userpermission 
(permissionid, contactid)
         select permissionid, @contactid from permissiontype where  permissionname='admin'
            ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@contactid", contactId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return true;
    }
    public IEnumerable<Member> GetContactMembers(int contactId)
    {
        var contactMembers = new List<Member>();
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"  
        SELECT Insured.MemberId, MemberName, MemberNumber, MemberStatus,OrganizationType 
        FROM Insured 
        Where Insured.memberID in 
            (select InsuredContact.memberID from InsuredContact where contactID=@ContactID) 
            Order by MemberName
            ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@contactid", contactId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var member = new Member()
                    {
                        MemberId = reader["memberid"].asInt(),
                        MemberName = reader["memberName"].EmptyIfDBNullString(),
                        MemberNumber = reader["memberNumber"].EmptyIfDBNullString(),
                        MemberStatus = reader["memberStatus"].EmptyIfDBNullString(),
                        OrganizationType = reader["OrganizationType"].EmptyIfDBNullString()
                    };
                    contactMembers.Add(member);
                }
            }
        }
        return contactMembers;
    }
    public bool SaveContactMembers(int contactId, IList<int> memberIds)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
delete from InsuredContact where contactid=@contactid; ";
        for (int i = 0; i < memberIds.Count; i++)
        {
            query += $"INSERT INTO InsuredContact (contactid, MemberId) Values(@contactid, @memberId${i})";
        }
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@contactId", contactId);
                for (int i = 0; i < memberIds.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@memberId${i}", memberIds[i]);
                }
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return true;
    }

    public bool DeleteContactFromMember(int contactId, int memberId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
    update   InsuredContact set isDeleted = 1 where contactid=@contactid and memberid=@memberid
";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@contactId", contactId);
                cmd.Parameters.AddWithValue("@memberid", memberId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return true;

    }
    public bool MakeContactprimary(int contactId, int memberId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
   update insured set PrimaryContactId = @contactId  where insured.MemberId = @memberid;
";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@contactId", contactId);
                cmd.Parameters.AddWithValue("@memberid", memberId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return true;
    }

    public bool AddContactToMember(int contactId, int memberId)
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");

        string query = @"
    INSERT INTO InsuredContact (memberId, contactId) Select @memberID, @contactid where not exists (select * from insuredContact where memberid=@memberId and contactid=@contactid)
";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@contactId", contactId);
                cmd.Parameters.AddWithValue("@memberid", memberId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return true;
    }
}