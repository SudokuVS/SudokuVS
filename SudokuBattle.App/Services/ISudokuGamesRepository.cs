using SudokuBattle.Game;

namespace SudokuBattle.App.Services;

public interface ISudokuGamesRepository
{
    Task<SudokuGame?> Get(Guid id, CancellationToken cancellationToken = default);
    Task<SudokuGame> Require(Guid id, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    Task Save(SudokuGame game, CancellationToken cancellationToken = default);
}
