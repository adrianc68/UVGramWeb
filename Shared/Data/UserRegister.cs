using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class UserRegister
{
    [Required]
    public string name { get; set; }
    public string presentation { get; set; }
    [Required]
    public string username { get; set; }
    [Required]
    public string password { get; set; }
    [Required]
    public string phoneNumber { get; set; }
    [Required]
    public string email { get; set; }
    public DateTime birthdate { get; set; }
    public string verificationCode { get; set; }
}