using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.IntegrationTests.Api
{
  [TestFixture]
  internal class TagApiIntegrationTests
  {
    [Test]
    public async Task GetInfoAsync_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetInfoAsync("shoegaze");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var tag = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(tag.Reach, Is.GreaterThan(1));
        Assert.That(tag.Taggings, Is.GreaterThan(1));
        Assert.That(tag.UserUsedCount, Is.Null);
        Assert.That(tag.Count, Is.Null);
      });

      var wiki = tag.Wiki;
      Assert.That(wiki, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(wiki.Content, Is.Not.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
        Assert.That(wiki.Published, Is.Not.Null);
        Assert.That(wiki.Published, Is.Not.EqualTo(default(DateTime)));
      });
    }

    [Test]
    public async Task GetInfoAsync_Invalid_Tag_IntegrationTest()
    {
      var client = TestEnvironment.CreateClient();

      var response = await client.Tag.GetInfoAsync("SHOEGAZELASTFMINVALIDTAG");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.Not.Null);
      });

      var tag = response.Data;
      Assert.Multiple(() =>
      {
        Assert.That(tag.Reach, Is.EqualTo(0));
        Assert.That(tag.Taggings, Is.EqualTo(0));
        Assert.That(tag.UserUsedCount, Is.Null);
        Assert.That(tag.Count, Is.Null);
      });

      var wiki = tag.Wiki;
      Assert.That(wiki, Is.Not.Null);
      Assert.Multiple(() =>
      {
        Assert.That(wiki.Content, Is.Empty);
        Assert.That(wiki.Summary, Is.Not.Empty);
        Assert.That(wiki.Published, Is.Null);
      });
    }
  }
}
