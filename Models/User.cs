
using System;
using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Models
{
    public class User
    {
        public int id { get; set; }
        [Required]
        [StringLength(320, ErrorMessage = "El nombre es demasiado largo, debe tener menos de 320 caracteres.")]
        public string? nombre { get; set; }
        public string? presentacicon { get; set; }
        [Required]
        [StringLength(320, ErrorMessage = "El nombre de usuario es demasiado largo, debe tener menos de 60 caracteres")]
        public string? nombre_usuario { get; set; }
    }
}