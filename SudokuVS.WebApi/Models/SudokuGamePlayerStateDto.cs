using SudokuVS.Game;

namespace SudokuVS.WebApi.Models;

public class SudokuGamePlayerStateDto
{
    /// <summary>
    ///     The user corresponding to the player.
    /// </summary>
    public required UserIdentityDto User { get; set; }

    /// <summary>
    ///     The grid as seen by the player.
    /// </summary>
    public required SudokuPlayerGridDto Grid { get; set; }
}

public static class SudokuGamePlayerStateMappingExtensions
{
    public static SudokuGamePlayerStateDto ToPlayerStateDto(this PlayerState state) =>
        new()
        {
            User = state.User.ToDto(),
            Grid = state.Grid.ToPlayerGridDto(state)
        };
}
