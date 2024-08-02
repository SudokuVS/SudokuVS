using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Exceptions;

namespace SudokuVS.Game.Utils;

public static class EntityFrameworkExtensions
{
    public static async Task<TEntity> RequireAsync<TEntity>(this DbSet<TEntity> set, object key, CancellationToken cancellationToken = default) where TEntity: class =>
        await set.FindAsync([key], cancellationToken) ?? throw new EntityNotFoundException<TEntity>(key);
}
