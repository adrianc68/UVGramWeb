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
        public string Nombre { get; set; }
        public string Presentacion { get; set; }
        public string? NombreUsuario { get; set; }
        public UserRole RolUsuario { get; set; }
        public Account Cuenta { get; set; }
    }
}