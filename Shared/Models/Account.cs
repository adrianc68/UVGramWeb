
using System.ComponentModel.DataAnnotations;

namespace UVGramWeb.Models
{
    public class Account
    {
        public int id { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }
}
