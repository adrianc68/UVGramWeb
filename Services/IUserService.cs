using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UVGramWeb.Models;

namespace UVGramWeb.Data.Services
{
    public interface IUserService
    {
        User user { get; }
        Task Initialize();
        Task Login();
        Task RegisterUser(User user);

    }
}