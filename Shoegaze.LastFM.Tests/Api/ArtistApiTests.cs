using Moq;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tests.Api
{
  internal class ArtistApiTests
  {
    #region GetInfoByNameAsync

    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new ArtistApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some artist");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }


    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new ArtistApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some artist");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }

    #endregion GetInfoByNameAsync

    #region GetInfoByNameAsync

    [Test]
    public async Task GetSimilarByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getSimilar", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new ArtistApi(mock.Object);
      var response = await api.GetSimilarByNameAsync("some artist");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }


    [Test]
    public async Task GetSimilarByNameAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getSimilar", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new ArtistApi(mock.Object);
      var response = await api.GetSimilarByNameAsync("some artist");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }

    #endregion GetInfoByNameAsync
  }
}
