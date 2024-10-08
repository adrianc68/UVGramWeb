namespace UVGramWeb.Shared.Data;

public class PostDetails : AbstractPost
{
  public string description { get; set; }
  public bool comments_allowed { get; set; }
  public bool likes_allowed { get; set; }
  public bool isLiked { get; set; }
  public int likes { get; set; }
  public string created_time { get; set; }
  public List<PostFile> files { get; set; }
  public List<Comment> comments { get; set; }
  public UserSearch owner { get; set; }


}