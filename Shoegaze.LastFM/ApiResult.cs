using System.Net;

namespace Shoegaze.LastFM;

/// <summary>
/// Possible last.fm api status codes.
/// </summary>
public enum LastFmStatusCode
{
  /// <summary>
  /// An unknown error occured.
  /// </summary>
  UnknownError = -1,

  /// <summary>
  /// The request was successful.
  /// </summary>
  Success = 0,

  /// <summary>
  /// This service does not exist.
  /// </summary>
  InvalidService = 2,

  /// <summary>
  /// No method with that name in this package.
  /// </summary>
  InvalidMethod = 3,

  /// <summary>
  /// You do not have permissions to access the service.
  /// </summary>
  AuthenticationFailed = 4,

  /// <summary>
  /// This service doesn't exist in that format.
  /// </summary>
  InvalidFormat = 5,

  /// <summary>
  /// Your request is missing a required parameter.
  /// </summary>
  InvalidParameters = 6,

  /// <summary>
  /// Invalid resource specified.
  /// </summary>
  InvalidResource = 7,

  /// <summary>
  /// Something else went wrong.
  /// </summary>
  OperationFailed = 8,

  /// <summary>
  /// Invalid session key - please re-authenticate.
  /// </summary>
  InvalidSessionKey = 9,

  /// <summary>
  /// You must be granted a valid key by Last.fm.
  /// </summary>
  InvalidApiKey = 10,

  /// <summary>
  /// This service is temporarily offline. Try again later.
  /// </summary>
  ServiceOffline = 11,

  /// <summary>
  /// Invalid method signature supplied.
  /// </summary>
  InvalidMethodSignature = 13,

  /// <summary>
  /// There was a temporary error processing your request. Please try again.
  /// </summary>
  TemporaryError = 16,

  /// <summary>
  /// Access for your api account has been suspended, please contact Last.fm
  /// </summary>
  SuspendedApiKey = 26,

  /// <summary>
  /// Your IP has made too many requests in a short period.
  /// </summary>
  RateLimitExceeded = 29
}

/// <summary>
/// Represents Last.fm image sizes returned by the API.
/// </summary>
public enum ImageSize
{
  /// <summary>
  /// Small image size.
  /// </summary>
  Small,

  /// <summary>
  /// Medium image size.
  /// </summary>
  Medium,

  /// <summary>
  /// Large image size.
  /// </summary>
  Large,

  /// <summary>
  /// Extra large image size.
  /// </summary>
  ExtraLarge,

  /// <summary>
  /// Mega image size.
  /// </summary>
  Mega,

  /// <summary>
  /// Image size is unknown.
  /// </summary>
  Unknown
}

/// <summary>
/// The result of an api call.
/// </summary>
public class ApiResult(LastFmStatusCode? lastFmStatus, HttpStatusCode? httpStatus, string? errorMessage)
{
  /// <summary>
  /// Http status code.
  /// May be null in case an error was encountered
  /// before any http request was sent.
  /// </summary>
  public HttpStatusCode? HttpStatus { get; } = httpStatus;

  /// <summary>
  /// Error message describing the cause of the error.
  /// May be null in case of an unknown error.
  /// </summary>
  public string? ErrorMessage { get; } = errorMessage;

  /// <summary>
  /// The last.fm api status code.
  /// May be null in case the error was not reported by
  /// the last.fm api.
  /// </summary>
  public LastFmStatusCode? LastFmStatus { get; } = lastFmStatus;

  /// <summary>
  /// If the api request returned successfully.
  /// </summary>
  public bool IsSuccess => LastFmStatus == LastFmStatusCode.Success;

  internal static ApiResult Success(HttpStatusCode httpStatus = HttpStatusCode.OK)
    => new(lastFmStatus: LastFmStatusCode.Success, httpStatus: httpStatus, errorMessage: null);

  internal static ApiResult Failure(LastFmStatusCode? status = null, HttpStatusCode? httpStatus = null, string? error = null)
    => new(status, httpStatus, error);
}

/// <summary>
/// The result of an api call that returned data.
/// </summary>
public sealed class ApiResult<T>(T? data = default, LastFmStatusCode? lastFmStatus = null, HttpStatusCode? httpStatus = null, string? errorMessage = null)
  : ApiResult(lastFmStatus, httpStatus, errorMessage)
{
  /// <summary>
  /// The returned data from the api call.
  /// </summary>
  public T? Data { get; } = data;

  internal static ApiResult<T> Success(T data, HttpStatusCode httpStatus = HttpStatusCode.OK)
    => new(data, LastFmStatusCode.Success, httpStatus);

  internal new static ApiResult<T> Failure(LastFmStatusCode? status = null, HttpStatusCode? httpStatus = null, string? error = null)
      => new(default, status, httpStatus, error);
}