namespace UVGramWeb.Shared.Data;

public class Comment : AbstractPost
{
    public string comment { get; set; }
    public string created_time { get; set; }
    public string username { get; set; }
    public string url { get; set; }
    public int likes { get; set; }
    public bool isLiked { get; set; }
    public bool isReplyInnerComment { get; set; }
    public Comment parent_comment { get; set; }
    public string rootCommentUUID {get; set;}
    public List<Comment> replies { get; set; } 
}