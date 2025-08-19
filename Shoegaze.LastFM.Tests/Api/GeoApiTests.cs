using Shoegaze.LastFM.Geo;

namespace Shoegaze.LastFM.Tests.Api
{
  [TestFixture]
  internal class GeoApiTests
  {
    #region GetTopArtistsAsync

    [Test]
    public async Task GetTopArtistsAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("geo.getTopArtists", json);

      var api = new GeoApi(mock.Object);
      var response = await api.GetTopArtistsAsync("some country");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopArtistsAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("geo.getTopArtists");

      var api = new GeoApi(mock.Object);
      var response = await api.GetTopArtistsAsync("some country");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopArtistsAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("geo.getTopTracks", json);

      var api = new GeoApi(mock.Object);
      var response = await api.GetTopTracksAsync("some country");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopTracksAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("geo.getTopTracks");

      var api = new GeoApi(mock.Object);
      var response = await api.GetTopTracksAsync("some country");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopTracksAsync
  }
}
