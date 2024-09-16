namespace UVGramWeb.Shared.Models;

using System.ComponentModel.DataAnnotations;

public class CreatePostPubRequest
{
  [Required]
  public string Description { get; set; }
  [Required]
  public bool CommentsAllowed { get; set; }
  [Required]
  public bool LikesAllowed { get; set; }
  [Required]
  public List<Image> Files {get; set;} = new List<Image>();
}