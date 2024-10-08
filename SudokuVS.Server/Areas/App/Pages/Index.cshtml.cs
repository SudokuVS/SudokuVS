﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SudokuVS.Game;
using SudokuVS.Game.Services;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Areas.App.Pages;

public class Index : PageModel
{
    readonly UserManager<AppUser> _userManager;
    readonly GameplayService _gameplayService;
    readonly GamesService _gamesService;

    public Index(UserManager<AppUser> userManager, GameplayService gameplayService, GamesService gamesService)
    {
        _userManager = userManager;
        _gameplayService = gameplayService;
        _gamesService = gamesService;
    }

    [BindProperty]
    public NewGameModel? NewGame { get; set; }

    public IReadOnlyList<SudokuGame>? Games { get; set; }

    public void OnGet()
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        NewGame ??= new NewGameModel();
        Games ??= _gamesService.GetGames(user).ToList();
    }

    public async Task<RedirectResult> OnPostAsync()
    {
        NewGame ??= new NewGameModel();

        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.StartNewGameAsync(NewGame.Name, new SudokuGameOptions { MaxHints = NewGame.MaxHints }, user);
        PlayerState? playerState = game.GetPlayerState(user);
        if (playerState == null)
        {
            throw new InternalErrorException("Player has not joined game");
        }

        return Redirect($"/app/game/{game.Id}");
    }

    public class NewGameModel
    {
        public string? Name { get; set; }
        public int MaxHints { get; set; } = 3;
    }
}
