using System.Net;
using System.Numerics;
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
                BackendMessageHandler.GetMessageFromJson<IsRegisteredDataResponse>(data);

            if (apiResponse.Status == (int)HttpStatusCode.OK)
            {
                IsRegisteredDataResponse dataRegisteredResponse = (IsRegisteredDataResponse)
                    apiResponse.Data;
                isEmailRegistered = dataRegisteredResponse.isRegistered;
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
                BackendMessageHandler.GetMessageFromJson<IsRegisteredDataResponse>(data);

            if (apiResponse.Status == (int)HttpStatusCode.OK)
            {
                IsRegisteredDataResponse dataRegisteredResponse = (IsRegisteredDataResponse)
                    apiResponse.Data;
                isUsernameRegistered = dataRegisteredResponse.isRegistered;
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
                BackendMessageHandler.GetMessageFromJson<IsRegisteredDataResponse>(data);
            if (apiResponse.Status != (int)HttpStatusCode.OK)
            {
                CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
                result = codeMessageData.Code;
            }
            else
            {
                result = MessageType.OK;
            }
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
            ApiResponse<object> apiResponse =
                BackendMessageHandler.GetMessageFromJson<IsRegisteredDataResponse>(data);
            if (apiResponse.Status != (int)HttpStatusCode.OK)
            {
                CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
                result = codeMessageData.Code;
            }
            else
            {
                result = MessageType.OK;
            }
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
                BackendMessageHandler.GetMessageFromJson<IsUpdatedDataResponse>(data);
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
            ApiResponse<object> apiResponse =
                BackendMessageHandler.GetMessageFromJson<Profile>(resultData);
            if (apiResponse.Status == (int)HttpStatusCode.OK)
            {
             profile = (Profile) apiResponse.Data;
                // ProfileDataResponse profileDataResponse = (ProfileDataResponse)apiResponse.Data;
                // profile = new Profile
                // {
                //     name = profileDataResponse.Name,
                //     presentation = profileDataResponse.Presentation,
                //     username = profileDataResponse.Username,
                //     followers = profileDataResponse.Followers,
                //     followed = profileDataResponse.Followers,
                //     postsCreated = profileDataResponse.PostsCreated,
                //     privacyType = EnumHelper.GetEnumValue<PrivacyType>(
                //         profileDataResponse.PrivacyType
                //     ),
                //     isFollowed = profileDataResponse.IsFollowed,
                //     isFollowerRequestSent = profileDataResponse.IsFollowerRequestSent,
                //     isBlocked = profileDataResponse.IsBlocked,
                //     isBlocker = profileDataResponse.IsBlocker,
                //     isFollower = profileDataResponse.IsFollower,
                //     hasSubmittedFollowerRequest = profileDataResponse.HasSubmittedFollowerRequest,
                //     posts = new List<Post>()
                // };
                // foreach (var post in profileDataResponse.Posts)
                // {
                //     profile.posts.Add(post);
                // }
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
            var uri = "/user/follow/";
            Username usernameModel = new Username();
            usernameModel.username = username;
            string resultData = await httpService.Post(uri, usernameModel);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (Convert.ToString(message).Contains("you are now following to"))
            {
                isFollowed = true;
            }
            else if (Convert.ToString(message).Contains("follower request sent to"))
            {
                isFollowed = true;
            }
        }
        catch (System.Exception error)
        {
            Console.WriteLine(error);
            return isFollowed;
            // throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isFollowed;
    }

    public async Task<bool> UnfollowUser(string username)
    {
        bool isUnfollowed = false;
        try
        {
            var uri = "/user/unfollow/";
            Username usernameModel = new Username();
            usernameModel.username = username;
            string resultData = await httpService.Post(uri, usernameModel);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (Convert.ToString(message).Contains("you have unfollowed to"))
            {
                isUnfollowed = true;
            }
        }
        catch (System.Exception error)
        {
            Console.WriteLine(error);
            return isUnfollowed;
            // throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isUnfollowed;
    }

    public async Task<bool> BlockUser(string username)
    {
        bool isBlocked = false;
        try
        {
            var uri = "/user/block/";
            Username usernameModel = new Username();
            usernameModel.username = username;
            string resultData = await httpService.Post(uri, usernameModel);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (Convert.ToString(message).Contains("you have blocked to"))
            {
                isBlocked = true;
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isBlocked;
    }

    public async Task<bool> UnblockUser(string username)
    {
        bool isUnblocked = false;
        try
        {
            var uri = "/user/unblock/";
            Username usernameModel = new Username();
            usernameModel.username = username;
            string resultData = await httpService.Post(uri, usernameModel);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (Convert.ToString(message).Contains("you have unblocked to"))
            {
                isUnblocked = true;
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isUnblocked;
    }

    public async Task<bool> CheckIfUserIsBlocker(string username)
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

    public async Task<UserData> GetAccountData()
    {
        PersonalUserData personalUserdata = null;
        try
        {
            var uri = "/accounts/data";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            personalUserdata = new PersonalUserData();
            personalUserdata.name = Convert.ToString(message.name);
            personalUserdata.presentation = Convert.ToString(message.presentation);
            personalUserdata.username = Convert.ToString(message.username);
            personalUserdata.email = Convert.ToString(message.email);
            personalUserdata.phoneNumber = Convert.ToString(message.phone_number);
            personalUserdata.birthdate = Convert.ToString(message.birthday);
            personalUserdata.role = EnumHelper.GetEnumValue<RoleType>(
                Convert.ToString(message.role)
            );
            personalUserdata.privacy = EnumHelper.GetEnumValue<PrivacyType>(
                Convert.ToString(message.privacy)
            );
            if (personalUserdata.role == RoleType.PERSONAL)
            {
                personalUserdata.gender = EnumHelper.GetEnumValue<GenderType>(
                    Convert.ToString(message.gender)
                );

                if (message.id_educational_program != null)
                {
                    Region region = new Region();
                    region.id = Convert.ToInt32(message.id_region);
                    Faculty faculty = new Faculty();
                    faculty.id = Convert.ToInt32(message.id_faculty);
                    faculty.region = region;
                    EducationalProgram educationalProgram = new EducationalProgram();
                    educationalProgram.id = Convert.ToInt32(message.id_educational_program);
                    educationalProgram.faculty = faculty;
                    personalUserdata.educational_program = educationalProgram;
                }
                else
                {
                    Region region = new Region();
                    Faculty faculty = new Faculty();
                    faculty.region = region;
                    EducationalProgram educationalProgram = new EducationalProgram();
                    educationalProgram.faculty = faculty;
                    personalUserdata.educational_program = educationalProgram;
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return personalUserdata;
    }

    public async Task<List<Region>> GetAvailableRegion()
    {
        List<Region> regions = new List<Region>();
        try
        {
            var uri = "/data/region/";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var messages = json.message;
            foreach (var regionData in messages)
            {
                Region region = new Region();
                region.id = Convert.ToInt32(regionData.id);
                region.region = Convert.ToString(regionData.region);
                regions.Add(region);
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return regions;
    }

    public async Task<List<Faculty>> GetAvailableFaculty(int regionId)
    {
        List<Faculty> faculties = new List<Faculty>();
        try
        {
            var uri = "/data/faculty/";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var messages = json.message;
            foreach (var facultyData in messages)
            {
                if (regionId == Convert.ToInt32(facultyData.id_region))
                {
                    Faculty objectData = new Faculty();
                    objectData.id = Convert.ToInt32(facultyData.id);
                    objectData.faculty = Convert.ToString(facultyData.faculty);
                    faculties.Add(objectData);
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return faculties;
    }

    public async Task<List<EducationalProgram>> GetAvailableEducationalProgram(int facultyId)
    {
        List<EducationalProgram> educationalPrograms = new List<EducationalProgram>();
        try
        {
            var uri = "/data/educationalprogram/";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var messages = json.message;
            foreach (var educationalProgramData in messages)
            {
                if (facultyId == Convert.ToInt32(educationalProgramData.id_faculty))
                {
                    EducationalProgram objectData = new EducationalProgram();
                    objectData.id = Convert.ToInt32(educationalProgramData.id);
                    objectData.educational_program = Convert.ToString(
                        educationalProgramData.educational_program
                    );
                    educationalPrograms.Add(objectData);
                }
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
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
            modelRequest.gender = model.gender.ToString();
            modelRequest.idCareer = model.educational_program.id.ToString();
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
                BackendMessageHandler.GetMessageFromJson<IsUpdatedDataResponse>(data);
            if (apiResponse.Status != (int)HttpStatusCode.OK)
            {
                CodeMessageDataResponse codeMessageDataResponse = (CodeMessageDataResponse)
                    apiResponse.Data;
                result = codeMessageDataResponse.Code;
            }
            else
            {
                result = MessageType.OK;
            }
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
            var uri = $"/users/{filter}";
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

    public async Task<List<UserSearch>> GetFollowers(string username)
    {
        List<UserSearch> users = new List<UserSearch>();
        try
        {
            var uri = $"/user/followers-of/{username}";
            string data = await httpService.Get(uri);
            // ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<>(data);



            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            if (message != null)
            {
                foreach (var item in message)
                {
                    UserSearch userSearch = new UserSearch();
                    userSearch.username = Convert.ToString(item.username);
                    userSearch.name = Convert.ToString(item.name);
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

    public async Task<List<UserSearch>> GetFollowed(string username)
    {
        List<UserSearch> users = new List<UserSearch>();
        try
        {
            var uri = $"/user/followed-by/{username}";
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
