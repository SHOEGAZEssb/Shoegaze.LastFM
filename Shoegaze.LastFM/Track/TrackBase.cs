using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Track
{
  public abstract class TrackBase
  {
    public string Name { get; init; } = "";
    public string Url { get; init; } = "";
    public string ArtistName { get; init; } = "";
    public string ArtistUrl { get; init; } = "";
    public Dictionary<ImageSize, string> Images { get; init; } = [];

    public override string ToString() => $"{ArtistName} – {Name}";
  }

}
