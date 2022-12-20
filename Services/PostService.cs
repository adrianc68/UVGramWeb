using System.Globalization;
using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Exceptions;
using UVGramWeb.Shared.Models;

namespace UVGramWeb.Services;

public class PostService : IPostService
{
    private IHttpService httpService;

    public PostService(IHttpService httpService)
    {
        this.httpService = httpService;
    }

    public async Task<PostDetails> GetPostDetails(string uuid)
    {
        PostDetails post = null;
        try
        {
            var uri = $"/post/details/{uuid}";
            var data = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            if (message != null)
            {
                post = new PostDetails();
                post.description = Convert.ToString(message.post.description);
                post.comments_allowed = Convert.ToBoolean(message.post.comments_allowed);
                post.likes_allowed = Convert.ToBoolean(message.post.likes_allowed);
                post.uuid = Convert.ToString(message.post.uuid);
                post.isLiked = Convert.ToBoolean(message.post.isLiked);
                post.likes = Convert.ToInt32(message.likes);
                List<Comment> comments = new List<Comment>();
                foreach (var item in message.comments)
                {
                    Comment comment = new Comment();
                    comment.comment = Convert.ToString(item.comment);
                    DateTime timeUtc = Convert.ToDateTime(item.created_time);
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
                    DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                    comment.created_time = cstTime.ToString();
                    comment.uuid = Convert.ToString(item.uuid);
                    comment.username = Convert.ToString(item.username);
                    comment.likes = Convert.ToInt32(item.likes);
                    comment.isLiked = Convert.ToBoolean(item.isLiked);
                    comment.replies = new List<Comment>();
                    foreach (var reply in item.replies)
                    {
                        Comment replyObject = new Comment();
                        replyObject.comment = Convert.ToString(reply.comment);
                        DateTime timeUtcAux = Convert.ToDateTime(reply.created_time);
                        TimeZoneInfo cstZoneAux = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
                        DateTime cstTimeAux = TimeZoneInfo.ConvertTimeFromUtc(timeUtcAux, cstZoneAux);
                        replyObject.created_time = cstTime.ToString();
                        replyObject.uuid = Convert.ToString(reply.uuid);
                        replyObject.username = Convert.ToString(reply.username);
                        replyObject.likes = Convert.ToInt32(reply.likes);
                        replyObject.isLiked = Convert.ToBoolean(reply.isLiked);
                        replyObject.parent_comment = comment;
                        comment.replies.Add(replyObject);
                    }
                    comments.Add(comment);
                }
                post.comments = comments;
                List<string> files = new List<string>();
                foreach (var item in message.files)
                {
                    files.Add(Convert.ToString(item));
                }
                post.files = files;
                UserSearch ownerData = new UserSearch();
                ownerData.name = Convert.ToString(message.owner.name);
                ownerData.username = Convert.ToString(message.owner.username);
                post.owner = ownerData;
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return post;
    }

    public async Task<bool> LikeComment(string uuid)
    {
        bool isLiked = false;
        try
        {
            Uuid model = new Uuid();
            model.uuid = uuid;
            var uri = "/post/comment/like/";
            var data = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            isLiked = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isLiked;
    }

    public async Task<bool> DisLikeComment(string uuid)
    {
        bool isDisliked = false;
        try
        {
            Uuid model = new Uuid();
            model.uuid = uuid;
            var uri = "/post/comment/dislike/";
            var data = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            isDisliked = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isDisliked;
    }

    public async Task<Comment> ReplyComment(CreateComment model)
    {
        Comment reply = null;
        try
        {
            var uri = "/post/comment/reply";
            var data = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            if (Convert.ToBoolean(message.isCreated))
            {
                var commentData = message.commentDetails;
                reply = new Comment();
                reply.comment = Convert.ToString(commentData.comment);
                DateTime timeUtc = Convert.ToDateTime(commentData.created_time);
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                reply.isReplyInnerComment = Convert.ToBoolean(commentData.isReplyInnerComment);
                reply.created_time = cstTime.ToString();
                reply.uuid = Convert.ToString(commentData.uuid);
                reply.likes = 0;
                reply.isLiked = false;
                reply.replies = new List<Comment>();
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return reply;
    }

    public async Task<Comment> CommentPost(CreateComment model)
    {
        Comment commentCreated = null;
        try
        {
            var uri = "/post/comment/create/";
            var data = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            if (Convert.ToBoolean(message.isCreated))
            {
                var commentData = message.commentDetails;
                commentCreated = new Comment();
                commentCreated.comment = Convert.ToString(commentData.comment);
                DateTime timeUtc = Convert.ToDateTime(commentData.created_time);
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                commentCreated.isReplyInnerComment = Convert.ToBoolean(commentData.isReplyInnerComment);
                commentCreated.created_time = cstTime.ToString();
                commentCreated.uuid = Convert.ToString(commentData.uuid);
                commentCreated.likes = 0;
                commentCreated.isLiked = false;
                commentCreated.replies = new List<Comment>();
            }
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return commentCreated;
    }

    public async Task<bool> LikePost(string uuid)
    {
        bool isLiked = false;
        try
        {
            Uuid model = new Uuid();
            model.uuid = uuid;
            var uri = "/post/like/";
            var data = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            isLiked = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isLiked;
    }

    public async Task<bool> DislikePost(string uuid)
    {
        bool isDisliked = false;
        try
        {
            Uuid model = new Uuid();
            model.uuid = uuid;
            var uri = "/post/dislike/";
            var data = await httpService.Post(uri, model);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            var message = json.message;
            isDisliked = Convert.ToBoolean(message);
        }
        catch (System.Exception error)
        {
            throw new InteralServerErrorException("El servidor ha tenido un error", error);
        }
        return isDisliked;
    }

    public async Task<List<UserSearch>> GetUsersLikesOfPost(string uuid)
    {
        List<UserSearch> users = new List<UserSearch>();
        try
        {
            var uri = $"/post/details/likes/{uuid}";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (message != null)
            {
                foreach (var item in message.likedBy)
                {
                    UserSearch userSearch = new UserSearch();
                    userSearch.username = Convert.ToString(item.username);
                    userSearch.name = Convert.ToString(item.name);
                    userSearch.isFollowed = Convert.ToBoolean(item.isFollowed);
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

    public async Task<List<UserSearch>> GetUsersLikesOfComment(string uuid)
    {
        List<UserSearch> users = new List<UserSearch>();
        try
        {
            var uri = $"/post/comment/details/likes/{uuid}";
            string resultData = await httpService.Get(uri);
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultData);
            var message = json.message;
            if (message != null)
            {
                foreach (var item in message.likedBy)
                {
                    UserSearch userSearch = new UserSearch();
                    userSearch.username = Convert.ToString(item.username);
                    userSearch.name = Convert.ToString(item.name);
                    userSearch.isFollowed = Convert.ToBoolean(item.isFollowed);
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
}