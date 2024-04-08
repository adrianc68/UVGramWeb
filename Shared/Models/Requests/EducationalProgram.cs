using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class EducationalProgram
{
    [Required]
    public int id { get; set; }
    public string educational_program { get; set; }
    public Faculty faculty { get; set; }
}