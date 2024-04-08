using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Shared.Data;

public class Username
{
    [Required]
    public string username { get; set; }
}