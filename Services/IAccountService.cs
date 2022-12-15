using UVGramWeb.Shared.Data;
namespace UVGramWeb.Services;

public interface IAccountService
{
    Task<Boolean> VerifyUsername(string username);
    Task<Boolean> VerifyEmailAddress(string email);
    Task<Boolean> CreateVerificationCode(UserCreateVerification model);
    Task<Boolean> CreateAccount(UserRegister model);
    Task<Boolean> CreateResetConfirmationAddress(UserEmailOrUsername model);
    Task<Boolean> ChangePasswordByURL(ChangeForgottenPassword model, string uriData);
    Task<Boolean> VerifyURLChangePassword(string uri);
    Task<Boolean> DoesUsernameExist(string username);
    Task<Profile> GetProfile(string username);
}