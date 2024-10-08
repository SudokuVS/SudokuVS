﻿namespace SudokuVS.Server.Exceptions;

class AccessDeniedException : ApiException
{
    public AccessDeniedException(Exception? innerException = null) : base(StatusCodes.Status403Forbidden, innerException)
    {
    }
}
