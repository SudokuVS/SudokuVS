namespace SudokuVS.Server.Exceptions;

class BadRequestException : ApiException
{
    public BadRequestException(string? message = null) : base(StatusCodes.Status400BadRequest, message) { }
    public BadRequestException(Exception? innerException = null) : base(StatusCodes.Status400BadRequest, innerException) { }
}
