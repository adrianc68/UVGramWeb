


using System.Net;
using UVGramWeb.Helpers;
using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Models;

namespace UVGramWeb.Services;

public class ChatService : IChatService
{
  private IHttpService httpService;

  public ChatService(IHttpService httpService)
  {
    this.httpService = httpService;
  }

  public async Task<List<Chat>> GetAllChat()
  {
    List<Chat> chats = null;
    try
    {
      string uri = $"/chat/all";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<GetAllChatResponse>(data);
      if (apiResponse.Status == (int)HttpStatusCode.OK)
      {
        GetAllChatResponse dataResponse = (GetAllChatResponse)apiResponse.Data;
        chats = dataResponse.Chats;
      }

      foreach (var chat in chats)
      {
        chat.Initiator.url = ConfigHelper.SetResourcesApiBaseUrl(chat.Initiator.url);
        chat.Receiver.url = ConfigHelper.SetResourcesApiBaseUrl(chat.Receiver.url);
        foreach (var message in chat.Messages)
        {
          message.User.url = ConfigHelper.SetResourcesApiBaseUrl(message.User.url);
        }
      }
      chats = chats.OrderByDescending(chat => chat.Messages[0].Sent_At).ToList();
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }

    return chats;
  }

  public async Task<List<Message>> GetAllChatMessages(string uuid)
  {
    List<Message> messages = null;
    try
    {
      string uri = $"/chat/messages/{uuid}";
      string data = await httpService.Get(uri);
      ApiResponse<object> apiResponse = BackendMessageHandler.GetMessageFromJson<GetAllChatMessagesResponse>(data);
      if(apiResponse.Status == (int)HttpStatusCode.OK)
      {
        GetAllChatMessagesResponse dataResponse = (GetAllChatMessagesResponse) apiResponse.Data;
        messages = dataResponse.Messages;
      }

      foreach(var message in messages) 
      {
        message.User.url = ConfigHelper.SetResourcesApiBaseUrl(message.User.url);
      }


      messages = messages.OrderBy(message => message.Sent_At).ToList();
    }
    catch (System.Exception error)
    {
      string ErrorMessage = BackendMessageHandler.GetErrorMessage(error).ToString();
      throw new Exception(ErrorMessage, error);
    }
    return messages;
  }
}