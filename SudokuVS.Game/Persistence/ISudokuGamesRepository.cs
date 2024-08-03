namespace SudokuVS.Game.Persistence;

public interface ISudokuGamesRepository
{
    IAsyncEnumerable<SudokuGame> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SudokuGame?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(SudokuGame game, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
