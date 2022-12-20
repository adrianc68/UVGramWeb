namespace UVGramWeb.Shared.Data;

public class Profile
{
    public string name { get; set; }
    public string presentation { get; set; }
    public string username { get; set; }
    public int followers { get; set; }
    public int followed { get; set; }
    public bool isFollowed { get; set; }
    public bool isFollowerRequestSent { get; set; }
    public bool isBlocked { get; set; }
    public bool isBlocker { get; set; }
    public bool isFollower { get; set; }
    public int postsCreated { get; set; }
    public bool hasSubmittedFollowerRequest { get; set; }
    public PrivacyType privacyType { get; set; }
    public List<Post> posts { get; set; }
}