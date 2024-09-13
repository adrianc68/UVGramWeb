using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using UVGramWeb.Services;

public interface IHttpService
{
  Task<string> Get(string uri);
  Task<(byte[], Dictionary<string, IEnumerable<string>>)> GetBytes(string uri);
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

  public HttpService(
      HttpClient httpClient,
      NavigationManager navigationManager,
      ILocalStorageService localStorageService,
      IConfiguration configuration
  )
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

  public async Task<(byte[], Dictionary<string, IEnumerable<string>>)> GetBytes(string uri)
  {
    var request = createRequest(HttpMethod.Get, uri);
    (byte[] responseBytes, Dictionary<string, IEnumerable<string>> responseHeaders) = await sendRequestGetBytes(request);
    return (responseBytes, responseHeaders);
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
      if (value is MultipartFormDataContent)
      {
        request.Content = (MultipartFormDataContent)value;
      }
      else
      {
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(value),
            System.Text.Encoding.UTF8,
            "application/json"
        );
      }
    }
    return request;
  }

  private async Task<string> sendRequest<T>(HttpRequestMessage request)
  {
    await addJwtHeader(request);
    using var response = await httpClient.SendAsync(request);
    return await response.Content.ReadAsStringAsync();
  }

  private async Task<(byte[], Dictionary<String, IEnumerable<string>>)> sendRequestGetBytes(HttpRequestMessage request)
  {
    await addJwtHeader(request);
    var response = await httpClient.SendAsync(request);
    byte[] responseBody = await response.Content.ReadAsByteArrayAsync();
    var responseHeaders = response.Headers.ToDictionary(h => h.Key, h => h.Value);
    foreach (var header in response.Content.Headers)
    {
      if (!responseHeaders.ContainsKey(header.Key))
      {
        responseHeaders.Add(header.Key, header.Value);
      }
      else
      {
        responseHeaders[header.Key] = responseHeaders[header.Key].Concat(header.Value);
      }
    }
    return (responseBody, responseHeaders);
  }

  private async Task addJwtHeader(HttpRequestMessage request)
  {
    var User = await localStorageService.GetItem<UVGramWeb.Shared.Models.UserAuthentication>("login");
    if (User != null)
    {
      var isApiUrl = !request.RequestUri.IsAbsoluteUri;
      if (isApiUrl)
      {
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            User.AccessToken
        );
      }
    }
  }

}
