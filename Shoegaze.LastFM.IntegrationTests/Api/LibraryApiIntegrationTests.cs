namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class LibraryApiIntegrationTests
  {
    [Test]
    public async Task GetArtistsAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Library.GetArtistsAsync("coczero");
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
          Assert.That(artist.ListenerCount, Is.Null);
          Assert.That(artist.UserPlayCount, Is.GreaterThan(1));
          Assert.That(artist.IsStreamable, Is.Not.Null);
        }
      }
    }
  }
}
