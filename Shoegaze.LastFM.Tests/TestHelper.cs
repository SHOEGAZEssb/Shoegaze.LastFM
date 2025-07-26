using Moq;
using Moq.Protected;

namespace Shoegaze.LastFM.Tests
{
  internal static class TestHelper
  {
    internal static HttpClient CreateMockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
      var handlerMock = new Mock<HttpMessageHandler>();
      handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
          {
            return responder(request);
          });

      return new HttpClient(handlerMock.Object)
      {
        BaseAddress = new Uri("https://ws.audioscrobbler.com")
      };
    }
  }
}
