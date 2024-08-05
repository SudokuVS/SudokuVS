namespace SudokuVS.Server.Exceptions;

class ApiException : Exception
{
    public ApiException(int statusCode) : this(statusCode, (string?)null) { }
    public ApiException(int statusCode, Exception? innerException) : this(statusCode, innerException?.ToString()) { }
    public ApiException(int statusCode, string? detail) : this(statusCode, null, detail) { }

    public ApiException(int statusCode, string? title, string? detail)
    {
        StatusCode = statusCode;
        Title = title;
        Detail = detail;
    }

    public int StatusCode { get; }
    public string? Title { get; init; }
    public string? Detail { get; init; }
}
