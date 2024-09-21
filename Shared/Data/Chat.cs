namespace UVGramWeb.Shared.Data;

public class Chat
{
  public string Uuid { get; set; }
  public UserSearch Initiator { get; set; }
  public UserSearch Receiver { get; set; }
  public List<Message> Messages { get; set; }

}