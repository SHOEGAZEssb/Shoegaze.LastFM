namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  public class GeoApiIntegrationTests
  {
    #region GetTopArtistsAsync

    [Test]
    public async Task GetTopArtistsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Geo.GetTopArtistsAsync("germany");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      foreach (var artist in response.Data.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(artist.PlayCount, Is.Null);
          Assert.That(artist.ListenerCount, Is.GreaterThan(1));
          Assert.That(artist.UserPlayCount, Is.Null);
          Assert.That(artist.IsStreamable, Is.Not.Null);
        }
      }
    }

    [Test]
    public async Task GetTopArtistsAsync_Invalid_Country_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Geo.GetTopArtistsAsync("SHOEGAZELASTFMINVALIDCOUNTRY");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.LastFmStatus, Is.EqualTo(LastFmStatusCode.InvalidParameters));
      }
    }

    #endregion GetTopArtistsAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Geo.GetTopTracksAsync("germany");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      }

      foreach (var track in response.Data.Items.Take(10))
      {
        using (Assert.EnterMultipleScope())
        {
          Assert.That(track.PlayCount, Is.Null);
          Assert.That(track.ListenerCount, Is.GreaterThan(1));
          Assert.That(track.UserPlayCount, Is.Null);
          Assert.That(track.IsStreamable, Is.Not.Null);
          Assert.That(track.Duration, Is.Not.Null);
          Assert.That(track.Artist, Is.Not.Null);
          Assert.That(track.Album, Is.Null);
        }
      }
    }

    [Test]
    public async Task GetTopTracksAsync_Invalid_Country_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Geo.GetTopTracksAsync("SHOEGAZELASTFMINVALIDCOUNTRY");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.LastFmStatus, Is.EqualTo(LastFmStatusCode.InvalidParameters));
      }
    }

    #endregion GetTopTracksAsync
  }
}
