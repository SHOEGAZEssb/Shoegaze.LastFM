using System.Net;

namespace Shoegaze.LastFM;

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
/// Wraps the result of an API call, including the data and request metadata.
/// </summary>
public class ApiResult<T>
{
  public T? Data { get; set; }
  public HttpStatusCode? HttpStatus { get; set; }
  public string? ErrorMessage { get; set; }
  public LastFmStatusCode? Status { get; set; }

  public bool IsSuccess => Status == LastFmStatusCode.Success;

  internal static ApiResult<T> Success(T data, HttpStatusCode httpStatus = HttpStatusCode.OK)
          => new() { Data = data, HttpStatus = httpStatus, Status = LastFmStatusCode.Success };

  internal static ApiResult<T> Failure(LastFmStatusCode? status = null, HttpStatusCode? httpStatus = null, string? error = null)
      => new() { Status = status, HttpStatus = httpStatus, ErrorMessage = error };
}