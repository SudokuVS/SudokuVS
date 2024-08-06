using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SudokuVS.Game;
using SudokuVS.Game.Users;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Models;
using SudokuVS.Server.RestApi.Models;

namespace SudokuVS.Server.RestApi.Controllers.Games;

/// <summary>
///     Games
/// </summary>
[Route("/api/games")]
[Authorize]
[ApiController]
public class GamesController : ControllerBase
{
    readonly UserManager<AppUser> _userManager;
    readonly ISudokuGamesRepository _repository;

    /// <summary>
    /// </summary>
    public GamesController(UserManager<AppUser> userManager, ISudokuGamesRepository repository)
    {
        _repository = repository;
        _userManager = userManager;
    }

    /// <summary>
    ///     Search games
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<SudokuGameSummaryDto>> SearchGames() => await Task.WhenAll((await _repository.GetAllAsync()).Select(ToSummaryDto));

    /// <summary>
    ///     Get game summary
    /// </summary>
    [HttpGet("{id:guid}/summary")]
    public async Task<SudokuGameSummaryDto> GetGameSummary(Guid id)
    {
        SudokuGame? game = await _repository.GetAsync(id);
        if (game == null)
        {
            throw new NotFoundException();
        }

        return await ToSummaryDto(game);
    }

    async Task<SudokuGameSummaryDto> ToSummaryDto(SudokuGame game)
    {
        UserIdentity? player1 = game.Player1 == null ? null : await game.Player1.GetUserIdentity(_userManager);
        UserIdentity? player2 = game.Player2 == null ? null : await game.Player2.GetUserIdentity(_userManager);

        return game.ToSummaryDto(player1, player2);
    }
}
