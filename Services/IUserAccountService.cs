using UVGramWeb.Shared.Models;
using UVGramWeb.Shared.Data;

namespace UVGramWeb.Services;

public interface IUserAccountService
{
    User User { get; }
    Task Initialize();
    Task Login(Login model);
    Task Logout();
}