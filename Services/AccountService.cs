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
                profile.postsCreated = Convert.ToInt32(message.postsCreated);
                profile.privacyType = EnumHelper.GetEnumValue<PrivacyType>(Convert.ToString(json.message.privacyType));
                profile.isFollowed = (message.isFollowed != null) ? Convert.ToBoolean(message.isFollowed) : false;
                profile.isFollowerRequestSent = (message.isFollowerRequestSent != null) ? Convert.ToBoolean(message.isFollowerRequestSent) : false;
                profile.isBlocked = (message.isBlocked != null) ? Convert.ToBoolean(message.isBlocked) : false;
                profile.isBlocker = (message.isBlocker != null) ? Convert.ToBoolean(message.isBlocker) : false;
                profile.isFollower = (message.isFollower != null) ? Convert.ToBoolean(message.isFollower) : false;
                profile.hasSubmittedFollowerRequest = (message.hasSubmittedFollowerRequest != null) ? Convert.ToBoolean(message.hasSubmittedFollowerRequest) : false;
                profile.posts = new List<Post>();
                if (message.posts != null)
                {
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
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
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
            var uri = $"/user/check/block/{username}";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            isUserBlocker = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
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
            personalUserdata.role = EnumHelper.GetEnumValue<RoleType>(Convert.ToString(message.role));
            personalUserdata.privacy = EnumHelper.GetEnumValue<PrivacyType>(Convert.ToString(message.privacy));
            if (personalUserdata.role == RoleType.PERSONAL)
            {
                personalUserdata.gender = EnumHelper.GetEnumValue<GenderType>(Convert.ToString(message.gender));

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
                    objectData.educational_program = Convert.ToString(educationalProgramData.educational_program);
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
                else if (messageEmail.Contains("please wait 5 minutes to generate another confirmation address"))
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

    public async Task<bool> ChangePassword(ChangeActualPassword model)
    {
        bool isChanged = false;
        try
        {
            var uri = "/accounts/password/change";
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
                    userSearch.hasSubmittedFollowerRequest = String.Equals(Convert.ToString(item.status), "PENDIENTE");
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
