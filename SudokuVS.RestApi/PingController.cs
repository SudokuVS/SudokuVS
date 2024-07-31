using Microsoft.AspNetCore.Mvc;

namespace SudokuVS.RestApi;

[Route("/api/ping")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public string Ping() => "pong";
}
