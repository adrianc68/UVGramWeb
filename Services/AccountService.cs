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
            Console.WriteLine(error);
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isUsernameRegistered;
    }
}
