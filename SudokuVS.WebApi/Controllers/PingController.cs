using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SudokuVS.WebApi.Controllers;

[Route("/api/ping")]
[AllowAnonymous]
[ApiController]
public class PingController : ControllerBase
{
    /// <summary>
    ///     Ping
    /// </summary>
    [HttpGet]
    public string Ping() => "pong";
}
