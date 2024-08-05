using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Models;

namespace SudokuVS.Server.Controllers.Games;

/// <summary>
///     Games
/// </summary>
[Route("/api/games")]
[Authorize]
[ApiController]
public class GamesController : ControllerBase
{
    readonly ISudokuGamesRepository _repository;

    /// <summary>
    /// </summary>
    public GamesController(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Search games
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<SudokuGameSummaryDto>> SearchGames() => (await _repository.GetAllAsync()).Select(game => game.ToSummaryDto());

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

        return game.ToSummaryDto();
    }
}
