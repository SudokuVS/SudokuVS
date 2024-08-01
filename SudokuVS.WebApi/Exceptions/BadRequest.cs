namespace SudokuVS.WebApi.Exceptions;

public class BadRequest : ApiException
{
    public BadRequest(Exception? innerException = null) : base(StatusCodes.Status400BadRequest, innerException)
    {
    }
}
