namespace SudokuVS.Game.Persistence;

public interface ISudokuGamesRepository
{
    IAsyncEnumerable<SudokuGame> GetAll(CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    Task<SudokuGame?> Get(Guid id, CancellationToken cancellationToken = default);
    Task Save(SudokuGame game, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
}
