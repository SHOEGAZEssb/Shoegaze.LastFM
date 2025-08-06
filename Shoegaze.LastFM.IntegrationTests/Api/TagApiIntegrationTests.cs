namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class TagApiIntegrationTests
  {
    #region GetInfoAsync

    [Test]
    public async Task GetInfoAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetInfoAsync("shoegaze");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var tag = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(tag.Reach, Is.GreaterThan(1));
        Assert.That(tag.Taggings, Is.GreaterThan(1));
        Assert.That(tag.UserUsedCount, Is.Null);
        Assert.That(tag.CountOnTrack, Is.Null);
      }

      var wiki = tag.Wiki;
      Assert.That(wiki, Is.Not.Null);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(wiki.Content, Is.Not.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
        Assert.That(wiki.Published, Is.Null);
      }
    }

    [Test]
    public async Task GetInfoAsync_Invalid_Tag_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetInfoAsync("SHOEGAZELASTFMINVALIDTAG");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var tag = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(tag.Reach, Is.Zero);
        Assert.That(tag.Taggings, Is.Zero);
        Assert.That(tag.UserUsedCount, Is.Null);
        Assert.That(tag.CountOnTrack, Is.Null);
      }

      var wiki = tag.Wiki;
      Assert.That(wiki, Is.Not.Null);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(wiki.Content, Is.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
        Assert.That(wiki.Published, Is.Null);
      }
    }

    #endregion GetInfoAsync

    #region GetSimilarAsync

    /// <summary>
    /// Tests the currently broken behaviour of tag.GetSimilar
    /// (always returns an empty tag array).
    /// Once this test fails the api works again hopefully.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task GetSimilarAsync_Currently_Broken_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetSimilarAsync("shoegaze");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      Assert.That(response.Data, Is.Empty);
    }

    #endregion GetSimilarAsync

    #region GetTopAlbumsAsync

    [Test]
    public async Task GetTopAlbumsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopAlbumsAsync("shoegaze");
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
          Assert.That(album.Url, Is.Not.Null);
          Assert.That(album.Images, Is.Not.Empty);
          Assert.That(album.Artist, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task GetTopAlbumsAsync_With_Limit_And_PageIntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopAlbumsAsync("shoegaze", limit: 10, page: 2);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      var pages = response.Data;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(pages.Page, Is.EqualTo(2));
        Assert.That(pages.TotalPages, Is.GreaterThan(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      }

      Assert.That(pages.Items, Has.Count.EqualTo(10));

      foreach (var album in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(album.Url, Is.Not.Null);
          Assert.That(album.Images, Is.Not.Empty);
          Assert.That(album.Artist, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task GetTopAlbumsAsync_Invalid_Tag_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopAlbumsAsync("SHOEGAZELASTFMINVALIDTAG");
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
        Assert.That(pages.TotalPages, Is.Zero);
      }

      Assert.That(pages.Items, Is.Empty);
    }

    #endregion GetTopAlbumsAsync

    #region GetTopArtistsAsync

    [Test]
    public async Task GetTopArtistsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopArtistsAsync("shoegaze");
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
          Assert.That(artist.Url, Is.Not.Null);
          Assert.That(artist.Images, Is.Not.Empty);
          Assert.That(artist.IsStreamable, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task GetTopArtistsAsync_Invalid_Tag_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopArtistsAsync("SHOEGAZELASTFMINVALIDTAG");
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
        Assert.That(pages.TotalPages, Is.Zero);
      }

      Assert.That(pages.Items, Is.Empty);
    }

    #endregion GetTopArtistsAsync

    #region GetTopTagsAsync

    [Test]
    public async Task GetTopTagsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopTagsAsync();
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
        Assert.That(pages.PerPage, Is.EqualTo(50));
      }

      Assert.That(pages.Items, Has.Count.EqualTo(pages.PerPage));
      foreach (var tag in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.Reach, Is.Not.Null);
          Assert.That(tag.Taggings, Is.Not.Null);
          Assert.That(tag.CountOnTrack, Is.Null);
          Assert.That(tag.Wiki, Is.Null);
        }
      }
    }

    [Test]
    public async Task GetTopTagsAsync_With_Limit_And_Page_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopTagsAsync(limit: 100, page: 3);
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
        Assert.That(pages.PerPage, Is.EqualTo(100));
      }

      Assert.That(pages.Items, Has.Count.EqualTo(pages.PerPage));
      foreach (var tag in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.Reach, Is.Not.Null);
          Assert.That(tag.Taggings, Is.Not.Null);
          Assert.That(tag.CountOnTrack, Is.Null);
          Assert.That(tag.Wiki, Is.Null);
        }
      }
    }

    #endregion GetTopTagsAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopTracksAsync("shoegaze");
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

      foreach (var track in pages.Items)
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.Url, Is.Not.Null);
          Assert.That(track.Images, Is.Not.Empty);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.Duration, Is.Not.Null);
        }
      }
    }

    #endregion GetTopTracksAsync

    #region GetWeeklyChartListAsync

    [Test]
    public async Task GetWeeklyChartListAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetWeeklyChartListAsync("shoegaze");
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
  }
}
