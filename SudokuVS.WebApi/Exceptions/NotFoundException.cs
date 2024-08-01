namespace SudokuVS.WebApi.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(Exception? innerException = null) : base(StatusCodes.Status404NotFound, innerException)
    {
    }
}
