using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Shoegaze.LastFM.Authentication;

internal static class OAuth1Signer
{
  public static string GenerateSignature(string method, Uri uri, IDictionary<string, string> parameters, string consumerSecret, string tokenSecret = "")
  {
    var sortedParams = parameters
        .OrderBy(p => p.Key)
        .ThenBy(p => p.Value)
        .Select(p => $"{UrlEncode(p.Key)}={UrlEncode(p.Value)}");

    var paramString = string.Join("&", sortedParams);
    var baseString = $"{method.ToUpper()}&{UrlEncode(uri.ToString())}&{UrlEncode(paramString)}";

    var signingKey = $"{UrlEncode(consumerSecret)}&{UrlEncode(tokenSecret)}";
    using var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
    var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString));

    return Convert.ToBase64String(hash);
  }

  private static string UrlEncode(string value) =>
      HttpUtility.UrlEncode(value)
          .Replace("+", "%20")
          .Replace("*", "%2A")
          .Replace("%7E", "~");
}
