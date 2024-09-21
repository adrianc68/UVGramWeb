namespace UVGramWeb.Shared.Models;

using System.ComponentModel.DataAnnotations;
using UVGramWeb.Shared.Data;

public class GetAllChatMessagesResponse
{
  public List<Message> Messages { get; set; }
}