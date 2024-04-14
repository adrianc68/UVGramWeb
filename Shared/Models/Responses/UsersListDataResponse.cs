using UVGramWeb.Shared.Data;

public class UsersListDataResponse
{
     public MessageType Code { get; set; }
    public string Message { get; set; }
    public List<UserSearch> Users { get; set; }
}