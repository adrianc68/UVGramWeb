using System.Diagnostics;
using System.Net;
using UVGramWeb.Shared.Helpers;

public static class BackendMessageHandler
{
 public static MessageType GetErrorMessage(Exception error)
 {
  if (error is HttpRequestException)
  {
   return MessageType.API_ERROR;
  }
  return MessageType.INTERNAL_ERROR;
 }

 public static ApiResponse<object> GetMessageFromJson<T>(string data)
 {
  dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
  int status = json.status;
  string message = json.message;
  object dataObject;
  if (json.status == (int)HttpStatusCode.OK)
  {
   dataObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json.data.ToString());
  }
  else
  {
   MessageType resultado = EnumHelper.GetEnumValue<MessageType>(
       json.data.code.ToString(),
       MessageType.NONE
   );
   ErrorBadRequestDataResponse[] errors = null;
   if (json.data.errors != null)
   {
    errors = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorBadRequestDataResponse[]>(json.data.errors.ToString());
   }
   dataObject = new CodeMessageDataResponse
   {
    Code = resultado,
    Message = json.data.message.ToString(),
    Errors = errors
   };
  }
  return new ApiResponse<object>
  {
   Status = status,
   Message = message,
   Data = dataObject,
   Version = json.version
  };
 }
}
