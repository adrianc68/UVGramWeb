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

    public async Task<bool> ChangePasswordByURL(ChangeForgottenPassword model, string uriData)
    {
        Boolean isUpdated = false;
        try
        {
            var uri = $"/accounts/password/reset/confirmation?{uriData}";
            var data = await httpService.Post(uri, model);
            if (data != null)
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                var message = json.message.isUpdated;
                isUpdated = Convert.ToBoolean(message);
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isUpdated;
    }

    public async Task<Boolean> VerifyURLChangePassword(string uri)
    {
        bool result = false;
        try
        {
            string resultData = await httpService.Get($"/accounts/verification/url/change_password?{uri}");
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message.redirect;
            result = (message != null);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }

        return result;
    }

    public async Task<bool> DoesUsernameExist(string username)
    {
        bool result = false;
        try
        {
            var uri = $"/accounts/username/check/{username}";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message.exist;
            result = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return result;
    }

    public async Task<Profile> GetProfile(string username)
    {
        Profile profile = null;
        try
        {
            var uri = $"/{username}";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (message != null)
            {
                profile = new Profile();
                profile.name = Convert.ToString(message.name);
                profile.presentation = Convert.ToString(message.presentation);
                profile.username = Convert.ToString(message.username);
                profile.followers = Convert.ToInt32(message.followers);
                profile.followed = Convert.ToInt32(message.followed);
                profile.posts = new List<Post>();
                foreach (var postData in message.posts)
                {
                    Post post = new Post();
                    post.description = Convert.ToString(postData.description);
                    post.comments_allowed = Convert.ToBoolean(postData.comments_allowed);
                    post.likes_allowed = Convert.ToBoolean(postData.likes_allowed);
                    post.uuid = Convert.ToString(postData.uuid);
                    post.likes = Convert.ToInt32(postData.likes);
                    post.comments = Convert.ToInt32(postData.comments);
                    post.files = new List<PostFile>();
                    foreach (var fileData in postData.files)
                    {
                        PostFile postFile = new PostFile();
                        postFile.url = Convert.ToString(fileData.url);
                        post.files.Add(postFile);
                    }
                    profile.posts.Add(post);
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }

        return profile;
    }
}
