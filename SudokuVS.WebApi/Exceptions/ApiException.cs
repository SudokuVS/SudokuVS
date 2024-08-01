namespace SudokuVS.WebApi.Exceptions;

public class ApiException : Exception
{
    public ApiException(int statusCode)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
    public string? Title { get; init; }
    public string? Details { get; init; }
}