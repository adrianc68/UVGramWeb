using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using UVGramWeb.Services;

namespace UVGramWeb.Shared;

public class AppRouteView : RouteView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IUserAccountService UserAccountService { get; set; }

    protected override void Render(RenderTreeBuilder builder)
    {
        var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
        if (authorize && UserAccountService.User == null)
        {
            var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
            NavigationManager.NavigateTo($"user/login?returnUrl={returnUrl}");
        }
        else
        {
            base.Render(builder);
        }
    }
}