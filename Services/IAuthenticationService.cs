using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Models;

namespace UVGramWeb.Services;

public interface IAuthenticationService
{
    User User { get; }
    Task Initialize();
    Task<MessageType> Login(Login model);
    Task Logout();
    Task UpdateData();
    void NotifyUserLoginChange();
}
