public class Contact
{
    public int ContactID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get { return this.FirstName + " " + this.LastName; } }
    public string Email { get; set; }
    public string ContactType { get; set; }
    public string Jobtitle { get; set; }
    public bool isPrimaryContact { get; set; }
}