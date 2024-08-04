using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SudokuVS.WebApi.Areas.App.Pages;

public class GamePage : PageModel
{
    public string? GameId { get; set; }

    public void OnGet(string gameId) => GameId = gameId;
}
