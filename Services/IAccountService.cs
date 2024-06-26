using UVGramWeb.Shared.Data;
namespace UVGramWeb.Services;

public interface IAccountService
{
    Task<Boolean> VerifyUsername(string username);
    Task<Boolean> VerifyEmailAddress(string email);
    Task<MessageType> CreateVerificationCode(UserCreateVerification model);
    Task<MessageType> CreateAccount(UserRegister model);
    Task<MessageType> CreateResetConfirmationAddress(UserEmailOrUsername model);
    Task<MessageType> ChangePasswordByURL(ChangeForgottenPassword model, string uriData);
    Task<Boolean> VerifyURLChangePassword(string uri);
    Task<Boolean> DoesUsernameExist(string username);
    Task<Profile> GetProfile(string username);
    Task<Boolean> FollowUser(string username);
    Task<Boolean> UnfollowUser(string username);
    Task<Boolean> BlockUser(string username);
    Task<Boolean> UnblockUser(string username);
    Task<Boolean> CheckIfUserIsBlockerOrBlocked(string username);
    Task<PersonalUserData> GetAccountPersonalData();
    Task<List<Region>> GetAvailableRegion();
    Task<List<Faculty>> GetAvailableFaculty(int regionId);
    Task<List<EducationalProgram>> GetAvailableEducationalProgram(int facultyId);
    Task<ResultUpdateAccount> UpdatePersonalAccountData(PersonalUserData model);
    Task<MessageType> ChangePassword(ChangeActualPassword model);
    Task<Boolean> ChangePrivacy(ChangePrivacy model);
    Task<List<UserSearch>> FilterUsers(string filter);
    Task<List<UserSearch>> GetFollowers(string username);
    Task<List<UserSearch>> GetFollowed(string username);
    Task<List<UserSearch>> GetPendingFollowerRequest();
    Task<Boolean> AcceptFollowerRequest(string username);
    Task<Boolean> DenyFollowerRequest(string username);
}