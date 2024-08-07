using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SudokuVS.Game;
using SudokuVS.Game.Services;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Authorization;
using SudokuVS.Server.Infrastructure.Database.Models;
using SudokuVS.Server.RestApi.Models;

namespace SudokuVS.Server.RestApi.Controllers.Games;

/// <summary>
///     Games
/// </summary>
[Route("/api/games")]
[Authorize(AuthorizationConstants.ApiAuthorizationPolicy)]
[ApiController]
public class GamesController : ControllerBase
{
    readonly UserManager<AppUser> _userManager;
    readonly GamesService _gamesService;

    /// <summary>
    /// </summary>
    public GamesController(UserManager<AppUser> userManager, GamesService gamesService)
    {
        _userManager = userManager;
        _gamesService = gamesService;
    }

    /// <summary>
    ///     Search games
    /// </summary>
    /// <remarks>
    ///     Get the games of the current user.
    /// </remarks>
    [HttpGet]
    public async Task<IEnumerable<SudokuGameSummaryDto>> SearchGames()
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        List<SudokuGame> games = _gamesService.GetGames(user).ToList();

        List<SudokuGameSummaryDto> result = [];
        foreach (SudokuGame game in games)
        {
            result.Add(await ToSummaryDto(game));
        }

        return result;
    }

    /// <summary>
    ///     Get game summary
    /// </summary>
    /// <remarks>
    ///     Get basic info about a game.
    /// </remarks>
    [HttpGet("{id:guid}/summary")]
    public async Task<SudokuGameSummaryDto> GetGameSummary(Guid id)
    {
        SudokuGame? game = await _gamesService.GetGameAsync(id);
        if (game == null)
        {
            throw new NotFoundException();
        }

        return await ToSummaryDto(game);
    }

    async Task<SudokuGameSummaryDto> ToSummaryDto(SudokuGame game)
    {
        UserIdentityDto? player1 = game.Player1 == null ? null : await game.Player1.GetUserIdentity(_userManager);
        UserIdentityDto? player2 = game.Player2 == null ? null : await game.Player2.GetUserIdentity(_userManager);

        return game.ToSummaryDto(player1, player2);
    }
}
