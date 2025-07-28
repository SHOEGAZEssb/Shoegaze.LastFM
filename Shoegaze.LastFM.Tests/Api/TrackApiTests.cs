using System.Text.Json;
using Moq;
using Shoegaze.LastFM.Track;

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
      Assert.That(result.Data.Artist.Name, Is.EqualTo("Korn"));
      Assert.That(result.Data.Album?.Title, Is.EqualTo("Korn"));
      Assert.That(result.Data.Duration, Is.EqualTo(TimeSpan.FromMilliseconds(263000)));
      Assert.That(result.Data.UserPlayCount, Is.EqualTo(15));
      Assert.That(result.Data.TopTags, Has.Count.EqualTo(2));
      Assert.That(result.Data.Wiki?.Summary, Is.EqualTo("Sample summary..."));
    });
  }

  [Test]
  public async Task GetInfoByMbidAsync_ReturnsTrackInfo_WhenSuccessful()
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
          { "name": "Nu Metal", "url": "https://www.last.fm/tag/Nu+Metal" }
        ]
      },
      "wiki": {
        "published": "08 Dec 2022, 17:14",
        "summary": "MBID summary...",
        "content": "MBID full content..."
      }
    }
  }
  """;

    var mockInvoker = new Mock<ILastfmRequestInvoker>();
    mockInvoker.Setup(i =>
        i.SendAsync("track.getInfo", It.Is<IDictionary<string, string>>(d => d["mbid"] == "e60739a6-eea8-4ee8-acae-d34d5d6ad1d9"), false, It.IsAny<CancellationToken>())
      )
      .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

    var api = new TrackApi(mockInvoker.Object);

    var result = await api.GetInfoByMbidAsync("e60739a6-eea8-4ee8-acae-d34d5d6ad1d9");

    Assert.Multiple(() =>
    {
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Data, Is.Not.Null);
    });

    Assert.Multiple(() =>
    {
      Assert.That(result.Data.Name, Is.EqualTo("Blind"));
      Assert.That(result.Data.Artist.Name, Is.EqualTo("Korn"));
      Assert.That(result.Data.Duration, Is.EqualTo(TimeSpan.FromMilliseconds(263000)));
      Assert.That(result.Data.TopTags, Has.Count.EqualTo(1));
      Assert.That(result.Data.Wiki?.Summary, Is.EqualTo("MBID summary..."));
    });
  }

}
