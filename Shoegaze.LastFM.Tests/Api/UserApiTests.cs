using Moq;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.User;
using System.Text.Json;

namespace Shoegaze.LastFM.Tests.Api
{
  [TestFixture]
  public class UserApiTests
  {
    #region GetInfoAsync

    [Test]
    public async Task GetInfoAsync_ReturnsUserInfo_WhenSuccessful()
    {
      // Arrange
      var json = """
      {
        "user": {
          "name": "coczero",
          "age": "0",
          "subscriber": "1",
          "realname": "Tim Stadler",
          "bootstrap": "0",
          "playcount": "256598",
          "artist_count": "9348",
          "playlists": "0",
          "track_count": "39235",
          "album_count": "17904",
          "image": [
            {
              "size": "small",
              "#text": "https://lastfm.freetls.fastly.net/i/u/34s/952d643f805d4ccabfb00ee8bf51f610.png"
            },
            {
              "size": "medium",
              "#text": "https://lastfm.freetls.fastly.net/i/u/64s/952d643f805d4ccabfb00ee8bf51f610.png"
            },
            {
              "size": "large",
              "#text": "https://lastfm.freetls.fastly.net/i/u/174s/952d643f805d4ccabfb00ee8bf51f610.png"
            },
            {
              "size": "extralarge",
              "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/952d643f805d4ccabfb00ee8bf51f610.png"
            }
          ],
          "registered": {
            "unixtime": "1285787447",
            "#text": 1285787447
          },
          "country": "Germany",
          "gender": "n",
          "url": "https://www.last.fm/user/coczero",
          "type": "subscriber"
        }
      }
      """;

      using var doc = JsonDocument.Parse(json);

      var invokerMock = new Mock<ILastfmRequestInvoker>();
      invokerMock
          .Setup(i => i.SendAsync(
              "user.getInfo",
              It.IsAny<IDictionary<string, string>>(),
              true,
              It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetInfoAsync();
      using (Assert.EnterMultipleScope())
      {
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }

      var user = result.Data!;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(user.Username, Is.EqualTo("coczero"));
        Assert.That(user.RealName, Is.EqualTo("Tim Stadler"));
        Assert.That(user.Url.ToString(), Is.EqualTo("https://www.last.fm/user/coczero"));
        Assert.That(user.ImageUrl!.ToString(), Is.EqualTo("https://lastfm.freetls.fastly.net/i/u/300x300/952d643f805d4ccabfb00ee8bf51f610.png"));
        Assert.That(user.Country, Is.EqualTo("Germany"));
        Assert.That(user.Age, Is.Zero);
        Assert.That(user.Gender, Is.EqualTo("n"));
        Assert.That(user.IsSubscriber, Is.True);
        Assert.That(user.Playcount, Is.EqualTo(256598));
        Assert.That(user.ArtistCount, Is.EqualTo(9348));
        Assert.That(user.TrackCount, Is.EqualTo(39235));
        Assert.That(user.AlbumCount, Is.EqualTo(17904));
        Assert.That(user.Playlists, Is.Zero);
        Assert.That(user.RegisteredDate, Is.EqualTo(DateTimeOffset.FromUnixTimeSeconds(1285787447).DateTime));
      }
    }

    [Test]
    public async Task GetInfoAsync_ReturnsFailure_WhenApiCallFails()
    {
      // Arrange
      var invokerMock = new Mock<ILastfmRequestInvoker>();
      invokerMock
          .Setup(i => i.SendAsync(
              "user.getInfo",
              It.IsAny<IDictionary<string, string>>(),
              true,
              It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure(
              LastFmStatusCode.AuthenticationFailed,
              httpStatus: System.Net.HttpStatusCode.Unauthorized,
              error: "Session key is missing or invalid."
          ));

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetInfoAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.False, "Call should not succeed");
        Assert.That(result.Data, Is.Null, "Data should be null");
        Assert.That(result.Status, Is.EqualTo(LastFmStatusCode.AuthenticationFailed));
        Assert.That(result.HttpStatus, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
        Assert.That(result.ErrorMessage, Is.EqualTo("Session key is missing or invalid."));
      }
    }

    #endregion GetInfoAsync

    #region GetFriendsAsync

    [Test]
    public async Task GetFriendsAsync_ReturnsFriends_WhenSuccessful()
    {
      // Arrange
      var json = """
      {
        "friends": {
          "@attr": {
            "user": "coczero",
            "totalPages": "19",
            "page": "1",
            "perPage": "2",
            "total": "37"
          },
          "user": [
            {
              "name": "Freyatentacle",
              "url": "https://www.last.fm/user/Freyatentacle",
              "country": "None",
              "playlists": "0",
              "playcount": "0",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/1ac36399949d567a937fbbb24e7caf46.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/1ac36399949d567a937fbbb24e7caf46.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/1ac36399949d567a937fbbb24e7caf46.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/1ac36399949d567a937fbbb24e7caf46.png"
                }
              ],
              "registered": {
                "unixtime": "1750950210",
                "#text": "2025-06-26 15:03"
              },
              "realname": "testName",
              "subscriber": "0",
              "bootstrap": "0",
              "type": "user"
            },
            {
              "name": "Heartshackles",
              "url": "https://www.last.fm/user/Heartshackles",
              "country": "Finland",
              "playlists": "0",
              "playcount": "0",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/b7bc4b0dfa5711752d46783b8c344769.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/b7bc4b0dfa5711752d46783b8c344769.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/b7bc4b0dfa5711752d46783b8c344769.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/b7bc4b0dfa5711752d46783b8c344769.png"
                }
              ],
              "registered": {
                "unixtime": "1196350851",
                "#text": "2007-11-29 15:40"
              },
              "realname": "scarlet",
              "subscriber": "0",
              "bootstrap": "0",
              "type": "user"
            }
          ]
        }
      }
      """;

      var invokerMock = new Mock<ILastfmRequestInvoker>();

      var doc = JsonDocument.Parse(json);
      var apiResult = ApiResult<JsonDocument>.Success(doc);

      invokerMock
        .Setup(x => x.SendAsync("user.getFriends", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(apiResult);

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetFriendsAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data.Page, Is.EqualTo(1));
      }

      var first = result.Data!.Items[0];
      using (Assert.EnterMultipleScope())
      {
        Assert.That(first.Username, Is.EqualTo("Freyatentacle"));
        Assert.That(first.RealName, Is.EqualTo("testName"));
        Assert.That(first.Images.ContainsKey(ImageSize.Small), Is.True);
        Assert.That(first.Images[ImageSize.Small].ToString(), Does.Contain("34"));
        Assert.That(result.Data!.Page, Is.EqualTo(1));
        Assert.That(result.Data.TotalPages, Is.EqualTo(19));
        Assert.That(result.Data.TotalItems, Is.EqualTo(37));
        Assert.That(result.Data.PerPage, Is.EqualTo(2));
      }
    }

    [Test]
    public async Task GetFriendsAsync_ReturnsEmptyList_WhenNoFriends()
    {
      // arrange
      var json = """
      {
        "friends": {
          "@attr": {
            "user": "lonelyuser",
            "page": "1",
            "totalPages": "1",
            "total": "0",
            "perPage": "50"
          }
        }
      }
      """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker.Setup(i => i.SendAsync("user.getFriends", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

      var api = new UserApi(mockInvoker.Object);

      // act
      var result = await api.GetFriendsAsync("lonelyuser");
      using (Assert.EnterMultipleScope())
      {
        // assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalItems, Is.Zero);
        Assert.That(result.Data.Page, Is.EqualTo(1));
        Assert.That(result.Data.TotalPages, Is.EqualTo(1));
        Assert.That(result.Data.PerPage, Is.EqualTo(50));
      }
    }

    #endregion GetFriendsAsync

    #region GetLovedTracksAsync

    [Test]
    public async Task GetLovedTracksAsync_ReturnsTracks_WhenPresent()
    {
      var json = """
      {
        "lovedtracks": {
          "track": [
            {
              "artist": {
                "url": "https://www.last.fm/music/Enjoy",
                "name": "Enjoy",
                "mbid": "cd220d6e-c656-4861-8281-949b92dd5905"
              },
              "date": {
                "uts": "1751272740",
                "#text": "30 Jun 2025, 08:39"
              },
              "mbid": "2f264800-7544-4108-b711-d8f768c0f390",
              "url": "https://www.last.fm/music/Enjoy/_/Quiet+In+The+West",
              "name": "Quiet In The West",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png"
                }
              ],
              "streamable": {
                "fulltrack": "0",
                "#text": "0"
              }
            },
            {
              "artist": {
                "url": "https://www.last.fm/music/Enjoy",
                "name": "Enjoy",
                "mbid": "cd220d6e-c656-4861-8281-949b92dd5905"
              },
              "date": {
                "uts": "1751272739",
                "#text": "30 Jun 2025, 08:38"
              },
              "mbid": "ddd34d9f-067b-4cee-9ce3-f0ed6e7738c7",
              "url": "https://www.last.fm/music/Enjoy/_/Flag+And+A+Heart",
              "name": "Flag And A Heart",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png"
                }
              ],
              "streamable": {
                "fulltrack": "0",
                "#text": "0"
              }
            }
          ],
          "@attr": {
            "user": "coczero",
            "totalPages": "91",
            "page": "1",
            "perPage": "2",
            "total": "182"
          }
        }
      }
      """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getLovedTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetLovedTracksAsync("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data.TotalItems, Is.EqualTo(182));
      }

      var track = result.Data.Items[0];
      using (Assert.EnterMultipleScope())
      {
        Assert.That(track.Name, Is.EqualTo("Quiet In The West"));
        Assert.That(track.Artist!.Name, Is.EqualTo("Enjoy"));
        Assert.That(track.UserLoved, Is.True);
      }
    }

    [Test]
    public async Task GetLovedTracksAsync_ReturnsEmptyList_WhenNonePresent()
    {
      var json = """
      {
        "lovedtracks": {
          "@attr": {
            "user": "testuser",
            "page": "1",
            "perPage": "50",
            "totalPages": "1",
            "total": "0"
          }
        }
      }
      """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getLovedTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetLovedTracksAsync("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalItems, Is.Zero);
      }
    }

    #endregion GetLovedTracksAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_ReturnsTracks_WhenSuccessful()
    {
      var json = """
      {
        "toptracks": {
          "track": [
            {
              "streamable": {
                "fulltrack": "0",
                "#text": "0"
              },
              "mbid": "",
              "name": "Ugly",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png"
                }
              ],
              "artist": {
                "url": "https://www.last.fm/music/Y%C3%BCth+Forever",
                "name": "Yüth Forever",
                "mbid": "3bf16eb5-b334-4b53-9f77-041038280bb6"
              },
              "url": "https://www.last.fm/music/Y%C3%BCth+Forever/_/Ugly",
              "duration": "0",
              "@attr": {
                "rank": "1"
              },
              "playcount": "431"
            },
            {
              "streamable": {
                "fulltrack": "0",
                "#text": "0"
              },
              "mbid": "",
              "name": "Lonely Bastard",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png"
                }
              ],
              "artist": {
                "url": "https://www.last.fm/music/Y%C3%BCth+Forever",
                "name": "Yüth Forever",
                "mbid": "3bf16eb5-b334-4b53-9f77-041038280bb6"
              },
              "url": "https://www.last.fm/music/Y%C3%BCth+Forever/_/Lonely+Bastard",
              "duration": "0",
              "@attr": {
                "rank": "2"
              },
              "playcount": "382"
            }
          ],
          "@attr": {
            "user": "coczero",
            "totalPages": "19619",
            "page": "1",
            "perPage": "2",
            "total": "39238"
          }
        }
      }
      """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getTopTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetTopTracksAsync("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data.TotalItems, Is.EqualTo(39238));
        Assert.That(result.Data.Items[0].Name, Is.EqualTo("Ugly"));
      }
    }

    [Test]
    public async Task GetTopTracksAsync_ReturnsEmptyList_WhenNoTracks()
    {
      var json = """
      {
        "toptracks": {
          "@attr": {
            "user": "testuser",
            "page": "1",
            "perPage": "50",
            "totalPages": "0",
            "total": "0"
          }
        }
      }
      """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getTopTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetTopTracksAsync("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      Assert.That(result.Data!.Items, Is.Empty);
    }

    [Test]
    public async Task GetTopTracksAsync_ReturnsSingleTrack_WhenJsonIsObject()
    {
      var json = """
      {
        "toptracks": {
          "track": [
            {
              "streamable": {
                "fulltrack": "0",
                "#text": "0"
              },
              "mbid": "",
              "name": "Ugly",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png"
                }
              ],
              "artist": {
                "url": "https://www.last.fm/music/Y%C3%BCth+Forever",
                "name": "Yüth Forever",
                "mbid": "3bf16eb5-b334-4b53-9f77-041038280bb6"
              },
              "url": "https://www.last.fm/music/Y%C3%BCth+Forever/_/Ugly",
              "duration": "0",
              "@attr": {
                "rank": "1"
              },
              "playcount": "431"
            }
          ],
          "@attr": {
            "user": "coczero",
            "totalPages": "39241",
            "page": "1",
            "perPage": "1",
            "total": "39241"
          }
        }
      }
      """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getTopTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json)));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetTopTracksAsync("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(1));
        Assert.That(result.Data.Items[0].Name, Is.EqualTo("Ugly"));
        Assert.That(result.Data.Items[0].Artist!.Name, Is.EqualTo("Yüth Forever"));
        Assert.That(result.Data.Items[0].UserPlayCount, Is.EqualTo(431));
        Assert.That(result.Data.Items[0].Duration, Is.EqualTo(TimeSpan.FromMilliseconds(0)));
      }
    }

    #endregion GetTopTracksAsync

    #region GetRecentTracksAsync

    [Test]
    public async Task GetRecentTracksAsync_ReturnsCorrectTracks_WhenMultipleTracks()
    {
      const string json = """
      {
        "recenttracks": {
          "track": [
            {
              "artist": {
                "mbid": "4df5d35b-a9d4-4d4f-91f6-bae55693de31",
                "#text": "Zebrahead"
              },
              "streamable": "0",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/6b6de3684dcfe6f76bed299f0a08ed89.jpg"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/6b6de3684dcfe6f76bed299f0a08ed89.jpg"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/6b6de3684dcfe6f76bed299f0a08ed89.jpg"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/6b6de3684dcfe6f76bed299f0a08ed89.jpg"
                }
              ],
              "mbid": "07c4b115-bd04-3846-8915-eb11556358d0",
              "album": {
                "mbid": "353d95fc-d4a9-45aa-9d7f-2f0a4b911412",
                "#text": "Waste of Mind"
              },
              "name": "Check",
              "url": "https://www.last.fm/music/Zebrahead/_/Check",
              "date": {
                "uts": "1753790467",
                "#text": "29 Jul 2025, 12:01"
              }
            },
            {
              "artist": {
                "mbid": "",
                "#text": "The Reverend Horton Heat"
              },
              "streamable": "0",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/c8a0ecd45679558a6ea10e76f658577e.jpg"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/c8a0ecd45679558a6ea10e76f658577e.jpg"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/c8a0ecd45679558a6ea10e76f658577e.jpg"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/c8a0ecd45679558a6ea10e76f658577e.jpg"
                }
              ],
              "mbid": "",
              "album": {
                "mbid": "b5876ebd-506f-406d-8b42-7e6736918a05",
                "#text": "Liquor In The Front"
              },
              "name": "I Can't Surf",
              "url": "https://www.last.fm/music/The+Reverend+Horton+Heat/_/I+Can%27t+Surf",
              "date": {
                "uts": "1753790242",
                "#text": "29 Jul 2025, 11:57"
              }
            }
          ],
          "@attr": {
            "user": "coczero",
            "totalPages": "128279",
            "page": "1",
            "perPage": "2",
            "total": "256557"
          }
        }
      }
      """;

      var jsonDoc = JsonDocument.Parse(json);
      var invoker = new Mock<ILastfmRequestInvoker>();
      invoker
        .Setup(x => x.SendAsync("user.getRecentTracks", It.IsAny<IDictionary<string, string>>(), true, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(jsonDoc));

      var api = new UserApi(invoker.Object);

      var result = await api.GetRecentTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data!.Items[0].Name, Is.EqualTo("Check"));
        Assert.That(result.Data!.Items[1].Name, Is.EqualTo("I Can't Surf"));
      }
    }

