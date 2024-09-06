using System.Globalization;
using System.Net;
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
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<PostDetailsDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        PostDetailsDataResponse postDetailsDataResponse = (PostDetailsDataResponse)apiResponse.Data;
        post = new()
        {
          description = postDetailsDataResponse.Post.description,
          comments_allowed = postDetailsDataResponse.Post.comments_allowed,
          likes_allowed = postDetailsDataResponse.Post.likes_allowed,
          isLiked = postDetailsDataResponse.Post.isLiked,
          likes = postDetailsDataResponse.Likes,
          files = postDetailsDataResponse.Files,
          owner = postDetailsDataResponse.Owner
        };
      }

    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
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


}