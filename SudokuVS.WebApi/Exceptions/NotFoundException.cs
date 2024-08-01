using Microsoft.AspNetCore.Http;

namespace SudokuVS.WebApi.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException() : base(StatusCodes.Status404NotFound)
    {
    }
}
