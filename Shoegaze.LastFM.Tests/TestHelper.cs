using Moq;
using Moq.Protected;
using System.Text.Json;

namespace Shoegaze.LastFM.Tests
{
  internal static class TestHelper
  {
    internal static Mock<ILastfmRequestInvoker> CreateMockInvoker(string apiCall, string? json = null)
    {
      var mock = new Mock<ILastfmRequestInvoker>();

      if (json == null)
      {
        mock.Setup(m => m.SendAsync(apiCall, It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(ApiResult<JsonDocument>.Failure());
      }
      else
      {
        mock.Setup(m => m.SendAsync(apiCall, It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));
      }

      return mock;
    }

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
