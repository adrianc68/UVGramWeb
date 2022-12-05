using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace UVGramWeb.Shared;

public interface IHttpService
{
    Task<T> Get<T>(string uri);
    Task Post(string uri, object value);
    Task<T> Post<T>(string uri, object value);
    Task Put(string uri, object value);
    Task<T> Put<T>(string uri, object value);
    Task Delete(string uri);
    Task<T> Delete<T>(string uri);
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

    public async Task<T> Get<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await sendRequest<T>(request);
    }

    public async Task Post(string uri, object value)
    {
        var request = createRequest(HttpMethod.Post, uri, value);
        await sendRequest(request);
    }

    public async Task<T> Post<T>(string uri, object value)
    {
        var request = createRequest(HttpMethod.Post, uri, value);
        return await sendRequest<T>(request);
    }

    public async Task Put(string uri, object value)
    {
        var request = createRequest(HttpMethod.Put, uri, value);
        await sendRequest(request);
    }

    public async Task<T> Put<T>(string uri, object value)
    {
        var request = createRequest(HttpMethod.Put, uri, value);
        return await sendRequest<T>(request);
    }

    public async Task Delete(string uri)
    {
        var request = createRequest(HttpMethod.Delete, uri);
        await sendRequest(request);
    }

    public async Task<T> Delete<T>(string uri)
    {
        var request = createRequest(HttpMethod.Delete, uri);
        return await sendRequest<T>(request);
    }

    private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (value != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(value), System.Text.Encoding.UTF8, "application/json");
        }
        return request;
    }

    private async Task sendRequest(HttpRequestMessage request)
    {
        await addJwtHeader(request);
        using var response = await httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("user/logout");
            return;
        }
        await handleErrors(response);
    }

    private async Task<T> sendRequest<T>(HttpRequestMessage request)
    {
        await addJwtHeader(request);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo("user/logout");
            return default;
        }
        await handleErrors(response);
        var options = new JsonSerializerOptions();
        // options.PropertyNameCaseInsensitive = true;
        // options.Converters.Add(new StringConverter());
        return await response.Content.ReadFromJsonAsync<T>(options);
    }

    private async Task addJwtHeader(HttpRequestMessage request)
    {
        var accessToken = await localStorageService.GetItem<string>("accessToken");
        var isApiUrl = !request.RequestUri.IsAbsoluteUri;
        if(isApiUrl) 
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private async Task handleErrors(HttpResponseMessage response)
    {
        if(!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error["message"]);
        }
    }




}