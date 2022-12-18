using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class UserData
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
    [RegularExpression("^[a-z0-9_]+(\\.([a-z0-9_]+))*$", ErrorMessage = "username must have allowed characters: words, numbers. no allowed spaces and period as last character and lowercase")]
    public string username { get; set; }
    [Required]
    [EmailAddress]
    [MinLength(3)]
    [MaxLength(254)]
    public string email { get; set; }
    [Required(AllowEmptyStrings = false)]
    [MinLength(8)]
    [MaxLength(15)]
    [RegularExpression("^[\\d]*$", ErrorMessage = "phoneNumber must be digits")]
    public string phoneNumber { get; set; }
    [Required]
    public string birthdate { get; set; }
    [Required]
    public RoleType role { get; set; }
    [Required]
    public PrivacyType privacy { get; set; }
}