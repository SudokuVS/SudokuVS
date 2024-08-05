using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Game.Users;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     State of a player.
/// </summary>
public class SudokuGamePlayerStateDto
{
    /// <summary>
    ///     The user corresponding to the player.
    /// </summary>
    [Required]
    public required UserIdentityDto User { get; set; }

    /// <summary>
    ///     The grid as seen by the player.
    /// </summary>
    [Required]
    public required SudokuPlayerGridDto Grid { get; set; }
}

static class SudokuGamePlayerStateMappingExtensions
{
    public static SudokuGamePlayerStateDto ToPlayerStateDto(this PlayerState state, UserIdentity user) =>
        new()
        {
            User = user.ToDto(),
            Grid = state.Grid.ToPlayerGridDto(state)
        };
}
