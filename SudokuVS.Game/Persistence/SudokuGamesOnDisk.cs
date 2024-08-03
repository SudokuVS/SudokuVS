using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SudokuVS.Game.Serialization;

namespace SudokuVS.Game.Persistence;

class SudokuGamesOnDisk : SudokuGameCachedRepository
{
    const string SaveFileExtension = "save";

    readonly SudokuGameJsonSerializer _serializer;
    readonly string _directory;
    readonly string _directoryFullPath;
    readonly ConcurrentDictionary<Guid, SemaphoreSlim> _semaphores = new();
    readonly ILogger<SudokuGamesOnDisk> _logger;

    public SudokuGamesOnDisk(string directory, ILogger<SudokuGamesOnDisk> logger)
    {
        string directoryFullPath = Path.GetFullPath(directory);
        if (!Path.Exists(directoryFullPath))
        {
            Directory.CreateDirectory(directoryFullPath);
        }

        _directory = directory;
        _directoryFullPath = directoryFullPath;
        _logger = logger;

#if DEBUG
        const bool indented = true;
#else 
        const bool indented = false;
#endif
        _serializer = new SudokuGameJsonSerializer(indented);
    }

    protected override IAsyncEnumerable<Guid> ListIdsAsync(CancellationToken cancellationToken) =>
        Directory.EnumerateFiles(_directory).Select(f => IsSaveFilePath(f, out Guid id) ? id : default).Where(id => id != default).ToAsyncEnumerable();

    protected override async Task<SudokuGame?> LoadFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken)
    {
        string path = GetSaveFilePath(id);
        if (!File.Exists(path))
        {
            return null;
        }

        string save;
        try
        {
            save = await File.ReadAllTextAsync(path, cancellationToken);
            _logger.LogDebug("Read game {id} from {path}.", id, path);
        }
        catch (Exception exn)
        {
            _logger.LogWarning(exn, "Exception while reading save file at {path}", path);
            return null;
        }

        return _serializer.Deserialize(save);
    }

    protected override Task<bool> ExistsInDistributedRepositoryAsync(Guid id, CancellationToken _)
    {
        string saveFileName = GetSaveFilePath(id);
        return Task.FromResult(File.Exists(saveFileName));
    }

    protected override async Task SaveToDistributedRepositoryAsync(SudokuGame game, CancellationToken cancellationToken)
    {
        string path = GetSaveFilePath(game.Id);
        string serialized = _serializer.Serialize(game);

        SemaphoreSlim semaphore = _semaphores.GetOrAdd(game.Id, new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        await File.WriteAllTextAsync(path, serialized, cancellationToken);
        _logger.LogDebug("Saved game {id} to {path}.", game.Id, path);

        semaphore.Release();
    }

    protected override async Task DeleteFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken)
    {
        string path = GetSaveFilePath(id);

        SemaphoreSlim semaphore = _semaphores.GetOrAdd(id, new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        File.Delete(path);
        _logger.LogDebug("Deleted game {id} at {path}...", id, path);

        semaphore.Release();
    }

    string GetSaveFilePath(Guid id) => Path.Join(_directoryFullPath, $"{id}.{SaveFileExtension}");

    bool IsSaveFilePath(string path, out Guid id)
    {
        string extension = Path.GetExtension(path);
        if (extension != $".{SaveFileExtension}")
        {
            id = default;
            return false;
        }

        string absolutePath = Path.GetFullPath(path);
        if (!absolutePath.StartsWith(_directoryFullPath))
        {
            id = default;
            return false;
        }

        string fileName = Path.GetFileNameWithoutExtension(path);
        return Guid.TryParse(fileName, out id);
    }
}
