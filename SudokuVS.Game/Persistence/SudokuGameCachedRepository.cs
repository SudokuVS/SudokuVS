using Microsoft.Extensions.Logging;
using SudokuVS.Game.Models;

namespace SudokuVS.Game.Persistence;

abstract class SudokuGameCachedRepository : ISudokuGamesRepository, IDisposable
{
    readonly SudokuGamesInMemory _localCache = new();
    readonly CancellationTokenSource _instanceCancellationSource = new();

    protected SudokuGameCachedRepository(ILogger<SudokuGameCachedRepository> logger)
    {
        Logger = logger;
    }

    protected ILogger<SudokuGameCachedRepository> Logger { get; }

    public void Dispose()
    {
        _instanceCancellationSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public IAsyncEnumerable<SudokuGame> GetAllAsync(CancellationToken cancellationToken = default) =>
        ListIdsAsync(cancellationToken).SelectAwait(async id => await GetAsync(id, cancellationToken)).OfType<SudokuGame>();

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _localCache.ExistsAsync(id, cancellationToken) || await ExistsInDistributedRepositoryAsync(id, cancellationToken);

    public async Task<SudokuGame?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        SudokuGame? game = await _localCache.GetAsync(id, cancellationToken);
        if (game != null)
        {
            return game;
        }

        game = await LoadFromDistributedRepositoryAsync(id, cancellationToken);
        if (game == null)
        {
            return null;
        }

        await _localCache.SaveAsync(game, cancellationToken);
        SubscribeToChanges(game);

        return game;
    }

    public async Task SaveAsync(SudokuGame game, CancellationToken cancellationToken = default)
    {
        await SaveToDistributedRepositoryAsync(game, cancellationToken);
        await _localCache.SaveAsync(game, cancellationToken);
        SubscribeToChanges(game);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException($"Game {id} doesn't exist");
        }

        await DeleteFromDistributedRepositoryAsync(id, cancellationToken);
        await _localCache.DeleteAsync(id, cancellationToken);
    }

    protected abstract IAsyncEnumerable<Guid> ListIdsAsync(CancellationToken cancellationToken);
    protected abstract Task<bool> ExistsInDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken);
    protected abstract Task<SudokuGame?> LoadFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken);
    protected abstract Task SaveToDistributedRepositoryAsync(SudokuGame game, CancellationToken cancellationToken);
    protected abstract Task DeleteFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken);

    void SubscribeToChanges(SudokuGame game)
    {
        SubscribeToPlayerStateChanges(game, PlayerSide.Player1);
        SubscribeToPlayerStateChanges(game, PlayerSide.Player2);

        game.PlayerJoined += (_, side) =>
        {
            SubscribeToPlayerStateChanges(game, side);
            SaveToDistributedRepository(game);
        };

        game.GameOver += (_, _) => SaveToDistributedRepository(game);
    }

    void SubscribeToPlayerStateChanges(SudokuGame game, PlayerSide side)
    {
        PlayerState? state = game.GetPlayerState(side);
        if (state == null)
        {
            return;
        }

        state.HintAdded += (_, _) => SaveToDistributedRepository(game);
        state.Grid.CellElementChanged += (_, _) => SaveToDistributedRepository(game);
        state.Grid.CellAnnotationsChanged += (_, _) => SaveToDistributedRepository(game);
        state.Grid.CellLockChanged += (_, _) => SaveToDistributedRepository(game);
    }

    // ReSharper disable once AsyncVoidMethod
    async void SaveToDistributedRepository(SudokuGame game)
    {
        try
        {
            await SaveToDistributedRepositoryAsync(game, _instanceCancellationSource.Token);
        }
        catch (Exception exn)
        {
            Logger.LogError(exn, "Could not save game {id} to repository", game.Id);
        }
    }
}
