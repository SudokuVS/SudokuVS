using Microsoft.AspNetCore.Http;

namespace SudokuVS.WebApi.Exceptions;

public class AccessDeniedException : ApiException
{
    public AccessDeniedException() : base(StatusCodes.Status403Forbidden)
    {
    }
}
