using System.Collections.Generic;
public class ContactDetails
{
    public int ContactID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get { return this.FirstName + " " + this.LastName; } }
    public string Email { get; set; }
    public string Salutation { get; set; }
    public string MiddleName { get; set; }
    public string JobTitle { get; set; }
    public string ContactType { get; set; }
    public string BusinessPhone { get; set; }
    public string HomePhone { get; set; }
    public string MobilePhone { get; set; }
    public string Fax { get; set; }
    public string AddressType { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Phone { get; set; }
    public string Description { get; set; }
    public bool isBoardMember { get; set; }
    public bool isExecutiveCommitteeMember { get; set; }
    public bool isApplicationUser { get; set; }
    public string userIdentifier { get; set; }
    public bool isPermaAdmin { get; set; }
    public IList<Member> Members { get; set; }
}