using SudokuVS.Game;
using SudokuVS.Game.Persistence;

namespace SudokuVS.Server.Services;

public class GamesService
{
    readonly ISudokuGamesRepository _repository;

    public GamesService(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    public async Task<SudokuGame?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default) => await _repository.GetAsync(gameId, cancellationToken);

    public async Task<IReadOnlyList<SudokuGame>> GetGamesAsync(CancellationToken cancellationToken = default) => await _repository.GetAllAsync(cancellationToken);
}
