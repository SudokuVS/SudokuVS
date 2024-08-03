using System.Collections.Concurrent;

namespace SudokuVS.Game.Persistence;

class SudokuGamesInMemory : ISudokuGamesRepository
{
    readonly ConcurrentDictionary<Guid, SudokuGame> _games = new();

    public IAsyncEnumerable<SudokuGame> GetAllAsync(CancellationToken cancellationToken = default) => _games.Values.ToAsyncEnumerable();
    public Task<bool> ExistsAsync(Guid id, CancellationToken _ = default) => Task.FromResult(_games.ContainsKey(id));
    public Task<SudokuGame?> GetAsync(Guid id, CancellationToken _ = default) => Task.FromResult(_games.GetValueOrDefault(id));

    public Task SaveAsync(SudokuGame game, CancellationToken _ = default)
    {
        SudokuGame existingGame = _games.GetOrAdd(game.Id, game);
        if (existingGame != game)
        {
            throw new InvalidOperationException("Two different games with the same id were added concurrently");
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken _ = default)
    {
        if (!_games.TryRemove(id, out SudokuGame _))
        {
            throw new InvalidOperationException($"Game {id} doesn't exist");
        }

        return Task.CompletedTask;
    }
}
