using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.WebApp.Exceptions;

namespace SudokuVS.WebApp.Services;

public class SudokuGamesInMemory : ISudokuGamesRepository
{
    readonly ConcurrentDictionary<Guid, SudokuGame> _games = new();

    public async IAsyncEnumerable<SudokuGame> GetAll([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (SudokuGame value in _games.Values)
        {
            yield return value;
        }
    }

    public Task<SudokuGame?> Get(Guid id, CancellationToken _ = default) => Task.FromResult(_games.GetValueOrDefault(id));

    public async Task<SudokuGame> Require(Guid id, CancellationToken cancellationToken = default) =>
        await Get(id, cancellationToken) ?? throw new NotFoundException<SudokuGame>(id);

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
