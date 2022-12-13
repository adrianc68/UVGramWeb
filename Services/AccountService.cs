using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Exceptions;

namespace UVGramWeb.Services;

public class AccountService : IAccountService
{
    private IHttpService httpService;

    public AccountService(IHttpService httpService)
    {
        this.httpService = httpService;
    }

    public async Task<Boolean> VerifyEmailAddress(string email)
    {
        bool isEmailRegistered = false;
        try
        {
            var uri = $"/accounts/email/check/{System.Net.WebUtility.UrlEncode(email)}";
            var data = await httpService.Get(uri);
            if (data != null)
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                isEmailRegistered = json.message.exist;
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isEmailRegistered;
    }

    public async Task<Boolean> VerifyUsername(string username)
    {
        Boolean isUsernameRegistered = false;
        try
        {
            var uri = $"/accounts/username/check/{username}";
            var data = await httpService.Get(uri);
            if (data != null)
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                isUsernameRegistered = json.message.exist;
            }
        }
        catch (Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isUsernameRegistered;
    }

    public async Task<bool> CreateVerificationCode(UserCreateVerification model)
    {
        Boolean isCodeSent = false;
        try
        {
            var uri = $"/accounts/create/verification";
            var data = await httpService.Post(uri, model);
            if (data != null)
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                isCodeSent = json.message;
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isCodeSent;
    }

    public async Task<bool> CreateAccount(UserRegister model)
    {
        Boolean isCreated = false;
        try
        {
            var uri = $"/accounts/create";
            var data = await httpService.Post(uri, model);
            if (data != null)
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                var message = json.message;
                string result = Convert.ToString(message);
                if (result.Contains("New entity was added"))
                {
                    isCreated = true;
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isCreated;
    }

    public async Task<bool> CreateResetConfirmationAddress(UserEmailOrUsername model)
    {
        Boolean isCreated = false;
        try
        {
            var uri = $"/accounts/password/reset/";
            var data = await httpService.Post(uri, model);
            if (data != null)
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                var message = json.message;
                string result = Convert.ToString(message);
                if (result.Contains("a confirmation address has been sent to the new email"))
                {
                    isCreated = true;
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isCreated;
    }
}
