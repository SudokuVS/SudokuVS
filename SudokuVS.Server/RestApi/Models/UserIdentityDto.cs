using System.ComponentModel.DataAnnotations;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     Identity of a user.
/// </summary>
public class UserIdentityDto
{
    /// <summary>
    ///     The username of the user. This is the unique identifier of the user.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///     The public name of the user. This is the name the should be used as a display value when referring to the user.
    /// </summary>
    [Required]
    public required string PublicName { get; init; }
}
