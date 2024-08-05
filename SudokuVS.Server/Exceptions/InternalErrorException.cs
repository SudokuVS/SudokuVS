namespace SudokuVS.Server.Exceptions;

class InternalErrorException : ApiException
{
    public InternalErrorException(string? detail = null) : base(StatusCodes.Status500InternalServerError, detail) { }
    public InternalErrorException(Exception? innerException = null) : base(StatusCodes.Status500InternalServerError, innerException) { }
}
