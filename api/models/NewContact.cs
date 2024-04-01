public class NewContact
{
    public int ContactID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get { return this.FirstName + " " + this.LastName; } }
    public string Email { get; set; }
    public string ContactType { get; set; }
    public int InitialMemberID {get;set;}
}