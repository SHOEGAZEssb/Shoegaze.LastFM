using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class UserApiIntegrationTests
  {
    #region GetInfoAsync

    [Test]
    public async Task GetInfoAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetInfoAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var user = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(user.Username, Is.EqualTo("coczero"));
        Assert.That(user.TrackCount, Is.GreaterThan(1));
        Assert.That(user.ArtistCount, Is.GreaterThan(1));
        Assert.That(user.AlbumCount, Is.GreaterThan(1));
        Assert.That(user.Country, Is.EqualTo("Germany"));
        Assert.That(user.Playcount, Is.GreaterThan(1));
        Assert.That(user.RealName, Is.EqualTo("Tim Stadler"));
        Assert.That(user.Images, Contains.Key(ImageSize.Small));
        Assert.That(user.Images, Contains.Key(ImageSize.Medium));
        Assert.That(user.Images, Contains.Key(ImageSize.Large));
        Assert.That(user.Images, Contains.Key(ImageSize.ExtraLarge));
        Assert.That(user.RegisteredDate, Is.EqualTo(DateTimeOffset.FromUnixTimeSeconds(1285787447).DateTime));
      }
    }

    [Test]
    public async Task GetInfoAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetInfoAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetInfoAsync

    #region GetFriendsAsync

    [Test]
    public async Task GetFriendsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetFriendsAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var friends = pages.Items;
      Assert.That(friends, Has.Count.GreaterThan(1));
      foreach (var friend in friends)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(friend.Url.ToString(), Is.Not.Empty);
          Assert.That(friend.Username, Is.Not.Empty);
        }
      }
    }

    [Test]
    public async Task GetFriendsAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetFriendsAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetFriendsAsync

    #region GetLovedTracksAsync

    [Test]
    public async Task GetLovedTracksAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetLovedTracksAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var tracks = pages.Items;
      foreach (var track in tracks)
      {
        Assert.That(track.Artist, Is.Not.Null);
        var artist = track.Artist;
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
        }
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Album, Is.Null);
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          Assert.That(track.UserLoved, Is.True);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.Images, Contains.Key(ImageSize.Small));
          Assert.That(track.Images, Contains.Key(ImageSize.Medium));
          Assert.That(track.Images, Contains.Key(ImageSize.Large));
          Assert.That(track.Images, Contains.Key(ImageSize.ExtraLarge));
          Assert.That(track.Duration, Is.Null);
          Assert.That(track.UserLovedDate, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task GetLovedTrackAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetLovedTracksAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetLovedTracksAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopTracksAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var tracks = pages.Items;
      foreach (var track in tracks)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.UserPlayCount, Is.GreaterThan(1));
          Assert.That(track.Duration, Is.Not.Null);
          Assert.That(track.Images, Contains.Key(ImageSize.Small));
          Assert.That(track.Images, Contains.Key(ImageSize.Medium));
          Assert.That(track.Images, Contains.Key(ImageSize.Large));
          Assert.That(track.Images, Contains.Key(ImageSize.ExtraLarge));
          Assert.That(track.Album, Is.Null);
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.PlayedAtUtc, Is.Null);
          Assert.That(track.IsNowPlaying, Is.False);
        }

        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.IsWellFormedOriginalString(), Is.True);
        }
      }
    }

    [Test]
    public async Task GetTopTracksAsync_Authenticated_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      var response = await client.User.GetTopTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var tracks = pages.Items;
      foreach (var track in tracks)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.UserPlayCount, Is.GreaterThan(1));
          Assert.That(track.Duration, Is.Not.Null);
          Assert.That(track.Images, Contains.Key(ImageSize.Small));
          Assert.That(track.Images, Contains.Key(ImageSize.Medium));
          Assert.That(track.Images, Contains.Key(ImageSize.Large));
          Assert.That(track.Images, Contains.Key(ImageSize.ExtraLarge));
          Assert.That(track.Album, Is.Null);
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.PlayedAtUtc, Is.Null);
          Assert.That(track.IsNowPlaying, Is.False);
        }

        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.IsWellFormedOriginalString(), Is.True);
        }
      }
    }

    [Test]
    public async Task GetTopTracksAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopTracksAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopTracksAsync

    #region GetRecentTracksAsync

    [Test]
    public async Task GetRecentTracksAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetRecentTracksAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var tracks = pages.Items;
      foreach (var track in tracks)
      {
        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString, Is.Not.Empty);
        }

        var album = track.Album;
        Assert.That(album, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Name, Is.Not.Empty);
          Assert.That(album.Url, Is.Null);
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          if (track.IsNowPlaying)
            Assert.That(track.PlayedAtUtc, Is.Null);
          else
            Assert.That(track.PlayedAtUtc, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.UserLoved, Is.Null);
          Assert.That(track.UserLovedDate, Is.Null);
          Assert.That(track.Images, Contains.Key(ImageSize.Small));
          Assert.That(track.Images, Contains.Key(ImageSize.Medium));
          Assert.That(track.Images, Contains.Key(ImageSize.Large));
          Assert.That(track.Images, Contains.Key(ImageSize.ExtraLarge));
        }
      }
    }

    [Test]
    public async Task GetRecentTracksAsync_Extended_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetRecentTracksAsync("coczero", extended: true);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var tracks = pages.Items;
      foreach (var track in tracks)
      {
        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString, Is.Not.Empty);
          Assert.That(artist.Images, Contains.Key(ImageSize.Small));
          Assert.That(artist.Images, Contains.Key(ImageSize.Medium));
          Assert.That(artist.Images, Contains.Key(ImageSize.Large));
          Assert.That(artist.Images, Contains.Key(ImageSize.ExtraLarge));
        }

        var album = track.Album;
        Assert.That(album, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Name, Is.Not.Empty);
          Assert.That(album.Url, Is.Null);

          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          if (track.IsNowPlaying)
            Assert.That(track.PlayedAtUtc, Is.Null);
          else
            Assert.That(track.PlayedAtUtc, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.UserLoved, Is.Not.Null);
          Assert.That(track.UserLovedDate, Is.Null);
          Assert.That(track.Images, Contains.Key(ImageSize.Small));
          Assert.That(track.Images, Contains.Key(ImageSize.Medium));
          Assert.That(track.Images, Contains.Key(ImageSize.Large));
          Assert.That(track.Images, Contains.Key(ImageSize.ExtraLarge));
        }
      }
    }

    [Test]
    public async Task GetRecentTracksAsync_From_To_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetRecentTracksAsync("coczero", extended: false, fromDate: new DateTime(2000, 1, 1), toDate: DateTime.Now);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      var tracks = pages.Items;
      foreach (var track in tracks)
      {
        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString, Is.Not.Empty);
        }

        var album = track.Album;
        Assert.That(album, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Name, Is.Not.Empty);
          Assert.That(album.Url, Is.Null);
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          if (track.IsNowPlaying)
            Assert.That(track.PlayedAtUtc, Is.Null);
          else
            Assert.That(track.PlayedAtUtc, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.UserLoved, Is.Null);
          Assert.That(track.UserLovedDate, Is.Null);
          Assert.That(track.Images, Contains.Key(ImageSize.Small));
          Assert.That(track.Images, Contains.Key(ImageSize.Medium));
          Assert.That(track.Images, Contains.Key(ImageSize.Large));
          Assert.That(track.Images, Contains.Key(ImageSize.ExtraLarge));
        }
      }
    }

    [Test]
    public async Task GetRecentTracksAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetRecentTracksAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetRecentTracksAsync

    #region GetTopTagsAsync

    [Test]
    public async Task GetTopTagsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopTagsAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var tag in response.Data)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.UserUsedCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        }
      }
    }

    [Test]
    public async Task GetTopTagsAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopTagsAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopTagsAsync

    #region GetTopArtistsAsync

    [Test]
    public async Task GetTopArtistsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopArtistsAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      foreach (var artist in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
          Assert.That(artist.IsStreamable, Is.Not.Null);
          Assert.That(artist.UserPlayCount, Is.GreaterThan(1));
          Assert.That(artist.PlayCount, Is.Null);
          Assert.That(artist.Images, Contains.Key(ImageSize.Small));
          Assert.That(artist.Images, Contains.Key(ImageSize.Medium));
          Assert.That(artist.Images, Contains.Key(ImageSize.Large));
          Assert.That(artist.Images, Contains.Key(ImageSize.ExtraLarge));
        }
      }
    }

    [Test]
    public async Task GetTopArtistsAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopArtistsAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopArtistsAsync

    #region GetTopAlbumsAsync

    [Test]
    public async Task GetTopAlbumsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopAlbumsAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      foreach (var album in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Name, Is.Not.Empty);
          Assert.That(album.Mbid, Is.Not.Null);
          Assert.That(album.Url!.ToString(), Is.Not.Empty);
          Assert.That(album.UserPlayCount, Is.GreaterThan(1));
          Assert.That(album.Images, Contains.Key(ImageSize.Small));
          Assert.That(album.Images, Contains.Key(ImageSize.Medium));
          Assert.That(album.Images, Contains.Key(ImageSize.Large));
          Assert.That(album.Images, Contains.Key(ImageSize.ExtraLarge));
        }
      }
    }

    [Test]
    public async Task GetTopAlbumsAsync_InvalidUser_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetTopAlbumsAsync("SHOEGAZELASTFMINVALIDUSER");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopAlbumsAsync

    #region GetWeeklyChartListAsync

    [Test]
    public async Task GetWeeklyChartListAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetWeeklyChartListAsync("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var chart in response.Data.Take(10))
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
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetWeeklyChartAsync<ArtistInfo>("coczero");
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

    [Test]
    public async Task GetWeeklyChartAsync_Album_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetWeeklyChartAsync<AlbumInfo>("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var album in response.Data.Take(10))
      {
        var artist = album.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
        }
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Name, Is.Not.Empty);
          Assert.That(album.Url!.ToString(), Is.Not.Empty);
          Assert.That(album.Mbid, Is.Not.Null);
          Assert.That(album.UserPlayCount, Is.GreaterThanOrEqualTo(1));
        }
      }
    }

    [Test]
    public async Task GetWeeklyChartAsync_Track_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetWeeklyChartAsync<TrackInfo>("coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var track in response.Data.Take(10))
      {
        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
        }
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.UserPlayCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.Images, Is.Not.Empty);
          Assert.That(track.Album, Is.Null);
        }
      }
    }

    #endregion GetWeeklyChartAsync

    #region GetPersonalTagsAsync

    [Test]
    public async Task GetPersonalTagsAsync_Artist_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetPersonalTagsAsync<ArtistInfo>("coczero", "shoegaze");
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
        }
      }
    }

    [Test]
    public async Task GetPersonalTagsAsync_Album_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetPersonalTagsAsync<AlbumInfo>("coczero", "shoegaze");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var album in response.Data.Take(10))
      {
        var artist = album.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
        }
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Name, Is.Not.Empty);
          Assert.That(album.Url!.ToString(), Is.Not.Empty);
          Assert.That(album.Mbid, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task GetPersonalTagsAsync_Track_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.User.GetPersonalTagsAsync<TrackInfo>("coczero", "shoegaze");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var track in response.Data.Take(10))
      {
        var artist = track.Artist;
        Assert.That(artist, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Name, Is.Not.Empty);
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.Url.ToString(), Is.Not.Empty);
        }
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Name, Is.Not.Empty);
          Assert.That(track.Url.ToString(), Is.Not.Empty);
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.Album, Is.Null);
        }
      }
    }

    #endregion GetPersonalTagsAsync
  }
}