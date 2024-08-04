using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SudokuVS.WebApi.Controllers;

/// <summary>
///     The base pages
/// </summary>
[Route("/")]
public class HomeController : Controller
{
    /// <summary>
    ///     The login page
    /// </summary>
    [HttpGet]
    [Authorize]
    public ViewResult Index() => View();
}
