namespace UVGramWeb.Shared.Models;

using System.ComponentModel.DataAnnotations;
using UVGramWeb.Shared.Data;

public class SendMessageDataResponse
{
  public Chat ChatInfo {get; set;}
  public Message MessageCreated {get; set;}
}