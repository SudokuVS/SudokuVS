using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication;

public class ApiKeyEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // EF ctor
    public ApiKeyEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ApiKeyEntity(AppUser user)
    {
        CreationDate = DateTime.Now;
        User = user;
    }

    /// <summary>
    ///     The unique ID of the key
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    ///     The date at which the key was created.
    /// </summary>
    public DateTime CreationDate { get; private set; }

    /// <summary>
    ///     The user associated with the key
    /// </summary>
    public AppUser User { get; private set; }
}
