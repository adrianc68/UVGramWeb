using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace UVGramWeb.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorageService;
    public ApiAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var User = await localStorageService.GetItem<UVGramWeb.Shared.Models.User>("login");
        if (User == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        return new AuthenticationState(ParseClaimFromUserToken(User));
    }

    public void NewUserLogOutState()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymous));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NewUserLogInState(UVGramWeb.Shared.Models.User User)
    {
        if (User != null)
        {
            var authState = Task.FromResult(new AuthenticationState(ParseClaimFromUserToken(User)));
            NotifyAuthenticationStateChanged(authState);
        }
    }

    private ClaimsPrincipal ParseClaimFromUserToken(UVGramWeb.Shared.Models.User User)
    {
        var claims = new List<Claim>();
        claims.Add(new Claim("accessToken", User.accessToken));
        claims.Add(new Claim("refreshToken", User.refreshToken));
        var claimIdentity = new ClaimsIdentity(claims, "jwt");
        var claimPrincipal = new ClaimsPrincipal(claimIdentity);
        return claimPrincipal;
    }
}