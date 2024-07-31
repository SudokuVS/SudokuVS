using System.Collections.Concurrent;
using SudokuVS.App.Exceptions;
using SudokuVS.Game;
using SudokuVS.Game.Serialization;

namespace SudokuVS.App.Services;

public class SudokuGamesOnDisk : ISudokuGamesRepository
{
#if DEBUG
    readonly SudokuGameJsonSerializer _serializer = new(true);
#else
    readonly SudokuGameJsonSerializer _serializer = new();
#endif
    readonly SudokuGamesInMemory _cache = new();
    readonly HashSet<Guid> _subscribedTo = [];
    readonly string _directory;
    readonly ILogger<SudokuGamesOnDisk> _logger;

    public SudokuGamesOnDisk(string directory, ILogger<SudokuGamesOnDisk> logger)
    {
        if (!Path.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _directory = directory;
        _logger = logger;
    }

    public async Task<SudokuGame?> Get(Guid id, CancellationToken cancellationToken = default) =>
        await _cache.Get(id, cancellationToken) ?? await LoadFromDisk(id, cancellationToken);

    public async Task<SudokuGame> Require(Guid id, CancellationToken cancellationToken = default) =>
        await Get(id, cancellationToken) ?? throw new NotFoundException<SudokuGame>(id);

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default) =>
        await _cache.Exists(id, cancellationToken) || await ExistsOnDisk(id, cancellationToken);

    public async Task Save(SudokuGame game, CancellationToken cancellationToken = default)
    {
        await SaveToDisk(game, cancellationToken);
        await _cache.Save(game, cancellationToken);
        SubscribeToChanges(game);
    }

    async Task<SudokuGame?> LoadFromDisk(Guid id, CancellationToken cancellationToken)
    {
        string path = GetSaveFilePath(id);
        if (!File.Exists(path))
        {
            return null;
        }

        string save;
        try
        {
            _logger.LogDebug("Reading game {id} from {path}...", id, path);
            save = await File.ReadAllTextAsync(path, cancellationToken);
            _logger.LogDebug("Read game {id} from {path}.", id, path);
        }
        catch (Exception exn)
        {
            _logger.LogWarning(exn, "Exception while reading save file at {path}", path);
            return null;
        }

        SudokuGame? game = _serializer.Deserialize(save);
        if (game == null)
        {
            return null;
        }

        await _cache.Save(game, cancellationToken);
        SubscribeToChanges(game);
        return game;
    }

    Task<bool> ExistsOnDisk(Guid id, CancellationToken _)
    {
        string saveFileName = GetSaveFilePath(id);
        return Task.FromResult(File.Exists(saveFileName));
    }

    async Task SaveToDisk(SudokuGame game, CancellationToken cancellationToken)
    {
        string path = GetSaveFilePath(game.Id);
        string serialized = _serializer.Serialize(game);

        _logger.LogDebug("Saving game {id} to {path}...", game.Id, path);
        await File.WriteAllTextAsync(path, serialized, cancellationToken);
        _logger.LogDebug("Saved game {id} to {path}.", game.Id, path);
    }

    void SubscribeToChanges(SudokuGame game)
    {
        SubscribeToPlayerStateChanges(game, PlayerSide.Player1);
        SubscribeToPlayerStateChanges(game, PlayerSide.Player2);

        game.PlayerJoined += (_, side) =>
        {
            SubscribeToPlayerStateChanges(game, side);
            SaveToDiskDebounced(game);
        };

        game.GameOver += (_, _) => SaveToDiskDebounced(game);
    }

    void SubscribeToPlayerStateChanges(SudokuGame game, PlayerSide side)
    {
        PlayerState? state = game.GetPlayerState(side);
        if (state == null)
        {
            return;
        }

        state.HintAdded += (_, _) => SaveToDiskDebounced(game);
        state.Grid.CellValueChanged += (_, _) => SaveToDiskDebounced(game);
        state.Grid.CellAnnotationChanged += (_, _) => SaveToDiskDebounced(game);
        state.Grid.CellLockChanged += (_, _) => SaveToDiskDebounced(game);
    }

    string GetSaveFilePath(Guid id) => Path.Join(_directory, $"{id}.save");

    #region Save with debounce

    readonly ConcurrentDictionary<Guid, int> _debounceReferences = new();
    const int SaveDebounceDelayMs = 500;

    void SaveToDiskDebounced(SudokuGame game)
    {
        // source implementation https://stackoverflow.com/questions/28472205/c-sharp-event-debounce
        int last = _debounceReferences.GetOrAdd(game.Id, 1);
        Task.Delay(SaveDebounceDelayMs)
            .ContinueWith(
                async task =>
                {
                    int current = _debounceReferences.AddOrUpdate(game.Id, 1, (k, v) => Interlocked.Increment(ref v)) - 1;
                    if (current == last)
                    {
                        await SaveToDisk(game, default);
                    }
                    task.Dispose();
                }
            );
    }

    #endregion
}
