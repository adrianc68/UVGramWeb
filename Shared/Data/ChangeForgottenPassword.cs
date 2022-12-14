using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class ChangeForgottenPassword
{
    [Required]
    [MinLength(6)]
    public string password { get; set; }
}