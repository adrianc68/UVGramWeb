namespace UVGramWeb.Shared.Data;

public class UserSearch
{
    public string name { get; set; }
    public string username { get; set; }
    public bool isFollowed { get; set; } = false;
    public bool isFollowerRequestSent { get; set; } = false;
    public bool isFollower { get; set; } = false;
    public bool hasSubmittedFollowerRequest { get; set; } = false;

}