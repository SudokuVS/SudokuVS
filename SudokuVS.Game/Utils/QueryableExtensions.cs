using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Exceptions;

namespace SudokuVS.Game.Utils;

public static class QueryableExtensions
{
    public static async Task<TEntity> RequireAsync<TEntity>(this DbSet<TEntity> set, object key, CancellationToken cancellationToken = default) where TEntity: class =>
        await set.FindAsync([key], cancellationToken) ?? throw new EntityNotFoundException<TEntity>(key);

    public static async Task<TEntity> RequireAsync<TEntity>(this IQueryable<TEntity> set, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        where TEntity: class =>
        await set.SingleOrDefaultAsync(predicate, cancellationToken) ?? throw new EntityNotFoundException<TEntity>();
}
