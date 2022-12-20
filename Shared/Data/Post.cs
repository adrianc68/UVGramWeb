namespace UVGramWeb.Shared.Data;

public class Post : AbstractPost
{
    public string description { get; set; }
    public bool comments_allowed { get; set; }
    public bool likes_allowed { get; set; }
    public bool isLiked { get; set; }
    public int likes { get; set; }
    public int comments { get; set; }
    public List<PostFile> files { get; set; }
}