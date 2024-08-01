using Microsoft.AspNetCore.Http;

namespace SudokuVS.WebApi.Exceptions;

public class BadRequest : ApiException
{
    public BadRequest() : base(StatusCodes.Status400BadRequest)
    {
    }
}
