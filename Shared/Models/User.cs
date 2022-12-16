namespace UVGramWeb.Shared.Models;

public class User
{
    public string accessToken { get; set; } = default;
    public string refreshToken { get; set; } = default;
    public RoleType RoleType { get; set; } = default;
    public string Username { get; set; } = default;

}