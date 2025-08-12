using Shoegaze.LastFM.Album;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tests.Api
{
  internal class AlbumApiTests
  {
    #region GetInfoByNameAsync

    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("album.getInfo", json);

      var api = new AlbumApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some album", "some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("album.getInfo");

      var api = new AlbumApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some album", "some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetInfoByNameAsync
  }
}
