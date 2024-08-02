using System.ComponentModel.DataAnnotations;

namespace SudokuVS.Game.Models.Users;

public class UserIdentityEntity
{
    public required Guid Id { get; init; }

    [MaxLength(64)]
    public required string Name { get; init; }
}
