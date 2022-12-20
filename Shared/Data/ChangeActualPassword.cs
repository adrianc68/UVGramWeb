using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class ChangeActualPassword
{
    [Required]
    [MinLength(6)]
    public string password { get; set; }
    [Required]
    [MinLength(6)]
    public string oldPassword { get; set; }
}