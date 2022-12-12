using UVGramWeb.Shared.Data;
namespace UVGramWeb.Services;

public interface IAccountService
{
    Task<Boolean> VerifyUsername(string username);
    Task<Boolean> VerifyEmailAddress(string email);
    Task<Boolean> CreateVerificationCode(UserCreateVerification model);
    Task<Boolean> CreateAccount(UserRegister model);
}