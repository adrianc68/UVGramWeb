using UVGramWeb.Shared.Data;
public class CommentCreatedDataResponse
{
 public bool BoolValue { get; set; }
 public MessageType Code { get; set; }
 public string Message { get; set; }
 public Comment CommentDetails {get; set;}
}