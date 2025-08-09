using System.Text.Json;
using Moq;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.Tests.Api;

[TestFixture]
public class TrackApiTests
{
  #region GetInfoByNameAsync

  [Test]
  public async Task GetInfoByNameAsync_ReturnsTrackInfo_WhenSuccessful()
  {
    var json = """
    {
      "track": {
        "name": "Blind",
        "mbid": "e60739a6-eea8-4ee8-acae-d34d5d6ad1d9",
        "url": "https://www.last.fm/music/Korn/_/Blind",
        "duration": "263000",
        "streamable": {
          "#text": "0",
          "fulltrack": "0"
        },
        "listeners": "914262",
        "playcount": "6755855",
        "artist": {
          "name": "Korn",
          "mbid": "ac865b2e-bba8-4f5a-8756-dd40d5e39f46",
          "url": "https://www.last.fm/music/Korn"
        },
        "album": {
          "artist": "Korn",
          "title": "Korn",
          "mbid": "b06d3f9d-78b1-3155-89be-e7af11730806",
          "url": "https://www.last.fm/music/Korn/Korn",
          "image": [
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/34s/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "small"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/64s/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "medium"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/174s/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "large"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "extralarge"
            }
          ],
          "@attr": {
            "position": "1"
          }
        },
        "userplaycount": "15",
        "userloved": "0",
        "toptags": {
          "tag": [
            {
              "name": "Nu Metal",
              "url": "https://www.last.fm/tag/Nu+Metal"
            },
            {
              "name": "metal",
              "url": "https://www.last.fm/tag/metal"
            },
            {
              "name": "Korn",
              "url": "https://www.last.fm/tag/Korn"
            },
            {
              "name": "rock",
              "url": "https://www.last.fm/tag/rock"
            },
            {
              "name": "Nu-metal",
              "url": "https://www.last.fm/tag/Nu-metal"
            }
          ]
        },
        "wiki": {
          "published": "08 Dec 2022, 17:14",
          "summary": "\"Blind\" is a song by American nu metal band Korn for their self-titled debut album. It was released as the album's first single in August 1994. \"Blind\" had been written while Jonathan Davis was in Sexart before he left the band to join Korn. The song was composed entirely by Dennis Shinn.\n\nKorn's contribution came once they re-recorded the song for their debut album LP 1994. Even though the song was completed by Sexart, Korn added an extended song intro, utilizing drum cymbals, and an added small Bass-Line. <a href=\"http://www.last.fm/music/Korn/_/Blind\">Read more on Last.fm</a>.",
          "content": "\"Blind\" is a song by American nu metal band Korn for their self-titled debut album. It was released as the album's first single in August 1994. \"Blind\" had been written while Jonathan Davis was in Sexart before he left the band to join Korn. The song was composed entirely by Dennis Shinn.\n\nKorn's contribution came once they re-recorded the song for their debut album LP 1994. Even though the song was completed by Sexart, Korn added an extended song intro, utilizing drum cymbals, and an added small Bass-Line. Their addition acted as an intro that led into the original intro composed by Sexart. Also, Korn applied a small musical change away from Sexart's version, which landed under the vocal chorus \"I'm so blind\" lyric. That change didn't occur until the Korn LP was recorded. SexArt's original music piece was present on Korn's demo Neidermayer's Mind 1993. Korn also tailored the song ending, being the bass coda at the end of the song quotes Cypress Hill's song \"Lick a Shot\". The ending, technically, had nothing to do with the actual song itself. Jonathan Davis and his music group Korn utilized this song on the album without crediting the original songwriters Dennis Shinn, and or Ryan Shuck. However, both Dennis Shinn and Ryan Shuck were later credited on the Greatest Hits Vol.1 compilation album when the song was used on that LP., as well as being credited on multiple products that contain the song. The song was released as a promotional single in the United States, Canada and Australia, and as a limited edition 10\" vinyl single in the United Kingdom. It charted on the Canadian alternative chart, the RPM Alternative 30, in November 1995, peaking at number 15.\n\n\"Blind\" is widely considered to be one of the band's best songs. In 2019, Loudwire ranked the song number three on their list of the 50 greatest Korn songs,and in 2021, Kerrang ranked the song number one on their list of the 20 greatest Korn songs. <a href=\"http://www.last.fm/music/Korn/_/Blind\">Read more on Last.fm</a>. User-contributed text is available under the Creative Commons By-SA License; additional terms may apply."
        }
      }
    }
    """;

    var mockInvoker = new Mock<ILastfmApiInvoker>();
    mockInvoker.Setup(i => i.SendAsync("track.getInfo", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
      .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

    var api = new TrackApi(mockInvoker.Object);

    var result = await api.GetInfoByNameAsync("Blind", "Korn", "testuser");
    using (Assert.EnterMultipleScope())
    {
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Data, Is.Not.Null);
    }
    using (Assert.EnterMultipleScope())
    {
      Assert.That(result.Data.Name, Is.EqualTo("Blind"));
      Assert.That(result.Data.Artist!.Name, Is.EqualTo("Korn"));
      Assert.That(result.Data.Album?.Name, Is.EqualTo("Korn"));
      Assert.That(result.Data.Duration, Is.EqualTo(TimeSpan.FromMilliseconds(263000)));
      Assert.That(result.Data.UserPlayCount, Is.EqualTo(15));
      Assert.That(result.Data.TopTags, Has.Count.EqualTo(5));
      Assert.That(result.Data.Wiki?.Summary, Does.Contain("self-titled"));
    }
  }

  #endregion GetInfoByNameAsync

  #region GetInfoByMbidAsync

  [Test]
  public async Task GetInfoByMbidAsync_ReturnsTrackInfo_WhenSuccessful()
  {
    var json = """
    {
      "track": {
        "name": "Blind",
        "mbid": "e60739a6-eea8-4ee8-acae-d34d5d6ad1d9",
        "url": "https://www.last.fm/music/Korn/_/Blind",
        "duration": "263000",
        "streamable": {
          "#text": "0",
          "fulltrack": "0"
        },
        "listeners": "914262",
        "playcount": "6755855",
        "artist": {
          "name": "Korn",
          "mbid": "ac865b2e-bba8-4f5a-8756-dd40d5e39f46",
          "url": "https://www.last.fm/music/Korn"
        },
        "album": {
          "artist": "Korn",
          "title": "Korn",
          "mbid": "b06d3f9d-78b1-3155-89be-e7af11730806",
          "url": "https://www.last.fm/music/Korn/Korn",
          "image": [
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/34s/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "small"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/64s/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "medium"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/174s/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "large"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/96a5ae52a9a04676f4e1ce2081bc15b6.png",
              "size": "extralarge"
            }
          ],
          "@attr": {
            "position": "1"
          }
        },
        "userplaycount": "15",
        "userloved": "0",
        "toptags": {
          "tag": [
            {
              "name": "Nu Metal",
              "url": "https://www.last.fm/tag/Nu+Metal"
            },
            {
              "name": "metal",
              "url": "https://www.last.fm/tag/metal"
            },
            {
              "name": "Korn",
              "url": "https://www.last.fm/tag/Korn"
            },
            {
              "name": "rock",
              "url": "https://www.last.fm/tag/rock"
            },
            {
              "name": "Nu-metal",
              "url": "https://www.last.fm/tag/Nu-metal"
            }
          ]
        },
        "wiki": {
          "published": "08 Dec 2022, 17:14",
          "summary": "\"Blind\" is a song by American nu metal band Korn for their self-titled debut album. It was released as the album's first single in August 1994. \"Blind\" had been written while Jonathan Davis was in Sexart before he left the band to join Korn. The song was composed entirely by Dennis Shinn.\n\nKorn's contribution came once they re-recorded the song for their debut album LP 1994. Even though the song was completed by Sexart, Korn added an extended song intro, utilizing drum cymbals, and an added small Bass-Line. <a href=\"http://www.last.fm/music/Korn/_/Blind\">Read more on Last.fm</a>.",
          "content": "\"Blind\" is a song by American nu metal band Korn for their self-titled debut album. It was released as the album's first single in August 1994. \"Blind\" had been written while Jonathan Davis was in Sexart before he left the band to join Korn. The song was composed entirely by Dennis Shinn.\n\nKorn's contribution came once they re-recorded the song for their debut album LP 1994. Even though the song was completed by Sexart, Korn added an extended song intro, utilizing drum cymbals, and an added small Bass-Line. Their addition acted as an intro that led into the original intro composed by Sexart. Also, Korn applied a small musical change away from Sexart's version, which landed under the vocal chorus \"I'm so blind\" lyric. That change didn't occur until the Korn LP was recorded. SexArt's original music piece was present on Korn's demo Neidermayer's Mind 1993. Korn also tailored the song ending, being the bass coda at the end of the song quotes Cypress Hill's song \"Lick a Shot\". The ending, technically, had nothing to do with the actual song itself. Jonathan Davis and his music group Korn utilized this song on the album without crediting the original songwriters Dennis Shinn, and or Ryan Shuck. However, both Dennis Shinn and Ryan Shuck were later credited on the Greatest Hits Vol.1 compilation album when the song was used on that LP., as well as being credited on multiple products that contain the song. The song was released as a promotional single in the United States, Canada and Australia, and as a limited edition 10\" vinyl single in the United Kingdom. It charted on the Canadian alternative chart, the RPM Alternative 30, in November 1995, peaking at number 15.\n\n\"Blind\" is widely considered to be one of the band's best songs. In 2019, Loudwire ranked the song number three on their list of the 50 greatest Korn songs,and in 2021, Kerrang ranked the song number one on their list of the 20 greatest Korn songs. <a href=\"http://www.last.fm/music/Korn/_/Blind\">Read more on Last.fm</a>. User-contributed text is available under the Creative Commons By-SA License; additional terms may apply."
        }
      }
    }
  """;

    var mockInvoker = new Mock<ILastfmApiInvoker>();
    mockInvoker.Setup(i =>
        i.SendAsync("track.getInfo", It.Is<IDictionary<string, string>>(d => d["mbid"] == "e60739a6-eea8-4ee8-acae-d34d5d6ad1d9"), false, It.IsAny<CancellationToken>())
      )
      .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

    var api = new TrackApi(mockInvoker.Object);

    var result = await api.GetInfoByMbidAsync("e60739a6-eea8-4ee8-acae-d34d5d6ad1d9");
    using (Assert.EnterMultipleScope())
    {
      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Data, Is.Not.Null);
    }
    using (Assert.EnterMultipleScope())
    {
      Assert.That(result.Data.Name, Is.EqualTo("Blind"));
      Assert.That(result.Data.Artist!.Name, Is.EqualTo("Korn"));
      Assert.That(result.Data.Duration, Is.EqualTo(TimeSpan.FromMilliseconds(263000)));
      Assert.That(result.Data.TopTags, Has.Count.EqualTo(5));
      Assert.That(result.Data.Wiki?.Summary, Does.Contain("self-titled"));
    }
  }

  #endregion GetInfoByMbidAsync
}