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
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new ArtistApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
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
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetSimilarByNameAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getSimilar", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new ArtistApi(mock.Object);
      var response = await api.GetSimilarByNameAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetInfoByNameAsync

    #region GetCorrectionAsync

    [Test]
    public async Task GetCorrectionAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getCorrection", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new ArtistApi(mock.Object);
      var response = await api.GetCorrectionAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetCorrectionAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getCorrection", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new ArtistApi(mock.Object);
      var response = await api.GetCorrectionAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetCorrectionAsync

    #region GetTagsByNameAsync

    [Test]
    public async Task GetTagsByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("artist.getTags", json);

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTagsByNameAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTagsByNameAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("artist.getTags");

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTagsByNameAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTagsByNameAsync

    #region GetTagsByMbidAsync

    [Test]
    public async Task GetTagsByMbidAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("artist.getTags", json);

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTagsByNameAsync("some-mbid");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTagsByMbidAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("artist.getTags");

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTagsByMbidAsync("some-mbid");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTagsByMbidAsync

    #region GetTopAlbumsByNameAsync

    [Test]
    public async Task GetTopAlbumsByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("artist.getTopAlbums", json);

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTopAlbumsByNameAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopAlbumsByNameAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("artist.getTopAlbums");

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTopAlbumsByNameAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopAlbumsByNameAsync

    #region GetTopAlbumsByMbidAsync

    [Test]
    public async Task GetTopAlbumsByMbidAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("artist.getTopAlbums", json);

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTopAlbumsByMbidAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopAlbumsByMbidAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("artist.getTopAlbums");

      var api = new ArtistApi(mock.Object);
      var response = await api.GetTopAlbumsByMbidAsync("some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopAlbumsByMbidAsync
  }
}
