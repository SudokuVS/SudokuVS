namespace SudokuVS.Server.Exceptions;

class BadRequestException : ApiException
{
    public BadRequestException(Exception? innerException = null) : base(StatusCodes.Status400BadRequest, innerException) { }
}
