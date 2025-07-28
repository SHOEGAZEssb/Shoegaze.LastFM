using Moq;
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
            "id": "1000002",
            "name": "RJ",
            "realname": "Richard Jones",
            "url": "http://www.last.fm/user/RJ",
            "image": "http://userserve-ak.last.fm/serve/126/8270359.jpg",
            "country": "UK",
            "age": "27",
            "gender": "m",
            "subscriber": "1",
            "playcount": "54189",
            "playlists": "4",
            "bootstrap": "0",
            "registered": {
              "#text": "2002-11-20 11:50",
              "unixtime": "1037793040"
            }
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
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc, 200));

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetInfoAsync();

      Assert.Multiple(() =>
      {
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });

      var user = result.Data!;
      Assert.Multiple(() =>
      {
        Assert.That(user.Id, Is.EqualTo("1000002"));
        Assert.That(user.Username, Is.EqualTo("RJ"));
        Assert.That(user.RealName, Is.EqualTo("Richard Jones"));
        Assert.That(user.Url, Is.EqualTo("http://www.last.fm/user/RJ"));
        Assert.That(user.ImageUrl, Is.EqualTo("http://userserve-ak.last.fm/serve/126/8270359.jpg"));
        Assert.That(user.Country, Is.EqualTo("UK"));
        Assert.That(user.Age, Is.EqualTo(27));
        Assert.That(user.Gender, Is.EqualTo("m"));
        Assert.That(user.IsSubscriber, Is.True);
        Assert.That(user.Playcount, Is.EqualTo(54189));
        Assert.That(user.Playlists, Is.EqualTo(4));
        Assert.That(user.RegisteredDate, Is.EqualTo(DateTimeOffset.FromUnixTimeSeconds(1037793040).DateTime));
      });
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
              ApiStatusCode.AuthenticationRequired,
              httpStatus: 401,
              error: "Session key is missing or invalid."
          ));

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetInfoAsync();

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.False, "Call should not succeed");
        Assert.That(result.Data, Is.Null, "Data should be null");
        Assert.That(result.Status, Is.EqualTo(ApiStatusCode.AuthenticationRequired));
        Assert.That(result.HttpStatusCode, Is.EqualTo(401));
        Assert.That(result.ErrorMessage, Is.EqualTo("Session key is missing or invalid."));
      });
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
        "page": "1",
        "totalPages": "3",
        "total": "109",
        "perPage": "50"
      },
      "user": [
        {
          "id": "123",
          "name": "eartle",
          "realname": "Michael Coffey",
          "url": "http://www.last.fm/user/eartle",
          "country": "UK",
          "age": "29",
          "gender": "m",
          "subscriber": "1",
          "playcount": "45366",
          "playlists": "4",
          "registered": { "unixtime": "1189696970" },
          "image": [
            { "#text": "http://.../34.jpg", "size": "small" },
            { "#text": "http://.../64.jpg", "size": "medium" }
          ]
        },
        {
          "id": "456",
          "name": "otheruser",
          "realname": "",
          "url": "http://www.last.fm/user/otheruser",
          "country": "",
          "age": "0",
          "gender": "",
          "subscriber": "0",
          "playcount": "0",
          "playlists": "0",
          "registered": { "unixtime": "1600000000" },
          "image": { "#text": "http://.../34.jpg", "size": "small" }
        }
      ]
    }
  }
  """;

      var invokerMock = new Mock<ILastfmRequestInvoker>();

      var doc = JsonDocument.Parse(json);
      var apiResult = ApiResult<JsonDocument>.Success(doc, 200);

      invokerMock
        .Setup(x => x.SendAsync("user.getFriends", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(apiResult);

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetFriendsAsync("joanofarctan");

      Assert.Multiple(() =>
      {
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });

      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data.Page, Is.EqualTo(1));
      });

      var first = result.Data!.Items[0];
      Assert.Multiple(() =>
      {
        Assert.That(first.Username, Is.EqualTo("eartle"));
        Assert.That(first.RealName, Is.EqualTo("Michael Coffey"));
        Assert.That(first.Images.ContainsKey(ImageSize.Small), Is.True);
        Assert.That(first.Images[ImageSize.Small], Does.Contain("34.jpg"));
        Assert.That(result.Data!.Page, Is.EqualTo(1));
        Assert.That(result.Data.TotalPages, Is.EqualTo(3));
        Assert.That(result.Data.TotalItems, Is.EqualTo(109));
        Assert.That(result.Data.PerPage, Is.EqualTo(50));
      });
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
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

      var api = new UserApi(mockInvoker.Object);

      // act
      var result = await api.GetFriendsAsync("lonelyuser");

      Assert.Multiple(() =>
      {
        // assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });

      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalItems, Is.EqualTo(0));
        Assert.That(result.Data.Page, Is.EqualTo(1));
        Assert.That(result.Data.TotalPages, Is.EqualTo(1));
        Assert.That(result.Data.PerPage, Is.EqualTo(50));
      });
    }

    #endregion GetFriendsAsync

    #region GetLovedTracksAsync

    [Test]
    public async Task GetLovedTracksAsync_ReturnsTracks_WhenPresent()
    {
      var json = """
  {
    "lovedtracks": {
      "@attr": {
        "user": "testuser",
        "page": "1",
        "perPage": "2",
        "totalPages": "1",
        "total": "2"
      },
      "track": [
        {
          "name": "Track One",
          "url": "https://www.last.fm/music/ArtistOne/_/Track+One",
          "artist": {
            "name": "ArtistOne",
            "url": "https://www.last.fm/music/ArtistOne"
          },
          "date": {
            "uts": "1600000000",
            "#text": "2020-09-13 10:00"
          },
          "image": [
            { "size": "small", "#text": "https://img1.png" },
            { "size": "medium", "#text": "https://img2.png" }
          ]
        },
        {
          "name": "Track Two",
          "url": "https://www.last.fm/music/ArtistTwo/_/Track+Two",
          "artist": {
            "name": "ArtistTwo",
            "url": "https://www.last.fm/music/ArtistTwo"
          },
          "date": {
            "uts": "1601000000",
            "#text": "2020-09-25 15:30"
          },
          "image": [
            { "size": "large", "#text": "https://img3.png" },
            { "size": "extralarge", "#text": "https://img4.png" }
          ]
        }
      ]
    }
  }
  """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getLovedTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetLovedTracksAsync("testuser");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data.TotalItems, Is.EqualTo(2));
      });

      var track = result.Data.Items[0];
      Assert.Multiple(() =>
      {
        Assert.That(track.Name, Is.EqualTo("Track One"));
        Assert.That(track.ArtistName, Is.EqualTo("ArtistOne"));
      });
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
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetLovedTracksAsync("testuser");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalItems, Is.EqualTo(0));
      });
    }

    #endregion GetLovedTracksAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_ReturnsTracks_WhenSuccessful()
    {
      var json = """
  {
    "toptracks": {
      "@attr": {
        "user": "testuser",
        "page": "1",
        "perPage": "2",
        "totalPages": "1",
        "total": "2"
      },
      "track": [
        {
          "name": "Song A",
          "url": "https://www.last.fm/music/ArtistA/_/Song+A",
          "playcount": "123",
          "duration": "240",
          "@attr": { "rank": "1" },
          "artist": {
            "name": "ArtistA",
            "url": "https://www.last.fm/music/ArtistA"
          },
          "image": []
        },
        {
          "name": "Song B",
          "url": "https://www.last.fm/music/ArtistB/_/Song+B",
          "playcount": "45",
          "duration": "180",
          "@attr": { "rank": "2" },
          "artist": {
            "name": "ArtistB",
            "url": "https://www.last.fm/music/ArtistB"
          },
          "image": []
        }
      ]
    }
  }
  """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getTopTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetTopTracksAsync("testuser");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data.TotalItems, Is.EqualTo(2));
        Assert.That(result.Data.Items[0].Name, Is.EqualTo("Song A"));
        Assert.That(result.Data.Items[0].Rank, Is.EqualTo(1));
      });
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
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetTopTracksAsync("testuser");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.That(result.Data!.Items, Is.Empty);
    }

    [Test]
    public async Task GetTopTracksAsync_ReturnsSingleTrack_WhenJsonIsObject()
    {
      var json = """
  {
    "toptracks": {
      "@attr": {
        "user": "testuser",
        "page": "1",
        "perPage": "1",
        "totalPages": "1",
        "total": "1"
      },
      "track": {
        "name": "Only Song",
        "url": "https://www.last.fm/music/OnlyArtist/_/Only+Song",
        "playcount": "999",
        "duration": "300",
        "@attr": { "rank": "1" },
        "artist": {
          "name": "OnlyArtist",
          "url": "https://www.last.fm/music/OnlyArtist"
        },
        "image": []
      }
    }
  }
  """;

      var mockInvoker = new Mock<ILastfmRequestInvoker>();
      mockInvoker
        .Setup(i => i.SendAsync("user.getTopTracks", It.IsAny<IDictionary<string, string>>(), false, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(JsonDocument.Parse(json), 200));

      var api = new UserApi(mockInvoker.Object);
      var result = await api.GetTopTracksAsync("testuser");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Has.Count.EqualTo(1));
        Assert.That(result.Data.Items[0].Name, Is.EqualTo("Only Song"));
        Assert.That(result.Data.Items[0].ArtistName, Is.EqualTo("OnlyArtist"));
        Assert.That(result.Data.Items[0].Playcount, Is.EqualTo(999));
        Assert.That(result.Data.Items[0].Duration, Is.EqualTo(TimeSpan.FromSeconds(300)));
      });
    }

    #endregion GetTopTracksAsync

    #region GetRecentTracksAsync

    [Test]
    public async Task GetRecentTracksAsync_ReturnsCorrectTracks_WhenMultipleTracks()
    {
      const string json = """
    {
      "recenttracks": {
        "@attr": {
          "user": "testuser",
          "page": "1",
          "perPage": "2",
          "totalPages": "1",
          "total": "2"
        },
        "track": [
          {
            "name": "Song One",
            "url": "https://www.last.fm/music/Artist/_/Song+One",
            "artist": {
              "#text": "Artist",
              "mbid": "artist-mbid"
            },
            "album": {
              "#text": "Album One",
              "mbid": "album-mbid"
            },
            "image": [
              { "size": "small", "#text": "http://img1" }
            ],
            "date": {
              "uts": "1710000000"
            },
            "streamable": "1"
          },
          {
            "name": "Song Two",
            "url": "https://www.last.fm/music/Artist/_/Song+Two",
            "artist": "Another Artist",
            "streamable": "0"
          }
        ]
      }
    }
    """;

      var jsonDoc = JsonDocument.Parse(json);
      var invoker = new Mock<ILastfmRequestInvoker>();
      invoker
        .Setup(x => x.SendAsync("user.getRecentTracks", It.IsAny<IDictionary<string, string>>(), true, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(jsonDoc, 200));

      var api = new UserApi(invoker.Object);

      var result = await api.GetRecentTracksAsync();

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Items, Has.Count.EqualTo(2));
        Assert.That(result.Data!.Items[0].Name, Is.EqualTo("Song One"));
        Assert.That(result.Data!.Items[1].Name, Is.EqualTo("Song Two"));
      });
    }

    [Test]
    public async Task GetRecentTracksAsync_ReturnsCorrectTrack_WhenSingleTrack()
    {
      const string json = """
    {
      "recenttracks": {
        "@attr": {
          "user": "testuser",
          "page": "1",
          "perPage": "1",
          "totalPages": "1",
          "total": "1"
        },
        "track":
        {
          "name": "Lone Song",
          "url": "https://www.last.fm/music/Artist/_/Lone+Song",
          "artist": "Solo Artist",
          "streamable": "1"
        }
      }
    }
    """;

      var jsonDoc = JsonDocument.Parse(json);
      var invoker = new Mock<ILastfmRequestInvoker>();
      invoker
        .Setup(x => x.SendAsync("user.getRecentTracks", It.IsAny<IDictionary<string, string>>(), true, It.IsAny<CancellationToken>()))
        .ReturnsAsync(ApiResult<JsonDocument>.Success(jsonDoc, 200));

      var api = new UserApi(invoker.Object);

      var result = await api.GetRecentTracksAsync();

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.That(result.Data!.Items, Has.Count.EqualTo(1));
      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items[0].Name, Is.EqualTo("Lone Song"));
        Assert.That(result.Data!.Items[0].ArtistName, Is.EqualTo("Solo Artist"));
      });
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
        .ReturnsAsync(ApiResult<JsonDocument>.Success(jsonDoc, 200));

      var api = new UserApi(invoker.Object);

      var result = await api.GetRecentTracksAsync();

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });
      Assert.Multiple(() =>
      {
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalItems, Is.EqualTo(0));
      });
    }

    [Test]
    public async Task GetTopTagsAsync_ReturnsTags_WhenSuccessful()
    {
      var json = """
  {
    "toptags": {
      "tag": [
        { "name": "shoegaze", "count": "42", "url": "https://www.last.fm/tag/shoegaze" },
        { "name": "dreampop", "count": "27", "url": "https://www.last.fm/tag/dreampop" }
      ],
      "@attr": { "user": "ts" }
    }
  }
  """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getTopTags", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc, 200));

      var api = new UserApi(mock.Object);
      var result = await api.GetTopTagsAsync("ts");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Has.Count.EqualTo(2));
      });
      Assert.Multiple(() =>
      {
        Assert.That(result.Data[0].Name, Is.EqualTo("shoegaze"));
        Assert.That(result.Data[0].Count, Is.EqualTo(42));
        Assert.That(result.Data[0].Url.ToString(), Is.EqualTo("https://www.last.fm/tag/shoegaze"));
      });
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
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc, 200));

      var api = new UserApi(mock.Object);
      var result = await api.GetTopTagsAsync("ts");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Empty);
      });
    }

    [Test]
    public async Task GetTopTagsAsync_ReturnsFailure_OnMalformedJson()
    {
      var json = """
  {
    "toptags": {
      "tag": { "name": "broken" }
    }
  }
  """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("user.getTopTags", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc, 200));

      var api = new UserApi(mock.Object);
      var result = await api.GetTopTagsAsync("ts");

      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Status, Is.EqualTo(ApiStatusCode.UnknownError));
      });
    }

    #endregion GetTopTagsAsync
  }
}