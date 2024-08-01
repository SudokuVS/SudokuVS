using System.ComponentModel.DataAnnotations;
using SudokuVS.Game.Users;

namespace SudokuVS.RestApi.Models;

public class UserIdentityDto
{
    /// <summary>
    ///     The unique identifier of the player.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the player.
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

public static class UserIdentityMappingExtensions
{
    public static UserIdentityDto ToDto(this UserIdentity user) => new() { Id = user.Id, Name = user.Name };
}
