namespace UVGramWeb.Shared.Data;

public class UserData
{
    public string name { get; set; }
    public string presentation { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string phone_number { get; set; }
    public string birthday { get; set; }
    public RoleType role { get; set; }
    public PrivacyType privacy { get; set; }
}