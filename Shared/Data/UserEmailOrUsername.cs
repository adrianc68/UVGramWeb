using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class UserEmailOrUsername
{
    [Required]
    public string emailOrUsername { get; set; }
}