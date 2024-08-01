using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.WebApi.Models;

namespace SudokuVS.WebApi.Controllers.Games;

[Route("/api/games")]
[OpenApiTag("Games")]
[Authorize]
[ApiController]
public class GamesController : ControllerBase
{
    readonly ISudokuGamesRepository _repository;

    public GamesController(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async IAsyncEnumerable<SudokuGameSummaryDto> SearchGames()
    {
        await foreach (SudokuGame game in _repository.GetAll())
        {
            yield return game.ToSummaryDto();
        }
    }

    [HttpGet("{id:guid}/summary")]
    public async Task<ActionResult<SudokuGameSummaryDto>> GetGame(Guid id)
    {
        SudokuGame? game = await _repository.Get(id);
        if (game == null)
        {
            return NotFound();
        }

        return game.ToSummaryDto();
    }

    [HttpPost]
    public ActionResult<SudokuGameSummaryDto> CreateGame(CreateGameRequest request)
    {
        SudokuGame? game = SudokuGame.Create(
            request.Name,
            new SudokuGameOptions
            {
                MaxHints = request.Hints
            }
        );

        if (game == null)
        {
            return Problem(title: "Failed to create game.");
        }

        return game.ToSummaryDto();
    }
}

public class CreateGameRequest
{
    /// <summary>
    ///     The name of the game.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The number of hints that can be used by each player during the round.
    /// </summary>
    public int Hints { get; init; }
}