    [Test]
    public async Task GetRecentTracksAsync_ReturnsCorrectTrack_WhenSingleTrack()
    {
      const string json = """
      {
        "recenttracks": {
          "track": [
            {
              "artist": {
                "mbid": "34aacaab-6139-4e8e-975d-7f8b66d4a058",
                "#text": "Guttermouth"
              },
              "streamable": "0",
              "image": [
                {
                  "size": "small",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/34s/838dbbd8a7c246d1942df1a33f63c075.jpg"
                },
                {
                  "size": "medium",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/64s/838dbbd8a7c246d1942df1a33f63c075.jpg"
                },
                {
                  "size": "large",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/174s/838dbbd8a7c246d1942df1a33f63c075.jpg"
                },
                {
                  "size": "extralarge",
                  "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/838dbbd8a7c246d1942df1a33f63c075.jpg"
                }
              ],
              "mbid": "39b8e6c4-8d23-3069-96bd-1140fa537679",
              "album": {
                "mbid": "5581458e-0c01-4906-9953-90778941135e",
                "#text": "Covered With Ants"
              },
              "name": "I'm Destroying the World",
              "url": "https://www.last.fm/music/Guttermouth/_/I%27m+Destroying+the+World",
              "date": {
                "uts": "1753778967",
                "#text": "29 Jul 2025, 08:49"
              }
            }
          ],
          "@attr": {
            "user": "coczero",
            "totalPages": "256554",
            "page": "1",
            "perPage": "1",
            "total": "256554"
          }
        }
      }
      """;

      var jsonDoc = JsonDocument.Parse(json);
      var invoker = new Mock<ILastfmRequestInvoker>();
      invoker
        .Setup(x => x.SendAsync("user.getRecentTracks", It.IsAny<IDictionary<string, string>>(), true, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(jsonDoc));

      var api = new UserApi(invoker.Object);

      var result = await api.GetRecentTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      Assert.That(result.Data!.Items, Has.Count.EqualTo(1));
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data.Items[0].Name, Is.EqualTo("I'm Destroying the World"));
        Assert.That(result.Data.Items[0].Artist!.Name, Is.EqualTo("Guttermouth"));
      }
    }


