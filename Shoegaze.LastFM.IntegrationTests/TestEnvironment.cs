namespace Shoegaze.LastFM.IntegrationTests
{
  internal static class TestEnvironment
  {
    private static string ApiKey { get; }
    private static string ApiSecret { get; }
    private static string? SessionKey { get; }

    static TestEnvironment()
    {
      try
      {
        string? apiKey = null;
        string? apiSecret = null;
        string? sessionKey = null;
        // try to get from environment variables
        apiKey = Environment.GetEnvironmentVariable("LAST_FM_API_KEY");
        apiSecret = Environment.GetEnvironmentVariable("LAST_FM_API_SECRET");
        sessionKey = Environment.GetEnvironmentVariable("LAST_FM_SESSION_KEY");

        if (apiKey == null || apiSecret == null)
        {
          // fallback to local file
          var text = File.ReadAllLines("../../../IntegrationTests.env");
          if (text.Length < 2)
            throw new Exception("Malformed environment file");

          apiKey = text[0];
          apiSecret = text[1];

          try
          {
            sessionKey = text[2];
          }
          catch
          {
            // session key may be null
          }
        }

        ApiKey = apiKey;
        ApiSecret = apiSecret;
        SessionKey = sessionKey;
      }
      catch (Exception ex)
      {
        throw new Exception($"Error during environment setup: {ex.Message}");
      }
    }

    public static ILastfmClient CreateClient(HttpClient? httpClient = null)
    {
      return new LastfmClient(ApiKey, ApiSecret, httpClient);
    }

    public static ILastfmClient CreateAuthenticatedClient(HttpClient? http = null)
    {
      if (SessionKey == null)
        throw new Exception("No session key available");

      var client = CreateClient(http);
      client.SetSessionKey(SessionKey);
      return client;
    }
  }
}