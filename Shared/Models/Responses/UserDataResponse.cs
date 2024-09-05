using UVGramWeb.Shared.Data;

public class UserDataResponse
{
    public string Name { get; set; }
    public string Presentation { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone_Number { get; set; }
    public string Birthday { get; set; }
    public RoleType Role { get; set; }
    public PrivacyType Privacy { get; set; }
    public string Url { get; set;}
}