    [Test]
    public async Task GetRecentTracksAsync_ReturnsEmptyList_WhenNoTracks()
    {
      const string json = """
      {
        "recenttracks": {
          "@attr": {
            "user": "testuser",
            "page": "1",
            "perPage": "50",
            "totalPages": "1",
            "total": "0"
          }
        }
      }
      """;

      var jsonDoc = JsonDocument.Parse(json);
      var invoker = new Mock<ILastfmRequestInvoker>();
      invoker
        .Setup(x => x.SendAsync("user.getRecentTracks", It.IsAny<IDictionary<string, string>>(), true, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(jsonDoc));

      var api = new UserApi(invoker.Object);

      var result = await api.GetRecentTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalItems, Is.Zero);
      }
    }

    [Test]
    public async Task GetTopTagsAsync_ReturnsTags_WhenSuccessful()
    {
      var json = """
      {
        "toptags": {
          "tag": [
            {
              "name": "shoegaze",
              "count": "78",
              "url": "https://www.last.fm/tag/shoegaze"
            },
            {
              "name": "seen live",
              "count": "67",
              "url": "https://www.last.fm/tag/seen+live"
            }
          ],
          "@attr": {
            "user": "coczero"
          }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getTopTags", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new UserApi(mock.Object);
      var result = await api.GetTopTagsAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Has.Count.EqualTo(2));
      }
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.Data[0].Name, Is.EqualTo("shoegaze"));
        Assert.That(result.Data[0].UserUsedCount, Is.EqualTo(78));
        Assert.That(result.Data[0].Url.ToString(), Is.EqualTo("https://www.last.fm/tag/shoegaze"));
      }
    }

    #endregion GetRecentTracksAsync

    #region GetTopTagsAsync

    [Test]
    public async Task GetTopTagsAsync_ReturnsEmpty_WhenNoTags()
    {
      var json = """
      {
        "toptags": {}
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getTopTags", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new UserApi(mock.Object);
      var result = await api.GetTopTagsAsync("ts");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Empty);
      }
    }

    [Test]
    public async Task GetTopTagsAsync_ReturnsFailure_OnMalformedJson()
    {
      var json = """
      {
        "toptags": {
          "tag": { "nameee": "broken" }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getTopTags", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new UserApi(mock.Object);
      var result = await api.GetTopTagsAsync("ts");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Status, Is.EqualTo(LastFmStatusCode.UnknownError));
      }
    }

    #endregion GetTopTagsAsync

    #region GetWeeklyChartListAsync

    [Test]
    public async Task GetWeeklyChartListAsync_IntegrationTest()
    {
      var json = """
      {
        "weeklychartlist": {
          "chart": [
            {
              "#text": "",
              "from": "1108296000",
              "to": "1108900800"
            },
            {
              "#text": "",
              "from": "1108900800",
              "to": "1109505600"
            },
            {
              "#text": "",
              "from": "1109505600",
              "to": "1110110400"
            },
            {
              "#text": "",
              "from": "1110110400",
              "to": "1110715200"
            }
          ],
          "@attr": {
            "user": "testuser"
          }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getWeeklyChartList", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new UserApi(mock.Object);
      var response = await api.GetWeeklyChartListAsync("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var chart in response.Data)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(chart.From, Is.Not.Default);
          Assert.That(chart.To, Is.Not.Default);
          Assert.That(new DateTimeOffset(chart.From).ToUnixTimeSeconds(), Is.LessThan(new DateTimeOffset(chart.To).ToUnixTimeSeconds()));
        }
      }
    }

    #endregion GetWeeklyChartListAsync

    #region GetWeeklyChartAsync

    [Test]
    public async Task GetWeeklyChartAsync_Artist_IntegrationTest()
    {
      var json = """
      {
        "weeklyartistchart": {
          "artist": [
            {
              "mbid": "3e9a8114-f872-46a0-a5f6-ff871556df6f",
              "url": "https://www.last.fm/music/Blind+Equation",
              "name": "Blind Equation",
              "@attr": {
                "rank": "1"
              },
              "playcount": "36"
            },
            {
              "mbid": "16356c85-6462-4fae-8c67-8064a5983632",
              "url": "https://www.last.fm/music/Vanna",
              "name": "Vanna",
              "@attr": {
                "rank": "2"
              },
              "playcount": "19"
            },
            {
              "mbid": "",
              "url": "https://www.last.fm/music/bedroom+suicide",
              "name": "bedroom suicide",
              "@attr": {
                "rank": "3"
              },
              "playcount": "16"
            },
            {
              "mbid": "f6beac20-5dfe-4d1f-ae02-0b0a740aafd6",
              "url": "https://www.last.fm/music/Tyler,+The+Creator",
              "name": "Tyler, The Creator",
              "@attr": {
                "rank": "4"
              },
              "playcount": "14"
            },
            {
              "mbid": "",
              "url": "https://www.last.fm/music/Kristian+Harting",
              "name": "Kristian Harting",
              "@attr": {
                "rank": "5"
              },
              "playcount": "13"
            }
          ],
          "@attr": {
            "from": "1753272000",
            "user": "testuser",
            "to": "1753876800"
          }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getWeeklyArtistChart", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new UserApi(mock.Object);
      var response = await api.GetWeeklyChartAsync<ArtistInfo>("testuser");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var artist in response.Data.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.UserPlayCount, Is.GreaterThanOrEqualTo(1));
        }
      }
    }

    #endregion GetWeeklyChartAsync
  }
}