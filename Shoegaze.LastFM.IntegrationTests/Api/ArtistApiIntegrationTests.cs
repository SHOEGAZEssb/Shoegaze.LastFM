using Shoegaze.LastFM.Artist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class ArtistApiIntegrationTests
  {
    private static void AssertGetInfoArtist(ArtistInfo artist, bool withUserInfo)
    {
      Assert.Multiple(() =>
      {
        Assert.That(artist.Name, Is.Not.Empty);
        Assert.That(artist.Url.ToString(), Is.Not.Empty);
        Assert.That(artist.Images, Has.Count.EqualTo(6));
        Assert.That(artist.IsStreamable, Is.Not.Null);
        Assert.That(artist.OnTour, Is.Not.Null);
        Assert.That(artist.Listeners, Is.GreaterThan(1));
        Assert.That(artist.PlayCount, Is.GreaterThan(1));
        if (withUserInfo)
          Assert.That(artist.UserPlayCount, Is.GreaterThan(1));
        else
          Assert.That(artist.UserPlayCount, Is.Null);
      });

      var similar = artist.SimilarArtists;
      Assert.That(similar, Is.Not.Null);
      Assert.That(similar, Has.Count.EqualTo(5));
      foreach (var sa in similar)
      {
        Assert.Multiple(() =>
        {
          Assert.That(sa.Name, Is.Not.Empty);
          Assert.That(sa.Url.ToString(), Is.Not.Empty);
          Assert.That(artist.Images, Has.Count.EqualTo(6));
        });
      }

      var tags = artist.Tags;
      Assert.That(tags, Is.Not.Null);
      Assert.That(tags, Has.Count.EqualTo(5));
      foreach (var tag in tags)
      {
        Assert.Multiple(() =>
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        });
      }

      var bio = artist.Biography;
      Assert.That(bio, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(bio.Published, Is.Not.Null);
        Assert.That(bio.Published, Is.Not.EqualTo(default(DateTime)));
        Assert.That(bio.Summary, Is.Not.Empty);
        Assert.That(bio.Content, Is.Not.Empty);
      });
    }

    #region GetInfoByNameAsync

    [Test]
    public async Task GetInfoByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("My Bloody Valentine");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoArtist(response.Data, withUserInfo: false);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("My Bloody Valentine", "coczero");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoArtist(response.Data, withUserInfo: true);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Correction_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("guns and roses", "coczero", autoCorrect: true);
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var artist = response.Data;
      Assert.That(artist.Name, Is.EqualTo("Guns N' Roses"));
      AssertGetInfoArtist(artist, withUserInfo: true);
    }

    [Test]
    public async Task GetInfoByNameAsync_Without_Correction_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("guns and roses", "coczero", autoCorrect: false);
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var artist = response.Data;
      Assert.That(artist.Name, Is.EqualTo("Guns and Roses"));
    }

    [Test]
    public async Task GetInfoByNameAsync_Invalid_Artist_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByNameAsync("SHOEGAZELASTFMINVALIDARTIST");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.Status, Is.EqualTo(LastFmStatusCode.InvalidParameters));
      });
    }

    #endregion GetInfoByNameAsync

    #region GetInfoByMbidAsync

    [Test]
    public async Task GetInfoByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByMbidAsync("ac865b2e-bba8-4f5a-8756-dd40d5e39f46");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoArtist(response.Data, withUserInfo: false);
    }

    [Test]
    public async Task GetInfoByMbidAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetInfoByMbidAsync("ac865b2e-bba8-4f5a-8756-dd40d5e39f46", "coczero");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoArtist(response.Data, withUserInfo: true);
    }

    #endregion GetInfoByMbidAsync

    #region GetSimilarByNameAsync

    [Test]
    public async Task GetSimilarByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Artist.GetSimilarByNameAsync("My Bloody Valentine");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Has.Count.EqualTo(100));
      foreach (var artist in response.Data.Take(10))
      {
        Assert.That(artist.Match, Is.Not.EqualTo(0.0d));
        Assert.That(artist.Images, Is.Not.Empty);
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
          Assert.That(artist.Match, Is.Not.EqualTo(0.0d));
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
  }
}