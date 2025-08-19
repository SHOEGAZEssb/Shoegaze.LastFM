using Shoegaze.LastFM.Album;
using System.Net;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  internal class AlbumApiIntegrationTests
  {
    private static void AssertGetInfoAlbum(AlbumInfo album, bool withUserInfo)
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(album.PlayCount, Is.GreaterThan(1));
        if (withUserInfo)
          Assert.That(album.UserPlayCount, Is.GreaterThan(1));
        else
          Assert.That(album.UserPlayCount, Is.Null);
        Assert.That(album.Images, Is.Not.Empty);
        Assert.That(album.Artist, Is.Not.Null);
        Assert.That(album.TopTags, Is.Not.Empty);
        Assert.That(album.Tracks, Is.Not.Empty);
        Assert.That(album.Wiki, Is.Not.Null);
      }
    }

    #region GetInfoByNameAsync

    [Test]
    public async Task GetInfoByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetInfoByNameAsync("Loveless", "My Bloody Valentine");

      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoAlbum(response.Data, false);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetInfoByNameAsync("Loveless", "My Bloody Valentine", "coczero");

      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoAlbum(response.Data, true);
    }

    #endregion GetInfobyNameAsync

    #region GetInfoByMbidAsync

    [Test]
    public async Task GetInfoByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetInfoByMbidAsync("09a09037-1ebd-3d38-a128-38ab11e6b0fe"); // mbv loveless mbid

      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoAlbum(response.Data, false);
    }

    [Test]
    public async Task GetInfoByMbidAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetInfoByMbidAsync("09a09037-1ebd-3d38-a128-38ab11e6b0fe", "coczero"); // mbv loveless mbid

      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      AssertGetInfoAlbum(response.Data, true);
    }

    #endregion GetInfoByMbidAsync

    #region GetTagsByNameAsync

    [Test]
    public async Task GetTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetTagsByNameAsync("loveless", "My Bloody Valentine", "coczero");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "shoegaze"), Is.True);

    }

    #endregion GetTagsByNameAsync

    #region GetTagsByMbidAsync

    [Test]
    public async Task GetTagsByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetTagsByMbidAsync("09a09037-1ebd-3d38-a128-38ab11e6b0fe", "coczero"); // loveless mbv mbid
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "shoegaze"), Is.True);
    }

    #endregion GetTagsByMbidAsync

    #region GetTopTagsByNameAsync

    [Test]
    public async Task GetTopTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetTopTagsByNameAsync("loveless", "My Bloody Valentine");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Not.Empty);
      Assert.That(response.Data.Any(static t => t.Name == "shoegaze"), Is.True);
      foreach (var tag in response.Data)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.WeightOnAlbum, Is.GreaterThanOrEqualTo(1));
          Assert.That(tag.CountOnTrack, Is.Null);
          Assert.That(tag.Taggings, Is.Null);
          Assert.That(tag.Reach, Is.Null);
          Assert.That(tag.UserUsedCount, Is.Null);
        }
      }
    }

    [Test]
    public async Task GetTopTagsByNameAsync_Invalid_Album_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.GetTopTagsByNameAsync("SHOEGAZELASTFMINVALIDALBUM", "SHOEGAZELASTFMINVALIDARTIST");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.LastFmStatus, Is.EqualTo(LastFmStatusCode.InvalidParameters));
      }
    }

    #endregion GetTopTagsByNameAsync

    #region SearchAsync

    [Test]
    public async Task SearchAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.SearchAsync("Loveless");
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
      foreach (var album in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Mbid, Is.Not.Null);
          Assert.That(album.IsStreamable, Is.Not.Null);
          Assert.That(album.PlayCount, Is.Null);
          Assert.That(album.UserPlayCount, Is.Null);
          Assert.That(album.Artist, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task SearchAsync_With_Limit_And_Page_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.SearchAsync("A", limit: 10, page: 3);
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
      foreach (var album in pages.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Mbid, Is.Not.Null);
          Assert.That(album.IsStreamable, Is.Not.Null);
          Assert.That(album.PlayCount, Is.Null);
          Assert.That(album.UserPlayCount, Is.Null);
          Assert.That(album.Artist, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task SearchAsync_Invalid_Track_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Album.SearchAsync("SHOEGAZELASTFMINVALIDARTIST");
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
      var userTags = await client.Album.GetTagsByNameAsync("Korn", "Korn");
      Assume.That(userTags.Data, Is.Empty, "Initial state is not correct.");

      try
      {
        var response = await client.Album.AddTagsAsync("Korn", "Korn", ["Nu Metal", "Metal"]);
        Assert.That(response.IsSuccess, Is.True);

        userTags = await client.Album.GetTagsByNameAsync("Korn", "Korn");
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
          await client.Album.RemoveTagsAsync("Korn", "Korn", ["Nu Metal", "Metal"]);
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

      await client.Album.AddTagsAsync("Korn", "Korn", ["Nu Metal", "Metal"]);

      // check initial state
      var userTags = await client.Album.GetTagsByNameAsync("Korn", "Korn");
      Assume.That(userTags.Data, Has.Count.EqualTo(2), "Initial state is not correct.");

      try
      {
        var response = await client.Album.RemoveTagsAsync("Korn", "Korn", ["Nu Metal", "Metal"]);
        Assert.That(response.IsSuccess, Is.True);

        userTags = await client.Album.GetTagsByNameAsync("Korn", "Korn");
        Assert.That(userTags.Data, Is.Empty);
      }
      catch (Exception ex)
      {
        TestContext.Error.WriteLine(ex.Message);
        Assert.Fail();
      }
    }

    #endregion RemoveTagsAsync
  }
}