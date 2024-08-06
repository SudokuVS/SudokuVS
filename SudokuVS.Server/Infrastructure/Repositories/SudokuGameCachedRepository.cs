using SudokuVS.Game;
using SudokuVS.Game.Abstractions;

namespace SudokuVS.Server.Infrastructure.Repositories;

abstract class SudokuGameCachedRepository : ISudokuGamesRepository, IDisposable
{
    readonly SudokuGamesInMemory LocalCache = new();
    readonly CancellationTokenSource _instanceCancellationSource = new();

    protected SudokuGameCachedRepository(ILogger<SudokuGameCachedRepository> logger)
    {
        Logger = logger;
    }

    protected ILogger<SudokuGameCachedRepository> Logger { get; }

    public async Task PreloadAsync(CancellationToken cancellationToken = default)
    {
        await foreach (SudokuGame game in LoadAllAsync().WithCancellation(cancellationToken))
        {
            await LocalCache.SaveAsync(game, cancellationToken);
            SubscribeToChanges(game);
        }
    }

    public void Dispose()
    {
        _instanceCancellationSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public IQueryable<SudokuGame> GetAllAsync() => LocalCache.GetAllAsync();
    public async Task<SudokuGame?> GetAsync(Guid id, CancellationToken cancellationToken = default) => await LocalCache.GetAsync(id, cancellationToken);
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => await LocalCache.ExistsAsync(id, cancellationToken);

    public async Task SaveAsync(SudokuGame game, CancellationToken cancellationToken = default)
    {
        await SaveToDistributedRepositoryAsync(game, cancellationToken);
        await LocalCache.SaveAsync(game, cancellationToken);
        SubscribeToChanges(game);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException($"Game {id} doesn't exist");
        }

        await DeleteFromDistributedRepositoryAsync(id, cancellationToken);
        await LocalCache.DeleteAsync(id, cancellationToken);
    }

    protected abstract IAsyncEnumerable<SudokuGame> LoadAllAsync();
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
