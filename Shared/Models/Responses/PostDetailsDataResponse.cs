using UVGramWeb.Shared.Data;

public class PostDetailsDataResponse
{
 public Post Post { get; set; }
 public int Likes { get; set; }
 public List<Comment> Comments {get; set;}
public List<PostFile> Files { get; set; }
public UserSearch Owner {get; set; }
}