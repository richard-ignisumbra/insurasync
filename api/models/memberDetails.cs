using System.Collections.Generic;
public class MemberDetails
{
    public int MemberId { get; set; }
    public string MemberName { get; set; }
    public string MemberNumber { get; set; }

    public string MemberStatus { get; set; }
    public string OrganizationType { get; set; }

    public Contact PrimaryContact { get; set; }

    public string CompanyName { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public Member ParentMember { get; set; }
    public System.DateTime? StartDate { get; set; }
    public string StateofIncorporation { get; set; }
    public string FEIN { get; set; }
    public List<string> LinesofCoverage { get; set; }
    public string Notes { get; set; }
    public System.DateTime? JoinDate { get; set; }

}