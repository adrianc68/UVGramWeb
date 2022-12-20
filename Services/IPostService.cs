using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Models;

namespace UVGramWeb.Services;

public interface IPostService
{
    Task<PostDetails> GetPostDetails(string uuid);
    Task<Boolean> LikeComment(string uuid);
    Task<Boolean> DisLikeComment(string uuid);
    Task<Boolean> LikePost(string uuid);
    Task<Boolean> DislikePost(string uuid);
    Task<Comment> ReplyComment(CreateComment model);
    Task<Comment> CommentPost(CreateComment model);
    Task<List<UserSearch>> GetUsersLikesOfPost(string uuid);
    Task<List<UserSearch>> GetUsersLikesOfComment(string uuid);
}