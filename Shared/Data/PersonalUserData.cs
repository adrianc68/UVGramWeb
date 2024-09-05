using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class PersonalUserData : UserData
{
 public GenderType Gender { get; set; }
 public EducationalProgram Educational_program { get; set; }
}