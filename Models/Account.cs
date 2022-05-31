
using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Models
{
    public class Account
    {

        public Account()
        {
           // Usuario = new User();
        }

        public int id { get; set; }

        [Required]
        [StringLength(320, ErrorMessage = "El correo es demasiado largo. No debe exceder de 320 caracteres")]
        public string? Correo { get; set; }

        [Required]
        [MinLength(8)]
        public string? Contrasena { get; set; }

        //private User Usuario { get; set; }

        
    }
}
