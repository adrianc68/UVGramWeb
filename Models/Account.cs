
using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Models
{
    public class Account
    {
        public int id { get; set; }
        [Required]
        [StringLength(320, ErrorMessage = "El correo es demasiado largo. No debe exceder de 320 caracteres")]
        public string Correo { get; set; }
        
        [Required]
        [MinLength(8, ErrorMessage = "La contraseña debe tener por lo menos 8 caracteres")]
        public string Contrasena { get; set; }
    }
}
