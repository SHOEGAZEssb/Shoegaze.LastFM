﻿using Shoegaze.LastFM.Track;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class TrackApiIntegrationTests
  {
    #region GetInfoByNameAsync

    private static void AssertGetInfoTrackInfo(TrackInfo track, bool usernameIncluded)
    {
      Assert.Multiple(() =>
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
        Assert.That(track.Rank, Is.Null);
        Assert.That(track.PlayedAt, Is.Null);
        Assert.That(track.Images, Is.Empty);
      });

      var artist = track.Artist;
      Assert.That(artist, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(artist.Name, Is.EqualTo("Korn"));
        Assert.That(artist.Mbid, Is.Not.Empty);
        Assert.That(artist.Url.ToString(), Is.Not.Empty);
        Assert.That(artist.Images, Is.Empty);
      });

      var album = track.Album;
      Assert.That(album, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(album.Name, Is.EqualTo("Korn"));
        Assert.That(album.Mbid, Is.Not.Empty);
        Assert.That(album.Url!.ToString(), Is.Not.Empty);
        Assert.That(album.Images, Is.Not.Empty);
      });

      var tags = track.TopTags;
      Assert.That(tags, Is.Not.Null);
      foreach (var tag in tags)
      {
        Assert.Multiple(() =>
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        });
      }

      var wiki = track.Wiki;
      Assert.That(wiki, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(wiki.Published, Is.Not.EqualTo(default(DateTime)));
        Assert.That(wiki.Content, Is.Not.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
      });
    }

    [Test]
    public async Task GetInfoByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByNameAsync("Blind", "Korn");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoTrackInfo(response.Data, false);
    }

    [Test]
    public async Task GetInfoByNameAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByNameAsync("Blind", "Korn", "coczero");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoTrackInfo(response.Data, true);
    }

    [Test]
    public async Task GetInfoByNameAsync_Missing_Artist_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByNameAsync("Blind", "");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.Status, Is.EqualTo(LastFmStatusCode.InvalidParameters));
        Assert.That(response.ErrorMessage, Is.Not.Empty);
      });
    }

    #endregion GetInfoByNameAsync

    #region GetInfoByMbidAsync

    [Test]
    public async Task GetInfoByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByMbidAsync("7bc25578-9469-4f46-97ce-1871d9ce1c69");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoTrackInfo(response.Data, false);
    }

    [Test]
    public async Task GetInfoByMbidAsync_With_Username_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetInfoByMbidAsync("7bc25578-9469-4f46-97ce-1871d9ce1c69", "coczero");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      AssertGetInfoTrackInfo(response.Data, true);
    }

    #endregion GetInfoByMbidAsync

    #region GetCorrectionAsync

    [Test]
    public async Task GetCorrectionAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetCorrectionAsync("Mrbrownstone", "guns and roses");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var track = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(track.Name, Is.EqualTo("Mr. Brownstone"));
        Assert.That(track.Mbid, Is.Not.Empty);
        Assert.That(track.Url.ToString(), Is.Not.Empty);
      });

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
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var track in response.Data)
      {
        Assert.Multiple(() =>
        {
          Assert.That(track.PlayCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.Match, Is.Not.EqualTo(0.0d));
        });
      }
    }

    #endregion GetSimilarByNameAsync

    #region GetSimilarByMbidAsync

    [Test]
    public async Task GetSimilarByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetSimilarByMbidAsync("7bc25578-9469-4f46-97ce-1871d9ce1c69");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var track in response.Data)
      {
        Assert.Multiple(() =>
        {
          Assert.That(track.PlayCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.Match, Is.Not.EqualTo(0.0d));
        });
      }
    }

    #endregion GetSimilarByMbidAsync

    #region GetUserTagsByNameAsync

    [Test]
    public async Task GetUserTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetUserTagsByName("soon", "my bloody valentine", "coczero");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var tag in response.Data)
      {
        Assert.Multiple(() =>
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        });
      }
    }

    #endregion GetUserTagsByNameAsync

    #region GetUserTagsByMbidAsync

    [Test]
    public async Task GetUserTagsByMbidAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetUserTagsByMbid("55f39cd9-9326-3ca0-bd7c-ed21f61b30b5", "coczero");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var tag in response.Data)
      {
        Assert.Multiple(() =>
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
        });
      }
    }

    #endregion GetUserTagsByMbidAsync

    #region GetTopTagsByNameAsync

    [Test]
    public async Task GetTopTagsByNameAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.GetTopTagsByName("soon", "my bloody valentine");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      Assert.That(response.Data, Has.Count.GreaterThan(1));
      foreach (var tag in response.Data)
      {
        Assert.Multiple(() =>
        {
          Assert.That(tag.Name, Is.Not.Empty);
          Assert.That(tag.Url.ToString(), Is.Not.Empty);
          Assert.That(tag.Count, Is.GreaterThanOrEqualTo(1));
          Assert.That(tag.UserUsedCount, Is.Null);
        });
      }
    }

    #endregion GetTopTagsByNameAsync

    #region SearchAsync

    [Test]
    public async Task SearchAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.SearchAsync("Blind");
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

      Assert.That(pages.Items, Has.Count.GreaterThan(1));
      foreach (var track in pages.Items)
      {
        Assert.Multiple(() =>
        {
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.Images, Is.Not.Empty);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.ListenerCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.UserPlayCount, Is.Null);
        });
      }
    }

    [Test]
    public async Task SearchAsync_With_Artist_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.SearchAsync("Blind", "Korn");
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

      Assert.That(pages.Items, Has.Count.GreaterThan(1));
      foreach (var track in pages.Items)
      {
        Assert.Multiple(() =>
        {
          Assert.That(track.Mbid, Is.Not.Null);
          Assert.That(track.Images, Is.Not.Empty);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.ListenerCount, Is.GreaterThanOrEqualTo(1));
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.UserPlayCount, Is.Null);
        });
      }
    }

    [Test]
    public async Task SearchAsync_Invalid_Track_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Track.SearchAsync("SHOEGAZELASTFMINVALIDTRACK");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var pages = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(pages.Page, Is.EqualTo(1));
        Assert.That(pages.TotalPages, Is.EqualTo(0));
        Assert.That(pages.TotalItems, Is.EqualTo(0));
      });

      Assert.That(pages.Items, Is.Empty);
    }

    #endregion SearchAsync
  }
}