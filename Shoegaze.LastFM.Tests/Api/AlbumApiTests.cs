using Shoegaze.LastFM.Album;
using Shoegaze.LastFM.Artist;
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

    #region GetTagsByNameAsync

    [Test]
    public async Task GetTagsByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("album.getTags", json);

      var api = new AlbumApi(mock.Object);
      var response = await api.GetTagsByNameAsync("some album", "some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTagsByNameAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("album.getTags");

      var api = new AlbumApi(mock.Object);
      var response = await api.GetTagsByNameAsync("some album", "some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTagsByNameAsync

    #region GetTagsByMbidAsync

    [Test]
    public async Task GetTagsByMbidAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("album.getTags", json);

      var api = new AlbumApi(mock.Object);
      var response = await api.GetTagsByMbidAsync("some-mbid");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task GetTagsByMbidAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("album.getTags");

      var api = new AlbumApi(mock.Object);
      var response = await api.GetTagsByMbidAsync("some-mbid");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTagsByMbidAsync

    #region GetTopTagsByNameAsync

    [Test]
    public async Task GetTopTagsByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("album.getTopTags", json);

      var api = new AlbumApi(mock.Object);
      var response = await api.GetTopTagsByNameAsync("some album", "some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    [Test]
    public async Task GetTopTagsByNameAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("album.getTopTags");

      var api = new AlbumApi(mock.Object);
      var response = await api.GetTopTagsByNameAsync("some album", "some artist");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion GetTopTagsByNameAsync

    #region SearchAsync

    [Test]
    public async Task SearchAsync_ReturnsError_WhenMalformed()
    {
      string json = "{}";
      var mock = TestHelper.CreateMockInvoker("album.search", json);

      var api = new AlbumApi(mock.Object);
      var response = await api.SearchAsync("some album");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }


    [Test]
    public async Task SearchAsync_ReturnsError_WhenError()
    {
      var mock = TestHelper.CreateMockInvoker("album.search");

      var api = new AlbumApi(mock.Object);
      var response = await api.SearchAsync("some album");
      using (Assert.EnterMultipleScope())
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      }
    }

    #endregion SearchAsync
  }
}