using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class TrackApiIntegrationTests
  {
    #region GetInfoByNameAsync

    private static void AssertGetInfoTrackInfo(TrackInfo track, bool usernameIncluded)
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(track.Name, Is.EqualTo("Blind"));
        Assert.That(track.Mbid, Is.Not.Empty);
        Assert.That(track.Url.ToString(), Is.Not.Empty);
        Assert.That(track.Duration!.Value.TotalSeconds, Is.GreaterThan(1));
        Assert.That(track.IsStreamable, Is.Not.Null);
        Assert.That(track.ListenerCount, Is.GreaterThan(1));
        Assert.That(track.PlayCount, Is.GreaterThan(1));
        Assert.That(track.UserLovedDate, Is.Null);
        if (usernameIncluded)
        {
          Assert.That(track.UserLoved, Is.Not.Null);
          Assert.That(track.UserPlayCount, Is.Not.Null);
        }
        else
        {
          Assert.That(track.UserLoved, Is.Null);
          Assert.That(track.UserPlayCount, Is.Null);
        }
        Assert.That(track.PlayedAt, Is.Null);
        Assert.That(track.Images, Is.Empty);
      }

      var artist = track.Artist;
      Assert.That(artist, Is.Not.Null);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(artist.Name, Is.EqualTo("Korn"));
        Assert.That(artist.Mbid, Is.Not.Empty);
        Assert.That(artist.Url.ToString(), Is.Not.Empty);
        Assert.That(artist.Images, Is.Empty);
      }

      var album = track.Album;
      Assert.That(album, Is.Not.Null);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(album.Name, Is.EqualTo("Korn"));
        Assert.That(album.Mbid, Is.Not.Empty);
        Assert.That(album.Url!.ToString(), Is.Not.Empty);
        Assert.That(album.Images, Is.Not.Empty);
      }

      var tags = track.TopTags;
      Assert.That(tags, Is.Not.Null);
      foreach (var tag in tags)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        }
      }

      var wiki = track.Wiki;
      Assert.That(wiki, Is.Not.Null);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(wiki.Published, Is.Not.EqualTo(default(DateTime)));
        Assert.That(wiki.Content, Is.Not.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
      }
    }

    [Test]
    public async Task GetInfoByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByNameAsync("Blind", "Korn");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoTrackInfo(response.Data, false);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByNameAsync("Blind", "Korn", "coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoTrackInfo(response.Data, true);
    }

    #endregion GetInfoByNameAsync

    #region GetInfoByMbidAsync

    [Test]
    public async Task GetInfoByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByMbidAsync("7bc25578-9469-4f46-97ce-1871d9ce1c69");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoTrackInfo(response.Data, false);
    }

    [Test]
    public async Task GetInfoByMbidAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByMbidAsync("7bc25578-9469-4f46-97ce-1871d9ce1c69", "coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoTrackInfo(response.Data, true);
    }

    #endregion GetInfoByMbidAsync

    #region GetCorrectionAsync

    [Test]
    public async Task GetCorrectionAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetCorrectionAsync("Mrbrownstone", "guns and roses");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var track = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(track.Name, Is.EqualTo("Mr. Brownstone"));
        Assert.That(track.Mbid, Is.Not.Empty);
        Assert.That(track.Url.ToString(), Is.Not.Empty);
      }

      var artist = track.Artist;
      Assert.That(artist, Is.Not.Null);
      Assert.That(artist.Name, Is.EqualTo("Guns N' Roses"));
    }


    #endregion GetCorrectionAsync

    #region GetSimilarByNameAsync

    [Test]
    public async Task GetSimilarByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetSimilarByNameAsync("Blind", "Korn");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var track in response.Data)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.PlayCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.Match, Is.Not.Zero);
        }
      }
    }

    #endregion GetSimilarByNameAsync

    #region GetSimilarByMbidAsync

    [Test]
    public async Task GetSimilarByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetSimilarByMbidAsync("7bc25578-9469-4f46-97ce-1871d9ce1c69");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var track in response.Data)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.PlayCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.Match, Is.Not.Zero);
        }
      }
    }

    #endregion GetSimilarByMbidAsync

    #region GetUserTagsByNameAsync

    [Test]
    public async Task GetUserTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetUserTagsByName("soon", "my bloody valentine", "coczero");
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
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        }
      }
    }

    #endregion GetUserTagsByNameAsync

    #region GetUserTagsByMbidAsync

    [Test]
    public async Task GetUserTagsByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetUserTagsByMbid("55f39cd9-9326-3ca0-bd7c-ed21f61b30b5", "coczero");
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
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        }
      }
    }

    #endregion GetUserTagsByMbidAsync

    #region GetTopTagsByNameAsync

    [Test]
    public async Task GetTopTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetTopTagsByName("Blind", "Korn");
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
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
          Assert.That(tag.CountOnTrack, Is.GreaterThanOrEqualTo(1));
          Assert.That(tag.UserUsedCount, Is.Null);
        }
      }
    }

    #endregion GetTopTagsByNameAsync

    #region SearchAsync

    [Test]
    public async Task SearchAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.SearchAsync("Blind");
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

      Assert.That(pages.Items, Has.Count.GreaterThan(1));
      foreach (var track in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.ListenerCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.UserPlayCount, Is.Null);
        }
      }
    }

    [Test]
    public async Task SearchAsync_With_Artist_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.SearchAsync("Blind", "Korn");
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

      Assert.That(pages.Items, Has.Count.GreaterThan(1));
      foreach (var track in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.Images, Is.Not.Empty);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.ListenerCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.UserPlayCount, Is.Null);
        }
      }
    }

    [Test]
    public async Task SearchAsync_Invalid_Track_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.SearchAsync("SHOEGAZELASTFMINVALIDTRACK");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.Zero);
        Assert.That(pages.TotalItems, Is.Zero);
      }

      Assert.That(pages.Items, Is.Empty);
    }

    #endregion SearchAsync

    #region AddTagsAsync

    [Test, NonParallelizable]
    public async Task AddTagsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      // check initial state
      var userTags = await client.Track.GetUserTagsByName("Blind", "Korn");
      Assume.That(userTags.Data, Is.Empty, "Initial state is not correct.");

      try
      {
        var response = await client.Track.AddTagsAsync("Blind", "Korn", ["Nu Metal", "Metal"]);
        Assert.That(response.IsSuccess, Is.True);

        userTags = await client.Track.GetUserTagsByName("Blind", "Korn");
        Assert.That(userTags.Data, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
          Assert.That(userTags.Data.Any(t => t.Name.Equals("nu metal", StringComparison.CurrentCultureIgnoreCase)), Is.True);
          Assert.That(userTags.Data.Any(t => t.Name.Equals("metal", StringComparison.CurrentCultureIgnoreCase)), Is.True);
        }
      }
      catch (Exception ex)
      {
        TestContext.Error.WriteLine(ex.Message);
        Assert.Fail();
      }
      finally
      {
        // cleanup
        try
        {
          await client.Track.RemoveTagsAsync("Blind", "Korn", ["Nu Metal", "Metal"]);
        }
        catch (Exception ex)
        {
          TestContext.Error.WriteLine($"Test cleanup failed: {ex.Message}");
        }
      }
    }

    #endregion AddTagsAsync

    #region RemoveTagsAsync

    [Test, NonParallelizable]
    public async Task RemoveTagsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      await client.Track.AddTagsAsync("Blind", "Korn", ["Nu Metal", "Metal"]);

      // check initial state
      var userTags = await client.Track.GetUserTagsByName("Blind", "Korn");
      Assume.That(userTags.Data, Has.Count.EqualTo(2), "Initial state is not correct.");

      try
      {
        var response = await client.Track.RemoveTagsAsync("Blind", "Korn", ["Nu Metal", "Metal"]);
        Assert.That(response.IsSuccess, Is.True);

        userTags = await client.Track.GetUserTagsByName("Blind", "Korn");
        Assert.That(userTags.Data, Is.Empty);
      }
      catch (Exception ex)
      {
        TestContext.Error.WriteLine(ex.Message);
        Assert.Fail();
      }
    }

    #endregion RemoveTagsAsync

    #region SetLoveStateAsync

    [Test, NonParallelizable]
    public async Task SetLoveStateAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      var trackResponse = await client.Track.GetInfoByNameAsync("Blind", "Korn", "coctest");
      Assume.That(trackResponse.Data, Is.Not.Null, "Initial state could not be checked.");
      Assume.That(trackResponse.Data.UserLoved, Is.False, "Initial state is not correct.");

      try
      {
        var response = await client.Track.SetLoveState("Blind", "Korn", loveState: true);
        Assert.That(response.IsSuccess, Is.True);
        trackResponse = await client.Track.GetInfoByNameAsync("Blind", "Korn", "coctest");
        Assert.That(trackResponse.Data, Is.Not.Null);
        Assert.That(trackResponse.Data.UserLoved, Is.True);

        response = await client.Track.SetLoveState("Blind", "Korn", loveState: false);
        Assert.That(response.IsSuccess, Is.True);
        trackResponse = await client.Track.GetInfoByNameAsync("Blind", "Korn", "coctest");
        Assert.That(trackResponse.Data, Is.Not.Null);
        Assert.That(trackResponse.Data.UserLoved, Is.False);
      }
      catch (Exception ex)
      {
        TestContext.Error.WriteLine(ex.Message);
        Assert.Fail();
      }
      finally
      {
        // best effor clean up
        await client.Track.SetLoveState("Blind", "Korn", loveState: false);
      }
    }

    #endregion SetLoveStateAsync

    #region UpdateNowPlayingAsync

    [Test, NonParallelizable]
    public async Task UpdateNowPlayingAsync_IntegrationTest()
    {
      var timestampFile = "last_now_playing_test_time.txt";
      if (File.Exists(timestampFile))
      {
        var lastRun = DateTime.Parse(File.ReadAllText(timestampFile), null, System.Globalization.DateTimeStyles.RoundtripKind);
        if (DateTime.UtcNow - lastRun < TimeSpan.FromMinutes(5))
          Assert.Ignore($"Test skipped — last run was {DateTime.UtcNow - lastRun:g} ago (must be at least 5 minutes).");
      }

      // store the current run time
      File.WriteAllText(timestampFile, DateTime.UtcNow.ToString("o"));

      var client = TestEnvironment.CreateAuthenticatedClient();

      var initialCurrentTracksResponse = await client.User.GetRecentTracksAsync(null, null, null, null, limit: 1);
      Assume.That(initialCurrentTracksResponse.IsSuccess, Is.True, "Initial state could not be checked.");
      Assume.That(initialCurrentTracksResponse.Data, Is.Not.Null, "Initial state could not be checked.");
      Assume.That(initialCurrentTracksResponse.Data.Items[0].IsNowPlaying, Is.Null.Or.False, "Initial state is not correct.");

      var response = await client.Track.UpdateNowPlayingAsync("Blind", "Korn", "Korn", "Korn");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      // do now playing check immediately after
      var currentTracksResponse = await client.User.GetRecentTracksAsync(null, null, null, null, limit: 1);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(currentTracksResponse.IsSuccess, Is.True);
        Assert.That(currentTracksResponse.Data, Is.Not.Null);
      }
      Assert.That(currentTracksResponse.Data.Items[0].IsNowPlaying, Is.True);

      // then check info
      var info = response.Data!;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(info.TrackName, Is.EqualTo("Blind"));
        Assert.That(info.IsTrackNameCorrected, Is.False);
        Assert.That(info.ArtistName, Is.EqualTo("Korn"));
        Assert.That(info.IsArtistNameCorrected, Is.False);
        Assert.That(info.AlbumName, Is.EqualTo("Korn"));
        Assert.That(info.IsAlbumNameCorrected, Is.False);
        Assert.That(info.AlbumArtistName, Is.EqualTo("Korn"));
        Assert.That(info.IsAlbumArtistNameCorrected, Is.False);
        Assert.That(info.IsIgnored, Is.False);
        Assert.That(info.IgnoredStatusCode, Is.EqualTo(IgnoredCode.None));
        Assert.That(info.IgnoredMessage, Is.Null);
      }
    }

    #endregion UpdateNowPlayingAsync
  }
}