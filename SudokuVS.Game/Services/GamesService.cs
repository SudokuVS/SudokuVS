using SudokuVS.Game.Abstractions;

namespace SudokuVS.Game.Services;

public class GamesService
{
    readonly ISudokuGamesRepository _repository;

    public GamesService(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    public async Task<SudokuGame?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default) => await _repository.GetAsync(gameId, cancellationToken);

    public IEnumerable<SudokuGame> GetGames(string username) =>
        _repository.GetAllAsync().Where(g => g.Player1 != null && g.Player1.Username == username || g.Player2 != null && g.Player2.Username == username);
}
