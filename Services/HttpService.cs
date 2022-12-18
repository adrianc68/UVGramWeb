using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using UVGramWeb.Services;

public interface IHttpService
{
    Task<string> Get(string uri);
    Task<string> Post(string uri, object value);
    Task<string> Patch(string uri, object value);
    Task<string> Put(string uri, object value);
    Task<string> Delete(string uri);
}

public class HttpService : IHttpService
{
    private HttpClient httpClient;
    private NavigationManager navigationManager;
    private ILocalStorageService localStorageService;
    private IConfiguration configuration;

    public HttpService(HttpClient httpClient, NavigationManager navigationManager, ILocalStorageService localStorageService, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
        this.localStorageService = localStorageService;
        this.configuration = configuration;
    }

    public async Task<string> Get(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await sendRequest<string>(request);
    }

    public async Task<string> Post(string uri, object value)
    {
        var request = createRequest(HttpMethod.Post, uri, value);
        return await sendRequest<string>(request);
    }

    public async Task<string> Patch(string uri, object value)
    {
        var request = createRequest(HttpMethod.Patch, uri, value);
        return await sendRequest<string>(request);
    }

    public async Task<string> Put(string uri, object value)
    {
        var request = createRequest(HttpMethod.Put, uri, value);
        return await sendRequest<string>(request);
    }

    public async Task<string> Delete(string uri)
    {
        var request = createRequest(HttpMethod.Delete, uri);
        return await sendRequest<string>(request);
    }

    private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (value != null)
        {
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), System.Text.Encoding.UTF8, "application/json");
        }
        return request;
    }

    private async Task sendRequest(HttpRequestMessage request)
    {
        await addJwtHeader(request);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("/authentication/logout");
            return;
        }
        await handleErrors(response);
    }

    private async Task<string> sendRequest<T>(HttpRequestMessage request)
    {
        await addJwtHeader(request);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("/logout");
            return default;
        }
        await handleErrors(response);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task addJwtHeader(HttpRequestMessage request)
    {
        var User = await localStorageService.GetItem<UVGramWeb.Shared.Models.User>("login");
        if (User != null)
        {
            var isApiUrl = !request.RequestUri.IsAbsoluteUri;
            if (isApiUrl)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", User.accessToken);
            }
        }
    }

    private async Task handleErrors(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }




}