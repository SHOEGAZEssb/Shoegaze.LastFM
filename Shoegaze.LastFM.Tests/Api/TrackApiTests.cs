using System.Text.Json;
using Moq;
using NUnit.Framework;
using Shoegaze.LastFM;
using Shoegaze.LastFM.Track;
using System.Threading;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tests.Api;

[TestFixture]
public class TrackApiTests
{
  [Test]
  public async Task GetInfoByNameAsync_ReturnsTrackInfo_WhenSuccessful()
  {
    var json = """
    {
      "track": {
        "name": "Blind",
        "url": "https://www.last.fm/music/Korn/_/Blind",
        "duration": "263000",
        "listeners": "913699",
        "playcount": "6750088",
        "userplaycount": "15",
        "artist": {
          "name": "Korn",
          "url": "https://www.last.fm/music/Korn"
        },
        "album": {
          "artist": "Korn",
          "title": "Korn",
          "url": "https://www.last.fm/music/Korn/Korn",
          "image": [
            { "size": "small", "#text": "https://last.fm/small.png" }
          ],
          "@attr": { "position": "1" }
        },
        "toptags": {
          "tag": [
            { "name": "Nu Metal", "url": "https://www.last.fm/tag/Nu+Metal" },
            { "name": "metal", "url": "https://www.last.fm/tag/metal" }
          ]
        },
        "wiki": {
          "published": "08 Dec 2022, 17:14",
          "summary": "Sample summary...",
          "content": "Full content..."
        }
      }
    }
    """;

    var mockInvoker = new Mock<ILastfmRequestInvoker>();
    mockInvoker.Setup(i => i.SendAsync("track.getInfo", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
      .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

    var api = new TrackApi(mockInvoker.Object);

    var result = await api.GetInfoByNameAsync("Blind", "Korn");

    Assert.Multiple(() =>
    {
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Data, Is.Not.Null);
    });

    Assert.Multiple(() =>
    {
      Assert.That(result.Data.Name, Is.EqualTo("Blind"));
      Assert.That(result.Data.ArtistName, Is.EqualTo("Korn"));
      Assert.That(result.Data.Album?.Title, Is.EqualTo("Korn"));
      Assert.That(result.Data.DurationMs, Is.EqualTo(263000));
      Assert.That(result.Data.UserPlaycount, Is.EqualTo(15));
      Assert.That(result.Data.TopTags, Has.Count.EqualTo(2));
      Assert.That(result.Data.Wiki?.Summary, Is.EqualTo("Sample summary..."));
    });
  }
}
