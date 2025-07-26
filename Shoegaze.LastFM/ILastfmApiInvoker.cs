using System.Text.Json;

namespace Shoegaze.LastFM;

/// <summary>
/// Internal interface for sending signed requests to the Last.fm API.
/// </summary>
internal interface ILastfmRequestInvoker
{
  /// <summary>
  /// Sends a request to the Last.fm API.
  /// </summary>
  Task<ApiResult<JsonDocument>> SendAsync(string method, IDictionary<string, string> parameters, bool requireAuth = false, CancellationToken ct = default);

  /// <summary>
  /// Optional session key for authenticated calls.
  /// </summary>
  string? SessionKey { get; set; }
}