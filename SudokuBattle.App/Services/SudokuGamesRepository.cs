using SudokuBattle.App.Exceptions;
using SudokuBattle.App.Models.Game;

namespace SudokuBattle.App.Services;

public class SudokuGamesRepository
{
    readonly Dictionary<Guid, SudokuGame> _games = new();
    readonly ILogger<SudokuGamesRepository> _logger;

    public SudokuGamesRepository(ILogger<SudokuGamesRepository> logger)
    {
        _logger = logger;
    }

    public SudokuGame? Get(Guid id) => _games.GetValueOrDefault(id);

    public SudokuGame Require(Guid id) => Get(id) ?? throw new NotFoundException<SudokuGame>(id);

    public bool Exists(Guid result) => _games.ContainsKey(result);

    public void Save(SudokuGame game)
    {
        _games[game.Id] = game;
        _logger.LogInformation("Saved game {id}", game.Id);
    }
}
