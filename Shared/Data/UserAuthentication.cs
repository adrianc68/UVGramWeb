namespace UVGramWeb.Shared.Models;

public class UserAuthentication
{
    public string AccessToken { get; set; } = default;
    public string RefreshToken { get; set; } = default;
    public UVGramWeb.Shared.Data.RoleType RoleType { get; set; } = default;
    public string Username { get; set; } = default;
    public string Url {get; set; } = default;
}