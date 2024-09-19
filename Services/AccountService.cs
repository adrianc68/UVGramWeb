using System.Net;
using System.Net.Http.Headers;
using UVGramWeb.Helpers;
using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Exceptions;

namespace UVGramWeb.Services;

public class AccountService : IAccountService
{
  private IHttpService httpService;
  private IAuthenticationService authenticationService;
  public event Action OnUserDataChanged;

  public AccountService(IHttpService httpService, IAuthenticationService authenticationService)
  {
    this.httpService = httpService;
    this.authenticationService = authenticationService;
  }

  public async Task<Boolean> VerifyEmailAddress(string email)
  {
    bool isEmailRegistered = false;
    try
    {
      string uri = $"/accounts/email/check/{System.Net.WebUtility.UrlEncode(email)}";
      var data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isEmailRegistered = codeMessageDataResponse.BoolValue;
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
      string uri = $"/accounts/username/check/{username}";
      var data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);

      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isUsernameRegistered = codeMessageDataResponse.BoolValue;
      }
    }
    catch (Exception error)
    {
      throw new InteralServerErrorException("El servidor ha tenido un error", error);
    }
    return isUsernameRegistered;
  }

  public async Task<MessageType> CreateVerificationCode(UserCreateVerification model)
  {
    MessageType result = MessageType.NONE;
    try
    {
      string uri = $"/accounts/create/verification";
      var data = await httpService.Post(uri, model);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
      result = codeMessageData.Code;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<MessageType> CreateAccount(UserRegister model)
  {
    MessageType result = MessageType.NONE;
    try
    {
      string uri = $"/accounts/create";
      var data = await httpService.Post(uri, model);
      string datos = data.ToString();
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
      result = codeMessageData.Code;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<MessageType> CreateResetConfirmationAddress(UserEmailOrUsername model)
  {
    MessageType result = MessageType.NONE;
    try
    {
      string uri = $"/accounts/password/reset";
      var data = await httpService.Post(uri, model);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
      result = codeMessageData.Code;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<MessageType> ChangePasswordByURL(
      ChangeForgottenPassword model,
      string uriData
  )
  {
    MessageType result = MessageType.NONE;
    try
    {
      var uri = $"/accounts/password/reset/confirmation?{uriData}";
      var data = await httpService.Post(uri, model);

      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
      result = codeMessageData.Code;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<Boolean> VerifyURLChangePassword(string uri)
  {
    bool result = false;
    try
    {
      string url = $"/accounts/verification/url/change_password?{uri}";
      string data = await httpService.Get(url);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        result = true;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<bool> DoesUsernameExist(string username)
  {
    bool result = false;
    try
    {
      string uri = $"/accounts/username/check/{username}";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        result = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<Profile> GetProfile(string username)
  {
    Profile profile = null;
    try
    {
      string uri = $"/{username}";
      string resultData = await httpService.Get(uri);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<Profile>(
          resultData
      );
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        profile = (Profile)apiResponse.Data;
        profile.url = ConfigHelper.SetResourcesApiBaseUrl(profile.url);
        if (profile.posts != null)
        {

          profile.posts = profile.posts.OrderByDescending(post => post.created_time).ToList();

          foreach (var post in profile.posts)
          {
            foreach (var file in post.files)
            {
              file.url = ConfigHelper.SetResourcesApiBaseUrl(file.url);
            }
          }
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return profile;
  }

  public async Task<bool> FollowUser(string username)
  {
    bool isFollowed = false;
    try
    {
      string uri = "/user/follow/";
      Username usernameModel = new Username();
      usernameModel.username = username;
      string data = await httpService.Post(uri, usernameModel);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isFollowed = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isFollowed;
  }

  public async Task<bool> UnfollowUser(string username)
  {
    bool isUnfollowed = false;
    try
    {
      string uri = "/user/unfollow/";
      Username usernameModel = new Username();
      usernameModel.username = username;
      string data = await httpService.Post(uri, usernameModel);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isUnfollowed = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isUnfollowed;
  }

  public async Task<bool> BlockUser(string username)
  {
    bool isBlocked = false;
    try
    {
      string uri = "/user/block/";
      Username usernameModel = new Username();
      usernameModel.username = username;
      string data = await httpService.Post(uri, usernameModel);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isBlocked = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isBlocked;
  }

  public async Task<bool> UnblockUser(string username)
  {
    bool isUnblocked = false;
    try
    {
      string uri = "/user/unblock/";
      Username usernameModel = new Username();
      usernameModel.username = username;
      string data = await httpService.Post(uri, usernameModel);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isUnblocked = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isUnblocked;
  }

  public async Task<bool> CheckIfUserIsBlockerOrBlocked(string username)
  {
    bool isUserBlocker = false;
    try
    {
      string uri = $"/user/check/block/{username}";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
            apiResponse.Data;
        isUserBlocker = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isUserBlocker;
  }

  public async Task<PersonalUserData> GetAccountPersonalData()
  {
    PersonalUserData userData = null;
    try
    {
      var uri = "/accounts/data";
      string data = await httpService.Get(uri);

      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<PersonalUserDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        PersonalUserDataResponse personalUserDataResponse = (PersonalUserDataResponse)apiResponse.Data;
        PersonalUserData personalData = new()
        {
          name = personalUserDataResponse.Name,
          presentation = personalUserDataResponse.Presentation,
          username = personalUserDataResponse.Username,
          email = personalUserDataResponse.Email,
          url = ConfigHelper.SetResourcesApiBaseUrl(personalUserDataResponse.Url),
          phoneNumber = personalUserDataResponse.Phone_Number,
          birthdate = personalUserDataResponse.Birthday,
          role = personalUserDataResponse.Role,
          privacy = personalUserDataResponse.Privacy,
          Gender = personalUserDataResponse.Gender
        };
        userData = personalData;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return userData;
  }

  public async Task<List<Region>> GetAvailableRegion()
  {
    List<Region> regions = new();
    try
    {
      string uri = "/data/region/";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<RegionListDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        RegionListDataResponse regionListDataResponse = (RegionListDataResponse)
            apiResponse.Data;
        foreach (var region in regionListDataResponse.Regions)
        {
          regions.Add(region);
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return regions;
  }

  public async Task<List<Faculty>> GetAvailableFaculty(int regionId)
  {
    List<Faculty> faculties = new();
    try
    {
      string uri = "/data/faculty/";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<FacultyListDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        FacultyListDataResponse facultyListDataResponse = (FacultyListDataResponse)
            apiResponse.Data;
        foreach (var faculty in facultyListDataResponse.Faculties)
        {
          faculties.Add(faculty);
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return faculties;
  }

  public async Task<List<EducationalProgram>> GetAvailableEducationalProgram(int facultyId)
  {
    List<EducationalProgram> educationalPrograms = new();
    try
    {
      string uri = "/data/educationalprogram/";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<EducationalProgramListDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        EducationalProgramListDataResponse educationalProgramListDataResponse =
            (EducationalProgramListDataResponse)apiResponse.Data;
        foreach (
            var educationalProgram in educationalProgramListDataResponse.EducationalPrograms
        )
        {
          educationalPrograms.Add(educationalProgram);
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return educationalPrograms;
  }

  public async Task<object> UpdatePersonalAccountData(PersonalUserData model)
  {
    object result = null;
    try
    {
      UpdatePersonalUserData modelRequest = new()
      {
        name = model.name,
        presentation = model.presentation,
        username = model.username,
        phoneNumber = model.phoneNumber,
        email = model.email,
        birthdate = model.birthdate,
        gender = model.Gender.ToString(),
      };
      string uri = $"/accounts/edit/personal";
      string data = await httpService.Patch(uri, modelRequest);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<UpdateAccountDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        UpdateAccountDataResponse updateAccountDataResponse = (UpdateAccountDataResponse)apiResponse.Data;
        result = updateAccountDataResponse;
      }
      else
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)apiResponse.Data;
        result = codeMessageDataResponse;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<MessageType> ChangePassword(ChangeActualPassword model)
  {
    MessageType result = MessageType.NONE;
    try
    {
      string uri = "/accounts/password/change";
      string data = await httpService.Post(uri, model);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
          apiResponse.Data;
      result = codeMessageDataResponse.Code;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<MessageType> ChangePrivacy(ChangePrivacy model)
  {
    MessageType messageType = MessageType.NONE;
    try
    {
      var uri = "/accounts/users/change-privacy/";
      string data = await httpService.Post(uri, model);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)apiResponse.Data;
      messageType = codeMessageDataResponse.Code;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return messageType;
  }

  public async Task<List<UserSearch>> FilterUsers(string filter)
  {
    List<UserSearch> users = new List<UserSearch>();
    try
    {
      string uri = $"/users/{filter}";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<UsersListDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        UsersListDataResponse usersListDataResponse = (UsersListDataResponse)
            apiResponse.Data;
        if (usersListDataResponse.Users != null)
        {
          foreach (var user in usersListDataResponse.Users)
          {
            user.url = ConfigHelper.SetResourcesApiBaseUrl(user.url);
            users.Add(user);
          }
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return users;
  }

  public async Task<List<UserSearch>> GetFollowers(string username)
  {
    List<UserSearch> users = new List<UserSearch>();
    try
    {
      string uri = $"/user/followers-of/{username}";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<UsersListDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        UsersListDataResponse userFollowersDataResponse = (UsersListDataResponse)
            apiResponse.Data;
        if (userFollowersDataResponse.Users != null)
        {
          foreach (var user in userFollowersDataResponse.Users)
          {
            user.url = ConfigHelper.SetResourcesApiBaseUrl(user.url);
            users.Add(user);
          }
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return users;
  }

  public async Task<List<UserSearch>> GetFollowed(string username)
  {
    List<UserSearch> users = new List<UserSearch>();
    try
    {
      string uri = $"/user/followed-by/{username}";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse =
          BackendMessageHandler.GetMessageFromJson<UsersListDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        UsersListDataResponse userFollowersDataResponse = (UsersListDataResponse)
            apiResponse.Data;
        if (userFollowersDataResponse.Users != null)
        {
          foreach (var user in userFollowersDataResponse.Users)
          {
            user.url = ConfigHelper.SetResourcesApiBaseUrl(user.url);
            users.Add(user);
          }
        }
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return users;
  }

  public async Task<List<UserSearch>> GetPendingFollowerRequest()
  {
    List<UserSearch> users = new List<UserSearch>();
    try
    {
      var uri = $"/user/followers/pending/";
      string resultData = await httpService.Get(uri);
      dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
      var message = json.message;
      if (message != null)
      {
        foreach (var item in message)
        {
          UserSearch userSearch = new UserSearch();
          userSearch.username = Convert.ToString(item.username);
          userSearch.name = Convert.ToString(item.name);
          userSearch.url = ConfigHelper.SetResourcesApiBaseUrl(userSearch.url);
          userSearch.hasSubmittedFollowerRequest = String.Equals(
              Convert.ToString(item.status),
              "PENDIENTE"
          );
          users.Add(userSearch);
        }
      }
    }
    catch (System.Exception error)
    {
      throw new InteralServerErrorException("El servidor ha tenido un error", error);
    }
    return users;
  }

  public async Task<bool> AcceptFollowerRequest(string username)
  {
    bool result = false;
    try
    {
      string uri = "/user/followers/accept/";
      Username usernameModel = new();
      usernameModel.username = username;
      string data = await httpService.Post(uri, usernameModel);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)apiResponse.Data;
        result = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<bool> DenyFollowerRequest(string username)
  {
    bool result = false;
    try
    {
      string uri = "/user/followers/deny/";
      Username usernameModel = new();
      usernameModel.username = username;
      string data = await httpService.Post(uri, usernameModel);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)apiResponse.Data;
        result = codeMessageDataResponse.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<string> UpdateProfileImage(Stream fileStream, string fileName, string contentType)
  {
    string result = null;
    try
    {
      var content = new MultipartFormDataContent();
      var fileContent = new StreamContent(fileStream);
      fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
      content.Add(fileContent, "file", fileName);
      string uri = "/accounts/edit/image/";
      string data = await httpService.Patch(uri, content);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<UpdateImageProfileResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        UpdateImageProfileResponse objectResult = (UpdateImageProfileResponse)apiResponse.Data;
        result = ConfigHelper.SetResourcesApiBaseUrl(objectResult.Url);
        authenticationService.User.Url = result;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return result;
  }

  public async Task<Image> GetImageResource(string uri)
  {
    Image image;
    try
    {
      (byte[] responseBytes, Dictionary<string, IEnumerable<string>> responseHeaders) = await httpService.GetBytes(uri);
      string contentType = "image/jpeg";
      if (responseHeaders.ContainsKey("Content-Type"))
      {
        contentType = responseHeaders["Content-Type"].FirstOrDefault();
      }
      image = new()
      {
        Data = responseBytes,
        ContentType = contentType
      };
      return image;
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
  }

  public void NotifyUserProfileChange()
  {
    OnUserDataChanged?.Invoke();
  }

}
