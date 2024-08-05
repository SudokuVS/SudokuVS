using System.ComponentModel.DataAnnotations;
using SudokuVS.Game.Users;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     Identity of a user.
/// </summary>
public class UserIdentityDto
{
    /// <summary>
    ///     The unique identifier of the user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     The name of the user.
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class UserIdentityMappingExtensions
{
    public static UserIdentityDto ToDto(this UserIdentity user) => new() { Id = user.Username, Name = user.DisplayName };
}
