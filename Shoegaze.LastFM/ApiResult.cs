namespace Shoegaze.LastFM;

/// <summary>
/// Represents logical categories of API outcomes.
/// </summary>
public enum ApiStatusCode
{
  Success = 0,
  HttpError = 1,
  InvalidSignature = 2,
  AuthenticationRequired = 3,
  RateLimited = 4,
  NotFound = 5,
  InvalidRequest = 6,
  NetworkError = 7,
  ServerError = 8,
  UnknownError = 9
}

/// <summary>
/// Wraps the result of an API call, including the data and request metadata.
/// </summary>
public class ApiResult<T>
{
  public T? Data { get; init; }
  public int HttpStatusCode { get; init; }
  public string? ErrorMessage { get; init; }
  public ApiStatusCode Status { get; init; }

  public bool IsSuccess => Status == ApiStatusCode.Success;

  internal static ApiResult<T> Success(T data, int httpStatus = 200)
          => new() { Data = data, HttpStatusCode = httpStatus, Status = ApiStatusCode.Success };

  internal static ApiResult<T> Failure(ApiStatusCode status, int httpStatus = 0, string? error = null)
      => new() { Status = status, HttpStatusCode = httpStatus, ErrorMessage = error };
}
