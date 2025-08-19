using Shoegaze.LastFM.Library;

namespace Shoegaze.LastFM.Tests.Api
{
  [TestFixture]
  internal class LibraryApiTests
  {
    #region GetArtistsAsync

    [Test]
    public async Task GetArtistsAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("library.getArtists", json);

      var api = new LibraryApi(mock.Object);
      var response = await api.GetArtistsAsync("some user");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetArtistsAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("library.getArtists");

      var api = new LibraryApi(mock.Object);
      var response = await api.GetArtistsAsync("some user");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetArtistsAsync
  }
}
