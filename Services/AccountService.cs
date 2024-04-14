using System.Net;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Exceptions;
using UVGramWeb.Shared.Helpers;

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
                BackendMessageHandler.GetMessageFromJson<PersonalUserData>(data);
            if (apiResponse.Status == (int)HttpStatusCode.OK)
            {
                PersonalUserData personalData = (PersonalUserData)apiResponse.Data;
                userData = personalData;
                if (personalData.Educational_program == null)
                {
                    Region region = new();
                    Faculty faculty = new() { region = region };
                    EducationalProgram educationalProgram = new() { faculty = faculty };
                    personalData.Educational_program = educationalProgram;
                }
            }
            //     if (message.id_Educational_program != null)
            //     {
            //         Region region = new Region();
            //         region.id = Convert.ToInt32(message.id_region);
            //         Faculty faculty = new Faculty();
            //         faculty.id = Convert.ToInt32(message.id_faculty);
            //         faculty.region = region;
            //         EducationalProgram educationalProgram = new EducationalProgram();
            //         educationalProgram.id = Convert.ToInt32(message.id_Educational_program);
            //         educationalProgram.faculty = faculty;
            //         userData.Educational_program = educationalProgram;
            //     }
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

    public async Task<ResultUpdateAccount> UpdatePersonalAccountData(PersonalUserData model)
    {
        ResultUpdateAccount result = new ResultUpdateAccount();
        try
        {
            UpdatePersonalUserData modelRequest = new UpdatePersonalUserData();
            modelRequest.name = model.name;
            modelRequest.presentation = model.presentation;
            modelRequest.username = model.username;
            modelRequest.phoneNumber = model.phoneNumber;
            modelRequest.email = model.email;
            modelRequest.birthdate = model.birthdate;
            modelRequest.Gender = model.Gender.ToString();
            modelRequest.idCareer = model.Educational_program.id.ToString();
            var uri = $"/accounts/edit/personal";
            string resultData = await httpService.Patch(uri, modelRequest);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            result.IsUpdated = Convert.ToBoolean(message.isUpdated);
            if (message.emailMessage != null)
            {
                string messageEmail = Convert.ToString(message.emailMessage);
                if (messageEmail.Contains("a confirmation address has been sent to the new email"))
                {
                    result.IsEmailUpdated = true;
                }
                else if (
                    messageEmail.Contains(
                        "please wait 5 minutes to generate another confirmation address"
                    )
                )
                {
                    result.IsEmailConfirmationAlreadySent = true;
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
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

    public async Task<bool> ChangePrivacy(ChangePrivacy model)
    {
        bool isChanged = false;
        try
        {
            var uri = "/accounts/users/change-privacy/";
            string resultData = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            isChanged = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isChanged;
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
                foreach (var user in usersListDataResponse.Users)
                {
                    users.Add(user);
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
                foreach (var user in userFollowersDataResponse.Users)
                {
                    users.Add(user);
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
                foreach (var user in userFollowersDataResponse.Users)
                {
                    users.Add(user);
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
        bool isAccepted = false;
        try
        {
            var uri = "/user/followers/accept/";
            Username usernameModel = new Username();
            usernameModel.username = username;
            string resultData = await httpService.Post(uri, usernameModel);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            isAccepted = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isAccepted;
    }

    public async Task<bool> DenyFollowerRequest(string username)
    {
        bool isRejected = false;
        try
        {
            var uri = "/user/followers/deny/";
            Username usernameModel = new Username();
            usernameModel.username = username;
            string resultData = await httpService.Post(uri, usernameModel);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            isRejected = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isRejected;
    }
}
