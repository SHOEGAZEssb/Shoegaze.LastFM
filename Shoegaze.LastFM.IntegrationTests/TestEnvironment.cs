namespace Shoegaze.LastFM.IntegrationTests
{
  internal static class TestEnvironment
  {
    public static string ApiKey { get; }
    public static string ApiSecret { get; }

    static TestEnvironment()
    {
      try
      {
        string? apiKey = null;
        string? apiSecret = null;
        // try to get from environment variables
        apiKey = Environment.GetEnvironmentVariable("LAST_FM_API_KEY");
        apiSecret = Environment.GetEnvironmentVariable("LAST_FM_API_SECRET");

        if (apiKey == null || apiSecret == null)
        {
          // fallback to local file
          var text = File.ReadAllLines("../../../IntegrationTests.env");
          if (text.Length != 2)
            throw new Exception("Malformed environment file");

          apiKey = text[0];
          apiSecret = text[1];
        }

        ApiKey = apiKey;
        ApiSecret = apiSecret;
      }
      catch (Exception ex)
      {
        throw new Exception($"Error during environment setup: {ex.Message}");
      }
    }

    public static ILastfmClient CreateClient(HttpClient? http = null)
    {
      return new LastfmClient(ApiKey, ApiSecret, http ?? new HttpClient());
    }
  }
}