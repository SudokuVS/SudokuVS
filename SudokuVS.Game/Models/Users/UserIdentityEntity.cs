using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Users;

namespace SudokuVS.Game.Models.Users;

public class UserIdentityEntity
{
    /// <summary>
    ///     The unique ID of the user in the database.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    ///     The unique ID of the user in the identity provider.
    /// </summary>
    public required Guid ExternalId { get; init; }

    /// <summary>
    ///     The name of the user.
    /// </summary>
    [MaxLength(64)]
    public required string Name { get; init; }
}

public static class UserIdentityEntityExtensions
{
    public static async Task<UserIdentityEntity> GetOrCreateAsync(this DbSet<UserIdentityEntity> set, UserIdentity user, CancellationToken cancellationToken = default)
    {
        UserIdentityEntity? existing = await set.SingleOrDefaultAsync(u => u.ExternalId == user.ExternalId, cancellationToken);
        if (existing != null)
        {
            return existing;
        }

        UserIdentityEntity newUser = new() { ExternalId = user.ExternalId, Name = user.Name };
        await set.AddAsync(newUser, cancellationToken);

        return newUser;
    }
}
