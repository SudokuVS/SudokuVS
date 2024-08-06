using SudokuVS.Game.Exceptions;

namespace SudokuVS.Game.Abstractions;

public interface ISudokuGamesRepository
{
    IQueryable<SudokuGame> GetAllAsync();
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SudokuGame?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(SudokuGame game, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public static class SudokuGamesRepositoryExtensions
{
    public static async Task<SudokuGame> RequireAsync(this ISudokuGamesRepository repository, Guid id, CancellationToken cancellationToken = default) =>
        await repository.GetAsync(id, cancellationToken) ?? throw new GameNotFound(id);
}
