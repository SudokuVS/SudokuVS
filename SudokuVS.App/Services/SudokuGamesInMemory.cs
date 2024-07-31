using SudokuVS.Game;
using SudokuVS.App.Exceptions;

namespace SudokuVS.App.Services;

public class SudokuGamesInMemory : ISudokuGamesRepository
{
    readonly Dictionary<Guid, SudokuGame> _games = new();

    public Task<SudokuGame?> Get(Guid id, CancellationToken _ = default) => Task.FromResult(_games.GetValueOrDefault(id));

    public async Task<SudokuGame> Require(Guid id, CancellationToken cancellationToken = default) =>
        await Get(id, cancellationToken) ?? throw new NotFoundException<SudokuGame>(id);

    public Task<bool> Exists(Guid id, CancellationToken _ = default) => Task.FromResult(_games.ContainsKey(id));

    public Task Save(SudokuGame game, CancellationToken _ = default)
    {
        _games[game.Id] = game;
        return Task.CompletedTask;
    }
}
