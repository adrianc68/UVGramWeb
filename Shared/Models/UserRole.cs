using System;
using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Models
{
    public class UserRole
    {
        public int id { get; set; }
        
        [Required]
        public int DiaNacimiento { get; set; }
        [Required]
        public int MesNacimiento { get; set; }
        [Required]
        public int AñoNacimiento { get; set; }

        [Required]
        public string? NombreCompleto { get; set; }
        [Required]
        public string? Telefono { get; set; }
        public Personal Personal { get; set; }
        public Company Empresa { get; set; }


    }
}