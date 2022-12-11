using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class UserRegister
{
    [Required]
    [MinLength(3)]
    [MaxLength(64)]
    [RegularExpression("^[a-zA-Zñ]+(\\ ([a-zA-Zñ]+))*$")]
    public string name { get; set; }
    [MinLength(0)]
    [MaxLength(150)]
    public string presentation { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(30)]
    [RegularExpression("^[\\w]+(\\.([\\w]+))*$", ErrorMessage = "username must have allowed characters: words, numbers. no allowed spaces and period as last character")]
    public string username { get; set; }
    [Required]
    [MinLength(6)]
    [MaxLength(128)]
    public string password { get; set; }
    [Required(AllowEmptyStrings = false)]
    [MinLength(8)]
    [MaxLength(15)]
    [RegularExpression("^[\\d]*$", ErrorMessage = "phoneNumber must be digits")]
    public string phoneNumber { get; set; }
    [Required]
    [EmailAddress]
    [MinLength(3)]
    [MaxLength(254)]
    public string email { get; set; }
    [Required]
    public DateTime birthdate { get; set; }
    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string verificationCode { get; set; }
}