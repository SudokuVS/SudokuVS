using System.ComponentModel.DataAnnotations;

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
