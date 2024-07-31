using Microsoft.AspNetCore.Mvc;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.RestApi.Models;

namespace SudokuVS.RestApi.Games;

[Route("/api/game")]
[ApiController]
public class GameController : ControllerBase
{
    readonly ISudokuGamesRepository _repository;

    public GameController(ISudokuGamesRepository repository)
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
