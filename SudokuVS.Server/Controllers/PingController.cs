using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SudokuVS.Server.Controllers;

/// <summary>
///     Ping
/// </summary>
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
