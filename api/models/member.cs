public class Member
{
    public int MemberId { get; set; }
    public string MemberName { get; set; }
    public string MemberNumber { get; set; }
    public string MemberStatus { get; set; }
    public string OrganizationType { get; set; }
    public Contact PrimaryContact { get; set; }
    public Member ParentMember { get; set; }
}