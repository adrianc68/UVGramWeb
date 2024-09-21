using UVGramWeb.Shared.Data;
using UVGramWeb.Shared.Models;

namespace UVGramWeb.Services;

public interface IChatService
{
    Task<List<Chat>> GetAllChat();
    Task<List<Message>> GetAllChatMessages(string uuid);
}