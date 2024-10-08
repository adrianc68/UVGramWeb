using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;
using UVGramWeb.Helpers;
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
      string uri = $"/post/details/{uuid}";
      string data = await httpService.Get(uri);
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
          owner = postDetailsDataResponse.Owner,
          comments = postDetailsDataResponse.Comments,
          uuid = postDetailsDataResponse.Post.uuid,
          created_time = postDetailsDataResponse.Post.created_time
        };
      }
      post.owner.url = ConfigHelper.SetResourcesApiBaseUrl(post.owner.url);
      foreach (var postfile in post.files)
      {
        postfile.url = ConfigHelper.SetResourcesApiBaseUrl(postfile.url);
      }
      foreach (var comment in post.comments)
      {
        comment.url = ConfigHelper.SetResourcesApiBaseUrl(comment.url);
        foreach (var reply in comment.replies)
        {
          reply.url = ConfigHelper.SetResourcesApiBaseUrl(reply.url);
        }
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
      Uuid model = new()
      {
        uuid = uuid
      };
      string uri = "/post/comment/like/";
      string data = await httpService.Post(uri, model);

      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
        isLiked = codeMessageData.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
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
      string uri = "/post/comment/dislike/";
      string data = await httpService.Post(uri, model);

      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
        isDisliked = codeMessageData.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isDisliked;
  }

  public async Task<Comment> ReplyComment(CreateComment model)
  {
    Comment reply = null;
    try
    {
      string uri = "/post/comment/reply";
      string data = await httpService.Post(uri, model);

      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CommentCreatedDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CommentCreatedDataResponse commentCreatedDataResponse = (CommentCreatedDataResponse)apiResponse.Data;
        reply = commentCreatedDataResponse.CommentDetails;
      }
      reply.url = ConfigHelper.SetResourcesApiBaseUrl(reply.url);
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return reply;
  }

  public async Task<Comment> CommentPost(CreateComment model)
  {
    Comment commentCreated = null;
    try
    {
      string uri = "/post/comment/create/";
      string data = await httpService.Post(uri, model);

      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CommentCreatedDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CommentCreatedDataResponse commentCreatedData = (CommentCreatedDataResponse)apiResponse.Data;
        commentCreated = commentCreatedData.CommentDetails;
      }
      commentCreated.url = ConfigHelper.SetResourcesApiBaseUrl(commentCreated.url);
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return commentCreated;
  }

  public async Task<bool> LikePost(string uuid)
  {
    bool isLiked = false;
    try
    {
      Uuid model = new()
      {
        uuid = uuid
      };
      string uri = "/post/like/";
      string data = await httpService.Post(uri, model);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {

        CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
        isLiked = codeMessageData.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      System.Console.WriteLine(error);
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isLiked;
  }

  public async Task<bool> DislikePost(string uuid)
  {
    bool isDisliked = false;
    try
    {
      Uuid model = new()
      {
        uuid = uuid
      };
      string uri = "/post/dislike/";
      string data = await httpService.Post(uri, model);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<CodeMessageDataResponse>(data);

      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        CodeMessageDataResponse codeMessageData = (CodeMessageDataResponse)apiResponse.Data;
        isDisliked = codeMessageData.BoolValue;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return isDisliked;
  }

  public async Task<List<UserSearch>> GetUsersLikesOfPost(string uuid)
  {
    List<UserSearch> users = new List<UserSearch>();
    try
    {
      string uri = $"/post/details/likes/{uuid}";
      string resultData = await httpService.Get(uri);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<PostLikeByDataResponse>(resultData);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        PostLikeByDataResponse postlikeData = (PostLikeByDataResponse)apiResponse.Data;
        users = postlikeData.LikedBy;
        foreach (var user in users)
        {
          user.url = ConfigHelper.SetResourcesApiBaseUrl(user.url);
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

  public async Task<List<UserSearch>> GetUsersLikesOfComment(string uuid)
  {
    List<UserSearch> users = new List<UserSearch>();
    try
    {
      string uri = $"/post/comment/details/likes/{uuid}";
      string resultData = await httpService.Get(uri);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<PostLikeByDataResponse>(resultData);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        PostLikeByDataResponse postlikeData = (PostLikeByDataResponse)apiResponse.Data;
        users = postlikeData.LikedBy;
        foreach (var user in users)
        {
          user.url = ConfigHelper.SetResourcesApiBaseUrl(user.url);
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

  public async Task<Post> CreatePost(CreatePostPubRequest model)
  {
    Post post = null;
    try
    {
      var formData = new MultipartFormDataContent();
      foreach (var mediaFile in model.Files)
      {
        var fileContent = new ByteArrayContent(mediaFile.Data);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaFile.ContentType);
        formData.Add(fileContent, "files[]", mediaFile.Filename);
      }

      formData.Add(new StringContent(model.Description ?? string.Empty, System.Text.Encoding.UTF8, "text/plain"),
      "description");
      formData.Add(new StringContent(model.CommentsAllowed.ToString().ToLower(), System.Text.Encoding.UTF8,
      "application/json"), "commentsAllowed");
      formData.Add(new StringContent(model.LikesAllowed.ToString().ToLower(), System.Text.Encoding.UTF8,
      "application/json"), "likesAllowed");
      string uri = "/post/create";
      string data = await httpService.Post(uri, formData);

      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<PostCreatedDataResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        PostCreatedDataResponse postCreatedDataResponse = (PostCreatedDataResponse)apiResponse.Data;
        post = postCreatedDataResponse.PostInfo;
      }
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return post;
  }


}