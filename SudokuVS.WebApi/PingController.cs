using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SudokuVS.WebApi;

[Route("/api/ping")]
[AllowAnonymous]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public string Ping() => "pong";
}
