using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class PersonalUserData : UserData
{
    [Required]
    public GenderType gender { get; set; }
    public EducationalProgram educational_program { get; set; }
}