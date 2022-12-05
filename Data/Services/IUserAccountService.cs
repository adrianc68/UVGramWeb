using UVGramWeb.Data.Models;

public interface IUserAccountService
{
    Task<bool> Login(UserLogin userData);
}