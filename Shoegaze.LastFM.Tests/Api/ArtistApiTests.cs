using Moq;
using Shoegaze.LastFM.Artist;
using Shoegaze.LastFM.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tests.Api
{
  internal class ArtistApiTests
  {
    #region GetInfoByNameAsync

    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenMalformed()
    {
      string json = """
      {
        "artist": {
          "nameMalformed": "Metallica",
          "mbid": "65f4f0c5-ef9e-490c-aee3-909e7ae6b2ab",
          "url": "https://www.last.fm/music/Metallica",
          "image": [
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/34s/b1db70984273ae75da5a4598ad906a2a.jpg",
              "size": "small"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/64s/b1db70984273ae75da5a4598ad906a2a.jpg",
              "size": "medium"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/174s/b1db70984273ae75da5a4598ad906a2a.jpg",
              "size": "large"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/b1db70984273ae75da5a4598ad906a2a.jpg",
              "size": "extralarge"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/b1db70984273ae75da5a4598ad906a2a.jpg",
              "size": "mega"
            },
            {
              "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/b1db70984273ae75da5a4598ad906a2a.jpg",
              "size": ""
            }
          ],
          "streamable": "0",
          "ontour": "1",
          "stats": {
            "listeners": "4877722",
            "playcount": "500694289",
            "userplaycount": "93"
          },
          "similar": {
            "artist": [
              {
                "name": "Megadeth",
                "url": "https://www.last.fm/music/Megadeth",
                "image": [
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "small"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "medium"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "large"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "extralarge"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "mega"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": ""
                  }
                ]
              },
              {
                "name": "Pantera",
                "url": "https://www.last.fm/music/Pantera",
                "image": [
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "small"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "medium"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "large"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "extralarge"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "mega"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": ""
                  }
                ]
              },
              {
                "name": "Slayer",
                "url": "https://www.last.fm/music/Slayer",
                "image": [
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "small"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "medium"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "large"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "extralarge"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "mega"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": ""
                  }
                ]
              },
              {
                "name": "Anthrax",
                "url": "https://www.last.fm/music/Anthrax",
                "image": [
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "small"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "medium"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "large"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "extralarge"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "mega"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": ""
                  }
                ]
              },
              {
                "name": "Iron Maiden",
                "url": "https://www.last.fm/music/Iron+Maiden",
                "image": [
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/34s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "small"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/64s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "medium"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/174s/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "large"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "extralarge"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": "mega"
                  },
                  {
                    "#text": "https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png",
                    "size": ""
                  }
                ]
              }
            ]
          },
          "tags": {
            "tag": [
              {
                "name": "thrash metal",
                "url": "https://www.last.fm/tag/thrash+metal"
              },
              {
                "name": "heavy metal",
                "url": "https://www.last.fm/tag/heavy+metal"
              },
              {
                "name": "metal",
                "url": "https://www.last.fm/tag/metal"
              },
              {
                "name": "hard rock",
                "url": "https://www.last.fm/tag/hard+rock"
              },
              {
                "name": "rock",
                "url": "https://www.last.fm/tag/rock"
              }
            ]
          },
          "bio": {
            "links": {
              "link": {
                "#text": "",
                "rel": "original",
                "href": "https://last.fm/music/Metallica/+wiki"
              }
            },
            "published": "01 Feb 2006, 18:28",
            "summary": "Metallica is an American heavy metal band. The band was formed in 1981 in Los Angeles by vocalist/guitarist James Hetfield and drummer Lars Ulrich, and has been based in San Francisco for most of its career. The band's fast tempos, instrumentals and aggressive musicianship made them one of the founding \"big four\" bands of thrash metal, alongside Megadeth, Anthrax and Slayer. Metallica's current lineup comprises founding members and primary songwriters Hetfield and Ulrich, longtime lead guitarist Kirk Hammett and bassist Robert Trujillo. <a href=\"https://www.last.fm/music/Metallica\">Read more on Last.fm</a>",
            "content": "Metallica is an American heavy metal band. The band was formed in 1981 in Los Angeles by vocalist/guitarist James Hetfield and drummer Lars Ulrich, and has been based in San Francisco for most of its career. The band's fast tempos, instrumentals and aggressive musicianship made them one of the founding \"big four\" bands of thrash metal, alongside Megadeth, Anthrax and Slayer. Metallica's current lineup comprises founding members and primary songwriters Hetfield and Ulrich, longtime lead guitarist Kirk Hammett and bassist Robert Trujillo. Guitarist Dave Mustaine, who formed Megadeth after being fired from the band, and bassists Ron McGovney, Cliff Burton and Jason Newsted are former members of the band.\n\nMetallica first found commercial success with the release of its third album, Master of Puppets (1986), which is cited as one of the heaviest metal albums and the band's best work. The band's next album, ...And Justice for All (1988), gave Metallica its first Grammy Award nomination. Its self-titled fifth album, Metallica (1991), was the band's first not to root predominantly in thrash metal; it appealed to a more mainstream audience, achieving substantial commercial success and selling over 16 million copies in the United States to date, making it the best-selling album of the SoundScan era. After experimenting with different genres and directions in subsequent releases, Metallica returned to its thrash metal roots with the release of its ninth album, Death Magnetic (2008), which drew similar praise to that of the band's earlier albums. This was followed by the band's 10th studio album, Hardwired... to Self-Destruct (2016), with its 11th album, 72 Seasons, releasing in 2023.\n\nIn 2000, Metallica led the case against the peer-to-peer file sharing service Napster, in which the band and several other artists filed lawsuits against the service for sharing their copyright-protected material without consent, eventually reaching a settlement. Metallica was the subject of the acclaimed 2004 documentary film Metallica: Some Kind of Monster, which documented the troubled production of the band's eighth album, St. Anger (2003), and the internal struggles within the band at the time. In 2009, Metallica was inducted into the Rock and Roll Hall of Fame. The band co-wrote the screenplay for and starred alongside Dane DeHaan in the 2013 concert film Metallica: Through the Never, in which the band performed live against a fictional thriller storyline.\n\nMetallica has released 10 studio albums, four live albums (including two performances with the San Francisco Symphony), 12 video albums, a cover album, two extended plays, 37 singles and 39 music videos. The band has won nine Grammy Awards from 23 nominations, and its last six studio albums (beginning with Metallica) have consecutively debuted at No. 1 on the Billboard 200. Metallica ranks as one of the most commercially successful bands of all time, having sold over 125 million albums worldwide as of 2018. Metallica has been listed as one of the greatest artists of all time by magazines such as Rolling Stone, which ranked the band No. 61 on its 100 Greatest Artists of All Time list. As of 2017, Metallica is the third-best-selling music artist since Nielsen SoundScan began tracking sales in 1991, selling a total of 58 million albums in the United States.\n\nFull Wikipedia article: https://en.wikipedia.org/wiki/Metallica\n\nStudio albums\nKill 'Em All (1983)\nRide the Lightning (1984)\nMaster of Puppets (1986)\n...And Justice for All (1988)\nMetallica (1991)\nLoad (1996)\nReload (1997)\nSt. Anger (2003)\nDeath Magnetic (2008)\nHardwired... to Self-Destruct (2016)\n72 Seasons (2023) <a href=\"https://www.last.fm/music/Metallica\">Read more on Last.fm</a>. User-contributed text is available under the Creative Commons By-SA License; additional terms may apply."
          }
        }
      }
      """;

      var doc = JsonDocument.Parse(json);
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Success(doc));

      var api = new ArtistApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some artist");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }


    [Test]
    public async Task GetInfoByNameAsync_ReturnsError_WhenError()
    {
      var mock = new Mock<ILastfmRequestInvoker>();
      mock.Setup(m => m.SendAsync("artist.getInfo", It.IsAny<IDictionary<string, string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(ApiResult<JsonDocument>.Failure());

      var api = new ArtistApi(mock.Object);
      var response = await api.GetInfoByNameAsync("some artist");
      Assert.Multiple(() =>
      {
        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Data, Is.Null);
      });
    }

    #endregion GetInfoByNameAsync
  }
}
