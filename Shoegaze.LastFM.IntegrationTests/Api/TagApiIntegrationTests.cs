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
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var tag = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(tag.Reach, Is.GreaterThan(1));
        Assert.That(tag.Taggings, Is.GreaterThan(1));
        Assert.That(tag.UserUsedCount, Is.Null);
        Assert.That(tag.Count, Is.Null);
      });

      var wiki = tag.Wiki;
      Assert.That(wiki, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(wiki.Content, Is.Not.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
        Assert.That(wiki.Published, Is.Null);
      });
    }

    [Test]
    public async Task GetInfoAsync_Invalid_Tag_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetInfoAsync("SHOEGAZELASTFMINVALIDTAG");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var tag = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(tag.Reach, Is.EqualTo(0));
        Assert.That(tag.Taggings, Is.EqualTo(0));
        Assert.That(tag.UserUsedCount, Is.Null);
        Assert.That(tag.Count, Is.Null);
      });

      var wiki = tag.Wiki;
      Assert.That(wiki, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(wiki.Content, Is.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
        Assert.That(wiki.Published, Is.Null);
      });
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
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Is.Empty);
    }

    #endregion GetSimilarAsync

    #region GetTopAlbumsAsync

    [Test]
    public async Task GetTopAlbumsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetTopAlbumsAsync("shoegaze");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var pages = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.GreaterThanOrEqualTo(1));
        Assert.That(pages.TotalItems, Is.GreaterThan(1));
      });

      int i = 1;
      foreach (var album in pages.Items)
      {
        Assert.Multiple(() =>
        {
          Assert.That(album.Url, Is.Not.Null);
          Assert.That(album.Images, Is.Not.Empty);
          Assert.That(album.Rank, Is.EqualTo(i++));
          Assert.That(album.Artist, Is.Not.Null);
        });
      }
    }

    #endregion GetTopAlbumsAsync
  }
}
