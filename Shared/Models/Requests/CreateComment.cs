using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Models;

public class CreateComment
{
    [Required]
    public string comment { get; set; }
    [Required]
    public string uuid { get; set; }
}