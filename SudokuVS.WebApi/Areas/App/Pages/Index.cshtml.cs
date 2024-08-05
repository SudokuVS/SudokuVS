using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SudokuVS.Game;
using SudokuVS.Game.Users;
using SudokuVS.WebApi.Exceptions;
using SudokuVS.WebApi.Services;

namespace SudokuVS.WebApi.Areas.App.Pages;

public class Index : PageModel
{
    readonly GameplayService _gameplayService;
    readonly GamesService _gamesService;

    public Index(GameplayService gameplayService, GamesService gamesService)
    {
        _gameplayService = gameplayService;
        _gamesService = gamesService;
    }

    [BindProperty]
    public NewGameModel? NewGame { get; set; }

    public IReadOnlyList<SudokuGame>? Games { get; set; }

    public async Task OnGetAsync()
    {
        NewGame ??= new NewGameModel();
        Games ??= await _gamesService.GetGamesAsync();
    }

    public async Task<RedirectResult> OnPostAsync()
    {
        NewGame ??= new NewGameModel();

        UserIdentity user = HttpContext.User.GetUserIdentity() ?? throw new AccessDeniedException();

        SudokuGame game = await _gameplayService.CreateGameAsync(NewGame.Name, new SudokuGameOptions { MaxHints = NewGame.MaxHints }, user);
        PlayerState? playerState = game.GetPlayerState(user.ExternalId);
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
