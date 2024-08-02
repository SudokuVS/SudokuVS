using System.Collections.Concurrent;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;

namespace SudokuVS.Apps.Common.Persistence;

public class SudokuGamesInMemory : ISudokuGamesRepository
{
    readonly ConcurrentDictionary<Guid, SudokuGame> _games = new();

    public IAsyncEnumerable<SudokuGame> GetAll(CancellationToken cancellationToken = default) => _games.Values.ToAsyncEnumerable();
    public Task<SudokuGame?> Get(Guid id, CancellationToken _ = default) => Task.FromResult(_games.GetValueOrDefault(id));
    public Task<bool> Exists(Guid id, CancellationToken _ = default) => Task.FromResult(_games.ContainsKey(id));

    public Task Save(SudokuGame game, CancellationToken _ = default)
    {
        SudokuGame existingGame = _games.GetOrAdd(game.Id, game);
        if (existingGame != game)
        {
            throw new InvalidOperationException("Two different games with the same id were added concurrently");
        }

        return Task.CompletedTask;
    }
}
