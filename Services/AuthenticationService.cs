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
            // object message = BackendMessageHandler.GetMessageFromJson<LoginResponse>(data);
            ApiResponse<object> message2 = BackendMessageHandler.GetMessageFromJson2<LoginResponse>(
                data
            );

           if(message2.Status != (int)HttpStatusCode.OK) {
                CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse) message2.Data;
                result = codeMessageData.Code;
           }


            // if (message is ApiResponse<LoginResponse>)
            // {
            //     ApiResponse<LoginResponse> loginResponse = (ApiResponse<LoginResponse>)message;
            //     User = new User
            //     {
            //         AccessToken = loginResponse.Data.AccessToken,
            //         RefreshToken = loginResponse.Data.RefreshToken
            //     };
            //     await localStorageService.SetItem(userKey, User);
            //     await UpdateData();
            //     result = MessageType.OK;
            // }
            // else
            // {
            //     result = (MessageType)message;
            // }
        }
        catch (Exception error)
        {
            MessageType resultError = BackendMessageHandler.GetErrorMessage(error);
            throw new Exception(resultError.ToString(), error);
        }
        return result;
    }

    public async Task Logout()
    {
        try
        {
            var data = await httpService.Post("/authentication/logout", null);
            User = null;
        }
        catch (Exception error)
        {
            await localStorageService.RemoveItem("login");
            ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogOutState();
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        await localStorageService.RemoveItem("login");
        ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogOutState();
        navigationManager.NavigateTo("/");
    }

    public async Task UpdateData()
    {
        // try
        // {
        //     var data = await httpService.Get("/accounts/data");
        //     object result = BackendMessageHandler.GetMessageFromJson<AccountDataResponse>(data);
        //     if (result is ApiResponse<AccountDataResponse>)
        //     {

        //         User.Username = result.data.username;
        //         User.RoleType = EnumHelper.GetEnumValue<RoleType>(
        //             Convert.ToString(json.message.role)
        //         );
        //         await localStorageService.SetItem(userKey, User);
        //     }

        //     dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
        // }
        // catch (System.Exception error)
        // {
        //     throw new InteralServerErrorException("El servidor ha tenido un error", error);
        // }
    }

    public void NotifyUserLoginChange()
    {
        ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogInState(User);
    }
}
