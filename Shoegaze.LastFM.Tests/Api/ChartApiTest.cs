using Shoegaze.LastFM.Chart;

namespace Shoegaze.LastFM.Tests.Api
{
  [TestFixture]
  internal class ChartApiTest
  {
    #region GetTopArtistsAsync

    [Test]
    public async Task GetTopArtistsAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("chart.getTopArtists", json);

      var api = new ChartApi(mock.Object);
      var response = await api.GetTopArtistsAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopArtistsAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("chart.getTopArtists");

      var api = new ChartApi(mock.Object);
      var response = await api.GetTopArtistsAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopArtistsAsync

    #region GetTopTagsAsync

    [Test]
    public async Task GetTopTagsAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("chart.getTopTags", json);

      var api = new ChartApi(mock.Object);
      var response = await api.GetTopTagsAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopTagsAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("chart.getTopTags");

      var api = new ChartApi(mock.Object);
      var response = await api.GetTopTagsAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopTagsAsync

    #region GetTopTracksAsync

    [Test]
    public async Task GetTopTracksAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("chart.getTopTracks", json);

      var api = new ChartApi(mock.Object);
      var response = await api.GetTopTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTopTracksAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("chart.getTopTracks");

      var api = new ChartApi(mock.Object);
      var response = await api.GetTopTracksAsync();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopTracksAsync
  }
}