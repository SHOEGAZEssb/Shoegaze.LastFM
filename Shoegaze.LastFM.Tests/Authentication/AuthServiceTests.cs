using Shoegaze.LastFM.Authentication;
using System.Net;
using System.Text;
using System.Web;

namespace Shoegaze.LastFM.Tests.Authentication
{
  [TestFixture]
  public class AuthServiceTests
  {
    [Test]
    public async Task GetAuthorizationUrlAsync_ReturnsCorrectUrl()
    {
      var http = new HttpClient(); // not used
      var apiKey = "abc123";
      var callbackUrl = "http://localhost:12345/";

      var auth = new LastfmAuthService(http, apiKey, "secret", callbackUrl);
      var result = await auth.GetAuthorizationUrlAsync();

      Assert.That(result.ToString(), Is.EqualTo($"https://www.last.fm/api/auth/?api_key={apiKey}&cb={HttpUtility.UrlEncode(callbackUrl)}"));
    }

    [Test]
    public async Task GetSessionAsync_ParsesValidJsonCorrectly()
    {
      var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent("""
        {
            "session": {
                "name": "tim",
                "key": "mock_session_key",
                "subscriber": 0
            }
        }
        """, Encoding.UTF8, "application/json")
      };

      var httpClient = TestHelper.CreateMockHttpClient(req =>
      {
        Assert.Multiple(() =>
        {
          Assert.That(req.Method, Is.EqualTo(HttpMethod.Post));
          Assert.That(req.RequestUri!.ToString(), Does.Contain("/2.0"));
        });
        return fakeResponse;
      });

      var auth = new LastfmAuthService(httpClient, "api_key", "secret", "http://localhost");

      var session = await auth.GetSessionAsync("token", "", "");

      Assert.Multiple(() =>
      {
        Assert.That(session.Username, Is.EqualTo("tim"));
        Assert.That(session.SessionKey, Is.EqualTo("mock_session_key"));
      });
    }

    [Test]
    public void GetFreePort_IsNotZero()
    {
      Assert.That(LastfmAuthService.GetFreePort(), Is.Not.EqualTo(0));
    }
  }
}