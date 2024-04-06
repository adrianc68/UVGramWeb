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
        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(error.Message);
        if (json.errors != null)
        {
            return MessageType.INVALID_DATA;
        }
        string code = json.data.code;
        return Enum.Parse<MessageType>(code);
    }

    public static object GetMessageFromJson<T>(string data)
    {
        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
        if (json.status == (int)HttpStatusCode.OK)
        {
            return new ApiResponse<T>
            {
                Status = json.status,
                Message = json.message,
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json.data.ToString()),
                Version = json.version
            };
        }

        if (json.errors != null)
        {
            return MessageType.INVALID_DATA;
        }
        string code = json.data.code;
        System.Console.WriteLine(code);
        return Enum.Parse<MessageType>(code);
    }

    public static ApiResponse<object> GetMessageFromJson2<T>(string data)
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
            dataObject = new CodeMessageDataResponse
            {
                Code = resultado,
                Message = json.data.message.ToString()
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
