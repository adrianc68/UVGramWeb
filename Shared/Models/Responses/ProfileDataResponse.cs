using UVGramWeb.Shared.Data;

public class ProfileDataResponse
{
 public string Name { get; set; } 
 public string Presentation { get; set; } 
 public string Username { get; set; } 
 public int Followed { get; set; } 
 public int Followers { get; set; } 
 public string PrivacyType { get; set; } 
 public int PostsCreated { get; set; } 
 public bool IsFollowed { get; set; } 
 public bool IsFollowerRequestSent { get; set; } 
 public bool IsBlocked { get; set; } 
 public bool IsBlocker { get; set; } 
 public bool IsFollower { get; set; } 
 public bool HasSubmittedFollowerRequest { get; set; } 
 public Post[] Posts { get; set; } 
}