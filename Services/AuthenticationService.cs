using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Exceptions;
using UVGramWeb.Shared.Helpers;
using UVGramWeb.Shared.Models;

namespace UVGramWeb.Services;

public class AuthenticationService : IAuthenticationService
{
    private IHttpService httpService;
    private ILocalStorageService localStorageService;
    private NavigationManager navigationManager;
    private AuthenticationStateProvider AuthenticationStateProvider;
    private string userKey = "login";
    public User User { get; private set; }

    public AuthenticationService(
        IHttpService httpService,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager,
        AuthenticationStateProvider AuthenticationStateProvider
    )
    {
        this.httpService = httpService;
        this.localStorageService = localStorageService;
        this.navigationManager = navigationManager;
        this.AuthenticationStateProvider = AuthenticationStateProvider;
    }

    public async Task Initialize()
    {
        User = await localStorageService.GetItem<User>(userKey);
    }

    public async Task<MessageType> Login(Login model)
    {
        MessageType result = MessageType.NONE;
        try
        {
            string data = await httpService.Post("/authentication/login", model);
            ApiResponse<object> message =
                BackendMessageHandler.GetMessageFromJson<LoginDataResponse>(data);
            if (message.Status != (int)HttpStatusCode.OK)
            {
                CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)message.Data;
                result = codeMessageData.Code;
            }
            else
            {
                LoginDataResponse LoginDataResponse = (LoginDataResponse)message.Data;
                User = new User
                {
                    AccessToken = LoginDataResponse.AccessToken,
                    RefreshToken = LoginDataResponse.RefreshToken
                };
                await localStorageService.SetItem(userKey, User);
                await UpdateData();
                result = MessageType.OK;
            }
        }
        catch (Exception error)
        {
            string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
            throw new Exception(ErrorMessage, error);
        }
        return result;
    }

    public async Task Logout()
    {
        try
        {
            string data = await httpService.Post("/authentication/logout", null);
            User = null;
        }
        catch (Exception error)
        {
            await localStorageService.RemoveItem("login");
            ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogOutState();
            string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
            throw new Exception(ErrorMessage, error);
        }
        await localStorageService.RemoveItem("login");
        ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogOutState();
        navigationManager.NavigateTo("/");
    }

    public async Task UpdateData()
    {
        try
        {
            var data = await httpService.Get("/accounts/data");
            ApiResponse<object> message =
                BackendMessageHandler.GetMessageFromJson<AccountDataResponse>(data);
            if (message.Status == (int)HttpStatusCode.OK)
            {
                AccountDataResponse accountDataResponse = (AccountDataResponse)message.Data;
                User.Username = accountDataResponse.Username;
                User.RoleType = EnumHelper.GetEnumValue<RoleType>(accountDataResponse.Role);
                await localStorageService.SetItem(userKey, User);
            } else {
             throw new InteralServerErrorException(MessageType.API_ERROR.ToString());
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException(MessageType.INTERNAL_ERROR.ToString(), error);
        }
    }

    public void NotifyUserLoginChange()
    {
        ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogInState(User);
    }
}
