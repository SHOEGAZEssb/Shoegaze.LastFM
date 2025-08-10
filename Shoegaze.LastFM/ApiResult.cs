using System.Net;

namespace Shoegaze.LastFM;

/// <summary>
/// Possible last.fm api status codes.
/// </summary>
public enum LastFmStatusCode
{
  UnknownError = -1,
  Success = 0,
  InvalidService = 2,
  AuthenticationFailed = 4,
  InvalidFormat = 5,
  InvalidParameters = 6,
  InvalidResource = 7,
  OperationFailed = 8,
  InvalidSessionKey = 9,
  InvalidApiKey = 10,
  ServiceOffline = 11,
  InvalidMethodSignature = 13,
  TemporaryError = 16,
  SuspendedApiKey = 26,
  RateLimitExceeded = 29
}

/// <summary>
/// Represents Last.fm image sizes returned by the API.
/// </summary>
public enum ImageSize
{
  Small,
  Medium,
  Large,
  ExtraLarge,
  Mega,
  Unknown
}

/// <summary>
/// The result of an api call.
/// </summary>
public class ApiResult
{
  /// <summary>
  /// Http status code.
  /// May be null in case an error was encountered
  /// before any http request was sent.
  /// </summary>
  public HttpStatusCode? HttpStatus { get; internal set; }

  /// <summary>
  /// Error message describing the cause of the error.
  /// May be null in case of an unknown error.
  /// </summary>
  public string? ErrorMessage { get; internal set; }

  /// <summary>
  /// The last.fm api status code.
  /// May be null in case the error was not reported by
  /// the last.fm api.
  /// </summary>
  public LastFmStatusCode? Status { get; internal set; }

  /// <summary>
  /// If the api request returned successfully.
  /// </summary>
  public bool IsSuccess => Status == LastFmStatusCode.Success;

  internal static ApiResult Success(HttpStatusCode httpStatus = HttpStatusCode.OK)
          => new() { HttpStatus = httpStatus, Status = LastFmStatusCode.Success };

  internal static ApiResult Failure(LastFmStatusCode? status = null, HttpStatusCode? httpStatus = null, string? error = null)
      => new() { Status = status, HttpStatus = httpStatus, ErrorMessage = error };
}

/// <summary>
/// The result of an api call that returned data.
/// </summary>
public sealed class ApiResult<T> : ApiResult
{
  /// <summary>
  /// The returned data from the api call.
  /// </summary>
  public T? Data { get; internal set; }

  internal static ApiResult<T> Success(T data, HttpStatusCode httpStatus = HttpStatusCode.OK)
          => new() { Data = data, HttpStatus = httpStatus, Status = LastFmStatusCode.Success };

  internal new static ApiResult<T> Failure(LastFmStatusCode? status = null, HttpStatusCode? httpStatus = null, string? error = null)
      => new() { Status = status, HttpStatus = httpStatus, ErrorMessage = error };
}