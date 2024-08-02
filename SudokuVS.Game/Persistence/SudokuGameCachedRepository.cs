using SudokuVS.Game.Models;

namespace SudokuVS.Game.Persistence;

abstract class SudokuGameCachedRepository : ISudokuGamesRepository, IDisposable
{
    readonly SudokuGamesInMemory _localCache = new();
    readonly CancellationTokenSource _instanceCancellationSource = new();

    public void Dispose()
    {
        _instanceCancellationSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public IAsyncEnumerable<SudokuGame> GetAll(CancellationToken cancellationToken = default) =>
        ListIds(cancellationToken).SelectAwait(async id => await Get(id, cancellationToken)).OfType<SudokuGame>();

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default) =>
        await _localCache.Exists(id, cancellationToken) || await ExistsInDistributedRepository(id, cancellationToken);

    public async Task<SudokuGame?> Get(Guid id, CancellationToken cancellationToken = default)
    {
        SudokuGame? game = await _localCache.Get(id, cancellationToken);
        if (game != null)
        {
            return game;
        }

        game = await LoadFromDistributedRepository(id, cancellationToken);
        if (game == null)
        {
            return null;
        }

        await _localCache.Save(game, cancellationToken);
        SubscribeToChanges(game);

        return game;
    }

    public async Task Save(SudokuGame game, CancellationToken cancellationToken = default)
    {
        await SaveToDistributedRepository(game, cancellationToken);
        await _localCache.Save(game, cancellationToken);
        SubscribeToChanges(game);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await Exists(id, cancellationToken))
        {
            throw new InvalidOperationException($"Game {id} doesn't exist");
        }

        await DeleteFromDistributedRepository(id, cancellationToken);
        await _localCache.Delete(id, cancellationToken);
    }

    public async Task<SudokuGame?> Refresh(Guid id, CancellationToken cancellationToken = default)
    {
        await _localCache.Delete(id, cancellationToken);
        return await Get(id, cancellationToken);
    }

    protected abstract IAsyncEnumerable<Guid> ListIds(CancellationToken cancellationToken);
    protected abstract Task<bool> ExistsInDistributedRepository(Guid id, CancellationToken cancellationToken);
    protected abstract Task<SudokuGame?> LoadFromDistributedRepository(Guid id, CancellationToken cancellationToken);
    protected abstract Task SaveToDistributedRepository(SudokuGame game, CancellationToken cancellationToken);
    protected abstract Task DeleteFromDistributedRepository(Guid id, CancellationToken cancellationToken);

    void SubscribeToChanges(SudokuGame game)
    {
        SubscribeToPlayerStateChanges(game, PlayerSide.Player1);
        SubscribeToPlayerStateChanges(game, PlayerSide.Player2);

        game.PlayerJoined += (_, side) =>
        {
            SubscribeToPlayerStateChanges(game, side);
            SaveToDistributedRepository(game, _instanceCancellationSource.Token);
        };

        game.GameOver += (_, _) => SaveToDistributedRepository(game, _instanceCancellationSource.Token);
    }

    void SubscribeToPlayerStateChanges(SudokuGame game, PlayerSide side)
    {
        PlayerState? state = game.GetPlayerState(side);
        if (state == null)
        {
            return;
        }

        state.HintAdded += (_, _) => SaveToDistributedRepository(game, _instanceCancellationSource.Token);
        state.Grid.CellElementChanged += (_, _) => SaveToDistributedRepository(game, _instanceCancellationSource.Token);
        state.Grid.CellAnnotationsChanged += (_, _) => SaveToDistributedRepository(game, _instanceCancellationSource.Token);
        state.Grid.CellLockChanged += (_, _) => SaveToDistributedRepository(game, _instanceCancellationSource.Token);
    }
}
