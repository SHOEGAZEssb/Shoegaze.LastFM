using Shoegaze.LastFM.Artist;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class ArtistApiIntegrationTests
  {
    /// <summary>
    /// Safety buffer for write operations.
    /// </summary>
    private static readonly TimeSpan SAFETYBUFFER = TimeSpan.FromSeconds(3);

    private static void AssertGetInfoArtist(ArtistInfo artist, bool withUserInfo)
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(artist.Name, Is.Not.Empty);
        Assert.That(artist.Url.ToString(), Is.Not.Empty);
        Assert.That(artist.Images, Has.Count.EqualTo(6));
        Assert.That(artist.IsStreamable, Is.Not.Null);
        Assert.That(artist.OnTour, Is.Not.Null);
        Assert.That(artist.ListenerCount, Is.GreaterThan(1));
        Assert.That(artist.PlayCount, Is.GreaterThan(1));
        if (withUserInfo)
          Assert.That(artist.UserPlayCount, Is.GreaterThan(1));
        else
          Assert.That(artist.UserPlayCount, Is.Null);
      }

      var similar = artist.SimilarArtists;
      Assert.That(similar, Is.Not.Null);
      Assert.That(similar, Has.Count.EqualTo(5));
      foreach (var sa in similar)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(sa.Name, Is.Not.Empty);
          Assert.That(sa.Url.ToString(), Is.Not.Empty);
          Assert.That(artist.Images, Has.Count.EqualTo(6));
        }
      }

      var tags = artist.Tags;
      Assert.That(tags, Is.Not.Null);
      Assert.That(tags, Has.Count.EqualTo(5));
      foreach (var tag in tags)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        }
      }

      var bio = artist.Biography;
      Assert.That(bio, Is.Not.Null);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(bio.Published, Is.Not.Null);
        Assert.That(bio.Published, Is.Not.EqualTo(default(DateTime)));
        Assert.That(bio.Summary, Is.Not.Empty);
        Assert.That(bio.Content, Is.Not.Empty);
      }
    }

    #region GetInfoByNameAsync

    [Test]
    public async Task GetInfoByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("My Bloody Valentine");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoArtist(response.Data, withUserInfo: false);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("My Bloody Valentine", "coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoArtist(response.Data, withUserInfo: true);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Correction_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("guns and roses", "coczero", autoCorrect: true);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var artist = response.Data;
      Assert.That(artist.Name, Is.EqualTo("Guns N' Roses"));
      AssertGetInfoArtist(artist, withUserInfo: true);
    }

    [Test]
    public async Task GetInfoByNameAsync_Without_Correction_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("guns and roses", "coczero", autoCorrect: false);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var artist = response.Data;
      Assert.That(artist.Name, Is.EqualTo("Guns and Roses"));
    }

    [Test]
    public async Task GetInfoByNameAsync_Invalid_Artist_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("SHOEGAZELASTFMINVALIDARTIST");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.LastFmStatus, Is.EqualTo(LastFmStatusCode.InvalidParameters));
      }
    }

    #endregion GetInfoByNameAsync

    #region GetInfoByMbidAsync

    [Test]
    public async Task GetInfoByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByMbidAsync("ac865b2e-bba8-4f5a-8756-dd40d5e39f46");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoArtist(response.Data, withUserInfo: false);
    }

    [Test]
    public async Task GetInfoByMbidAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByMbidAsync("ac865b2e-bba8-4f5a-8756-dd40d5e39f46", "coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoArtist(response.Data, withUserInfo: true);
    }

    #endregion GetInfoByMbidAsync

    #region GetSimilarByNameAsync

    [Test]
    public async Task GetSimilarByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetSimilarByNameAsync("My Bloody Valentine");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.EqualTo(100));
      foreach (var artist in response.Data.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Match, Is.Not.Zero);
          Assert.That(artist.Images, Is.Not.Empty);
        }
      }
    }

    #endregion GetSimilarByNameAsync

    #region GetSimilarByNameAsync

    [Test]
    public async Task GetSimilarByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetSimilarByMbidAsync("ac865b2e-bba8-4f5a-8756-dd40d5e39f46");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Has.Count.EqualTo(100));
      foreach (var artist in response.Data.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Match, Is.Not.Zero);
          Assert.That(artist.Images, Is.Not.Empty);
        }
      }
    }

    #endregion GetSimilarByNameAsync

    #region GetCorrectionAsync

    [Test]
    public async Task GetCorrectionAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetCorrectionAsync("guns and roses");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var artist = response.Data;
      Assert.That(artist.Name, Is.EqualTo("Guns N' Roses"));
    }

    #endregion GetCorrectionAsync

    #region GetTagsByNameAsync

    [Test]
    public async Task GetTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTagsByNameAsync("My Bloody Valentine", "coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "shoegaze"), Is.True);
    }

    [Test]
    public async Task GetTagsByNameAsync_Authenticated_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      var response = await client.Artist.GetTagsByNameAsync("My Bloody Valentine");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "shoegaze"), Is.True);
    }

    [Test]
    public async Task GetTagsByNameAsync_Authenticated_Empty_Without_Correction_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      var response = await client.Artist.GetTagsByNameAsync("guns and roses", username: null, autoCorrect: false);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Empty);
    }

    #endregion GetTagsByNameAsync

    #region GetTopAlbumsByNameAsync

    [Test]
    public async Task GetTopAlbumsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTopAlbumsByNameAsync("My Bloody Valentine");
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

      Assert.That(pages.Items, Is.Not.Empty);
      Assert.That(pages.Items.Any(static t => t.Name == "Loveless"), Is.True);
      foreach (var album in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.PlayCount, Is.GreaterThan(1));
          Assert.That(album.UserPlayCount, Is.Null);
        }
      }
    }

    [Test]
    public async Task GetTopAlbumsByNameAsync_Without_Correction_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTopAlbumsByNameAsync("guns and roses", autoCorrect: false);
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

      Assert.That(pages.Items, Is.Not.Empty);
      foreach (var album in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.PlayCount, Is.GreaterThan(1));
          Assert.That(album.UserPlayCount, Is.Null);
        }
      }
    }

    #endregion GetTopAlbumsByNameAsync

    #region GetTopTagsByNameAsync

    [Test]
    public async Task GetTopTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTopTagsByNameAsync("My Bloody Valentine");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "shoegaze"), Is.True);
    }

    #endregion GetTopTagsByNameAsync

    #region GetTopTagsByMbidAsync

    [Test]
    public async Task GetTopTagsByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTopTagsByMbidAsync("65f4f0c5-ef9e-490c-aee3-909e7ae6b2ab"); // metallica mbid
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "metal"), Is.True);
    }

    #endregion GetTopTagsByMbidAsync

    #region GetTopTracksByNameAsync

    [Test]
    public async Task GetTopTracksByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTopTracksByNameAsync("My Bloody Valentine");
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

      foreach (var track in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.PlayCount, Is.GreaterThan(1));
          Assert.That(track.ListenerCount, Is.GreaterThan(1));
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.UserPlayCount, Is.Null);
          Assert.That(track.Artist, Is.Not.Null);
        }
      }
    }

    #endregion GetTopTracksByNameAsync

    #region GetTopTracksByMbidAsync

    [Test]
    public async Task GetTopTracksByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetTopTracksByMbidAsync("65f4f0c5-ef9e-490c-aee3-909e7ae6b2ab"); // metallica mbid
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

      foreach (var track in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.PlayCount, Is.GreaterThan(1));
          Assert.That(track.ListenerCount, Is.GreaterThan(1));
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.UserPlayCount, Is.Null);
          Assert.That(track.Artist, Is.Not.Null);
        }
      }
    }

    #endregion GetTopTracksByNameAsync

    #region SearchAsync

    [Test]
    public async Task SearchAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.SearchAsync("Korn");
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
      foreach (var artist in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.IsStreamable, Is.Not.Null);
          Assert.That(artist.ListenerCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(artist.PlayCount, Is.Null);
          Assert.That(artist.UserPlayCount, Is.Null);
        }
      }
    }

    [Test]
    public async Task SearchAsync_With_Limit_And_Page_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.SearchAsync("A", limit: 10, page: 3);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(3));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
        Assert.That(pages.PerPage, Is.EqualTo(10));
      }

      Assert.That(pages.Items, Has.Count.GreaterThan(1));
      foreach (var artist in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.Mbid, Is.Not.Null);
          Assert.That(artist.IsStreamable, Is.Not.Null);
          Assert.That(artist.ListenerCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(artist.PlayCount, Is.Null);
          Assert.That(artist.UserPlayCount, Is.Null);
        }
      }
    }

    [Test]
    public async Task SearchAsync_Invalid_Track_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.SearchAsync("SHOEGAZELASTFMINVALIDARTIST");
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
      var userTags = await client.Artist.GetTagsByNameAsync("Korn");
      Assume.That(userTags.Data, Is.Empty, "Initial state is not correct.");

      try
      {
        var response = await client.Artist.AddTagsAsync("Korn", "Nu Metal");
        Assert.That(response.IsSuccess, Is.True);

        await Task.Delay(SAFETYBUFFER);

        userTags = await client.Artist.GetTagsByNameAsync("Korn");
        Assert.That(userTags.Data, Has.Count.EqualTo(1));
        Assert.That(userTags.Data[0].Name, Is.EqualTo("Nu Metal"));
      }
      catch(Exception ex)
      {
        TestContext.Error.WriteLine(ex.Message);
        Assert.Fail();
      }
      finally
      {
        // cleanup
        try
        {
          await client.Artist.RemoveTagsAsync("Korn", "Nu Metal");
        }
        catch (Exception ex)
        {
          TestContext.Error.WriteLine($"Test cleanup failed: {ex.Message}");
        }
      }
    }

    #endregion AddTagsAsync

    #region RemoveTagAsync

    [Test, NonParallelizable]
    public async Task RemoveTagAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateAuthenticatedClient();

      await client.Artist.AddTagsAsync("Korn", "Nu Metal");

      // check initial state
      var userTags = await client.Artist.GetTagsByNameAsync("Korn");
      Assume.That(userTags.Data, Has.Count.EqualTo(1), "Initial state is not correct.");

      try
      {
        var response = await client.Artist.RemoveTagsAsync("Korn", "Nu Metal");
        Assert.That(response.IsSuccess, Is.True);

        await Task.Delay(SAFETYBUFFER);

        userTags = await client.Artist.GetTagsByNameAsync("Korn");
        Assert.That(userTags.Data, Is.Empty);
      }
      catch (Exception ex)
      {
        TestContext.Error.WriteLine(ex.Message);
        Assert.Fail();
      }
    }

    #endregion RemoveTagAsync
  }
}