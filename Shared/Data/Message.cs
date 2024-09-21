namespace UVGramWeb.Shared.Data;

public class Message : AbstractPost
{
    public string Content { get; set; }

    public string Sent_At {get; set;}

    public MessageTypeEnum Message_Type {get; set; }

    public string Delivery_At {get; set;}
    public UserSearch User {get; set;}
}