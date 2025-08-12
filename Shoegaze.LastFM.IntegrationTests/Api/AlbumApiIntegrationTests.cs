using Shoegaze.LastFM.Album;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  internal class AlbumApiIntegrationTests
  {
    private static void AssertGetInfoAlbum(AlbumInfo album, bool withUserInfo)
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(album.Mbid, Is.Not.Null);
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
  }
}