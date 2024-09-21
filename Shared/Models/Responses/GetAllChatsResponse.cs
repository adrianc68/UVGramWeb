namespace UVGramWeb.Shared.Models;

using System.ComponentModel.DataAnnotations;
using UVGramWeb.Shared.Data;

public class GetAllChatResponse
{
  public List<Chat> Chats { get; set; }
}