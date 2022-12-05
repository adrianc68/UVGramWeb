using UVGramWeb.Data.Models;

namespace UserAccountServices.Web.Services
{
    public class UserAccountService: IUserAccountService
    {
        private readonly HttpClient httpClient;

        public UserAccountService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> Login(UserLogin userData) 
        {
            return await httpClient.GetFromJsonAsync<bool>("authentication/login");
        }

    }
}