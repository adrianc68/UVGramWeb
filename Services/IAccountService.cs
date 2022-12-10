
namespace UVGramWeb.Services;

public interface IAccountService
{
    Task<Boolean> VerifyUsername(string username);
    Task<Boolean> VerifyEmailAddress(string email);
}