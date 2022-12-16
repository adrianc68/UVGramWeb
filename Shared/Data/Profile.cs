namespace UVGramWeb.Shared.Data;

public class Profile
{
    public string name { get; set; }
    public string presentation { get; set; }
    public string username { get; set; }
    public int followers { get; set; }
    public int followed { get; set; }
    public bool isFollowed { get; set; }
    public PrivacyType privacyType { get; set; }
    public List<Post> posts { get; set; }
}