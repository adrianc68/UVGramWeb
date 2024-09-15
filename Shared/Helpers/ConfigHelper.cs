using Microsoft.Extensions.Configuration;

namespace UVGramWeb.Helpers
{
  public static class ConfigHelper
  {
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public static string SetResourcesApiBaseUrl(string url)
    {
      string fullUrl = null;
      if (url != null)
      {
        fullUrl = $"{_configuration["ApiSettings:BaseUrl"]}{url}";
      }
      return fullUrl;
    }
  }
}
