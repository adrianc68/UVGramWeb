using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class Login
{
    [Required]
    public string emailOrUsername { get; set; }

    [Required]
    public string password { get; set; }
}