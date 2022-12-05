using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class Login
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}