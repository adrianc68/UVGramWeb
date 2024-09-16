using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using UVGramWeb.Helpers;
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
  public UserAuthentication User { get; private set; }
  public event Action OnUserDataChanged;

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
    User = await localStorageService.GetItem<UserAuthentication>(userKey);
  }

  public async Task<MessageType> Login(Login model)
  {
    MessageType result = MessageType.NONE;
    try
    {
      string data = await httpService.Post("/authentication/login", model);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<UserAuthentication>(data);
      if (apiResponse.Status != (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
        result = codeMessageData.Code;
      }
      else
      {
        User = (UserAuthentication)apiResponse.Data;
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
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<AccountDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        AccountDataResponse accountDataResponse = (AccountDataResponse)apiResponse.Data;
        User.Username = accountDataResponse.Username;
        User.RoleType = EnumHelper.GetEnumValue<RoleType>(accountDataResponse.Role);
        User.Url = ConfigHelper.SetResourcesApiBaseUrl(accountDataResponse.Url);
        await localStorageService.SetItem(userKey, User);
        NotifyUserDataChanged();
      }
      else
      {
        throw new InteralServerErrorException(MessageType.API_ERROR.ToString());
      }
    }
    catch (System.Exception error)
    {
      throw new InteralServerErrorException(MessageType.INTERNAL_ERROR.ToString(), error);
    }
  }

  public void NotifyUserDataChanged()
  {
    OnUserDataChanged?.Invoke();
  }

  public void NotifyUserLoginChange()
  {
    ((ApiAuthenticationStateProvider)AuthenticationStateProvider).NewUserLogInState(User);
  }
}
