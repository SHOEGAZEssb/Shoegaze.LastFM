using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class ChartApiIntegrationTests
  {
    #region GetTopArtistsAsync

    [Test]
    public async Task GetTopArtistsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Chart.GetTopArtistsAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      foreach (var artist in response.Data.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.PlayCount, Is.GreaterThan(1));
          Assert.That(artist.ListenerCount, Is.GreaterThan(1));
          Assert.That(artist.UserPlayCount, Is.Null);
          Assert.That(artist.IsStreamable, Is.Not.Null);
        }
      }
    }

    #endregion GetTopArtistsAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Chart.GetTopTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      foreach (var track in response.Data.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.PlayCount, Is.GreaterThan(1));
          Assert.That(track.ListenerCount, Is.GreaterThan(1));
          Assert.That(track.UserPlayCount, Is.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.Duration, Is.Not.Null);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.Album, Is.Null);
        }
      }
    }

    #endregion GetTopTracksAsync

    #region GetTopTagsAsync

    [Test]
    public async Task GetTopTagsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Chart.GetTopTagsAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      foreach (var tag in response.Data.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(tag.Reach, Is.GreaterThan(1));
          Assert.That(tag.Taggings, Is.GreaterThan(1));
          Assert.That(tag.CountOnTrack, Is.Null);
          Assert.That(tag.WeightOnAlbum, Is.Null);
          Assert.That(tag.UserUsedCount, Is.Null);
          Assert.That(tag.Wiki, Is.Null);
        }
      }
    }

    #endregion GetTopTagsAsync
  }
}
