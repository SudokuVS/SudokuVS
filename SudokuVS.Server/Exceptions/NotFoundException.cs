namespace SudokuVS.Server.Exceptions;

class NotFoundException : ApiException
{
    public NotFoundException(string? message) : base(StatusCodes.Status404NotFound, message) { }
    public NotFoundException(Exception? innerException = null) : base(StatusCodes.Status404NotFound, innerException) { }
}
