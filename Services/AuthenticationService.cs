using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Models;
using Microsoft.AspNetCore.Components;
using UVGramWeb.Shared.Exceptions;
using Microsoft.AspNetCore.Components.Authorization;
using UVGramWeb.Shared.Helpers;

namespace UVGramWeb.Services;

public class AuthenticationService : IAuthenticationService
{
    private IHttpService httpService;
    private ILocalStorageService localStorageService;
    private NavigationManager navigationManager;
    private AuthenticationStateProvider AuthenticationStateProvider;
    private string userKey = "login";
    public User User { get; private set; }

    public AuthenticationService(IHttpService httpService, ILocalStorageService localStorageService, NavigationManager navigationManager, AuthenticationStateProvider AuthenticationStateProvider)
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

    public async Task Login(Login model)
    {
        string data = null;
        try
        {
            data = await httpService.Post("/authentication/login", model);
        }
        catch (Exception error)
        {
            if (error.GetType() == typeof(HttpRequestException))
            {
                throw new InteralServerErrorException("No se ha podido conectar con el servidor");
            }

            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(error.Message);
            if (json.errors != null)
            {
                throw new InvalidInputDataException("Los datos son invalidos");
            }
            else
            {
                string message = json["message"];
                if (message != null)
                {
                    if (message.Contains("user not found"))
                    {
                        throw new UserNotFoundException("El correo electrónico o usuario no existe");
                    }
                    else if (message.Contains("password does not match"))
                    {
                        throw new PasswordDoesNotMatchException("La contraseña no es correcta. Compruébala.");
                    }
                    else if (message.Contains("user has been kicked from server"))
                    {
                        throw new UserKickedFromServerException("El usuario ha sido bloqueado.");
                    }
                }
            }
        }

        try
        {
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            User = new User();
            User.accessToken = json.message.accessToken;
            User.refreshToken = json.message.refreshToken;
            await localStorageService.SetItem(userKey, User);
            await UpdateData();
            ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogInState(User);
            navigationManager.NavigateTo("/");
        }
        catch (Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
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
        try
        {
            var data = await httpService.Get("/accounts/data");
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            User.Username = json.message.username;
            User.RoleType = EnumHelper.GetEnumValue<RoleType>(Convert.ToString(json.message.role));
            await localStorageService.SetItem(userKey, User);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
    }

}