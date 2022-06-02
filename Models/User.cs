using System;
using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Models
{
    public class User
    {

        public User()
        {
           RolUsuario = new UserRole();
           Cuenta = new Account();
        }

        public int id { get; set; }
        [Required]
        [StringLength(320, ErrorMessage = "El nombre es demasiado largo, debe tener menos de 320 caracteres.")]
        public string Nombre { get; set; }
        public string Presentacion { get; set; }
        [Required]
        [StringLength(320, ErrorMessage = "El nombre de usuario es demasiado largo, debe tener menos de 60 caracteres")]
        public string? NombreUsuario { get; set; }
        public UserRole RolUsuario { get; set; }
        public Account Cuenta { get; set; }
    }
}