using Moq;
using Shoegaze.LastFM.User;
using System.Text.Json;

namespace Shoegaze.LastFM.Tests.User
{
  [TestFixture]
  public class UserApiTests
  {
    [Test]
    public async Task GetInfoAsync_ReturnsUserInfo_WhenSuccessful()
    {
      // Arrange
      var json = """
        {
          "user": {
            "id": "1000002",
            "name": "RJ",
            "realname": "Richard Jones",
            "url": "http://www.last.fm/user/RJ",
            "image": "http://userserve-ak.last.fm/serve/126/8270359.jpg",
            "country": "UK",
            "age": "27",
            "gender": "m",
            "subscriber": "1",
            "playcount": "54189",
            "playlists": "4",
            "bootstrap": "0",
            "registered": {
              "#text": "2002-11-20 11:50",
              "unixtime": "1037793040"
            }
          }
        }
        """;

      using var doc = JsonDocument.Parse(json);

      var invokerMock = new Mock<ILastfmRequestInvoker>();
      invokerMock
          .Setup(i => i.SendAsync(
              "user.getInfo",
              It.IsAny<IDictionary<string, string>>(),
              true,
              It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc, 200));

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetInfoAsync();

      Assert.Multiple(() =>
      {
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
      });

      var user = result.Data!;
      Assert.Multiple(() =>
      {
        Assert.That(user.Id, Is.EqualTo("1000002"));
        Assert.That(user.Username, Is.EqualTo("RJ"));
        Assert.That(user.RealName, Is.EqualTo("Richard Jones"));
        Assert.That(user.Url, Is.EqualTo("http://www.last.fm/user/RJ"));
        Assert.That(user.ImageUrl, Is.EqualTo("http://userserve-ak.last.fm/serve/126/8270359.jpg"));
        Assert.That(user.Country, Is.EqualTo("UK"));
        Assert.That(user.Age, Is.EqualTo(27));
        Assert.That(user.Gender, Is.EqualTo("m"));
        Assert.That(user.IsSubscriber, Is.True);
        Assert.That(user.Playcount, Is.EqualTo(54189));
        Assert.That(user.Playlists, Is.EqualTo(4));
        Assert.That(user.RegisteredDate, Is.EqualTo(DateTimeOffset.FromUnixTimeSeconds(1037793040).DateTime));
      });
    }

    [Test]
    public async Task GetInfoAsync_ReturnsFailure_WhenApiCallFails()
    {
      // Arrange
      var invokerMock = new Mock<ILastfmRequestInvoker>();
      invokerMock
          .Setup(i => i.SendAsync(
              "user.getInfo",
              It.IsAny<IDictionary<string, string>>(),
              true,
              It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure(
              ApiStatusCode.AuthenticationRequired,
              httpStatus: 401,
              error: "Session key is missing or invalid."
          ));

      var api = new UserApi(invokerMock.Object);

      // Act
      var result = await api.GetInfoAsync();

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result.IsSuccess, Is.False, "Call should not succeed");
        Assert.That(result.Data, Is.Null, "Data should be null");
        Assert.That(result.Status, Is.EqualTo(ApiStatusCode.AuthenticationRequired));
        Assert.That(result.HttpStatusCode, Is.EqualTo(401));
        Assert.That(result.ErrorMessage, Is.EqualTo("Session key is missing or invalid."));
      });
    }
  }
}