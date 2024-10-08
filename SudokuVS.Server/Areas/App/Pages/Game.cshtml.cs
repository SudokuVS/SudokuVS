﻿using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SudokuVS.Server.Areas.App.Pages;

public class GamePage : PageModel
{
    public string? GameId { get; set; }

    public void OnGet(string gameId) => GameId = gameId;
}
