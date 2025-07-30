namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class UserApiIntegrationTests
  {
    [Test]
    public async Task GetInfoAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var info = await client.User.GetInfoAsync("coczero");

      Assert.Multiple(() =>
      {
        Assert.That(info.IsSuccess, Is.True);
        Assert.That(info.Data, Is.Not.Null);
      });

      var user = info.Data;
      Assert.Multiple(() =>
      {
        Assert.That(user.Username, Is.EqualTo("coczero"));
        Assert.That(user.TrackCount, Is.GreaterThan(1));
        Assert.That(user.ArtistCount, Is.GreaterThan(1));
        Assert.That(user.AlbumCount, Is.GreaterThan(1));
        Assert.That(user.Country, Is.EqualTo("Germany"));
        Assert.That(user.Playcount, Is.GreaterThan(1));
        Assert.That(user.RealName, Is.EqualTo("Tim Stadler"));
        Assert.That(user.Images, Contains.Key(ImageSize.Small));
        Assert.That(user.Images, Contains.Key(ImageSize.Medium));
        Assert.That(user.Images, Contains.Key(ImageSize.Large));
        Assert.That(user.Images, Contains.Key(ImageSize.ExtraLarge));
        Assert.That(user.RegisteredDate, Is.EqualTo(DateTimeOffset.FromUnixTimeSeconds(1285787447).DateTime));
      });
    }
  }
}