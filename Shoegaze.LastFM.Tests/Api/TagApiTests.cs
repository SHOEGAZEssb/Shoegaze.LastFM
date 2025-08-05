using Moq;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using System.Text.Json;

namespace Shoegaze.LastFM.Tests.Api
{
  [TestFixture]
  internal class TagApiTests
  {
    #region GetInfoAsync

    [Test]
    public async Task GetInfoAsync_ReturnsInfo_WhenSuccessfull()
    {
      string json = """
      {
        "tag": {
          "name": "shoegaze",
          "total": 224095,
          "reach": 41944,
          "wiki": {
            "summary": "Shoegaze is a style of alternative rock...",
            "content": "Shoegaze is a style of alternative rock..."
          }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("tag.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new TagApi(mock.Object);
      var response = await api.GetInfoAsync("shoegaze");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var tag = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(tag.Name, Is.EqualTo("shoegaze"));
        Assert.That(tag.Taggings, Is.EqualTo(224095));
        Assert.That(tag.Reach, Is.EqualTo(41944));
        Assert.That(tag.Wiki, Is.Not.Null);
      });

      Assert.That(tag.Wiki.Summary, Contains.Substring("alternative rock"));
      Assert.That(tag.Wiki.Content, Contains.Substring("alternative rock"));
      Assert.That(tag.Wiki.Published, Is.Null);
    }

    [Test]
    public async Task GetInfoAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("tag.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new TagApi(mock.Object);
      var response = await api.GetInfoAsync("shoegaze");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }

    [Test]
    public async Task GetInfoAsync_ReturnsError_WhenMalformed()
    {
      string json = """
      {
        "tag": {
          "nameMalformed": "shoegaze",
          "total": 224095,
          "reach": 41944,
          "wiki": {
            "summary": "Shoegaze is a style of alternative rock...",
            "content": "Shoegaze is a style of alternative rock..."
          }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("tag.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new TagApi(mock.Object);
      var response = await api.GetInfoAsync("shoegaze");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }

    #endregion GetInfoAsync
  }
}
