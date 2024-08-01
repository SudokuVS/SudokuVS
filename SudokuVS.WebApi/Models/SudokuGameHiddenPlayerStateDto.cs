using SudokuVS.Game;

namespace SudokuVS.WebApi.Models;

public class SudokuGameHiddenPlayerStateDto
{
    /// <summary>
    ///     The user corresponding to the player.
    /// </summary>
    public required UserIdentityDto User { get; set; }

    /// <summary>
    ///     The grid as seen by the player.
    /// </summary>
    public required SudokuHiddenPlayerGridDto Grid { get; set; }
}

public static class SudokuGameHiddenPlayerStateMappingExtensions
{
    public static SudokuGameHiddenPlayerStateDto ToHiddenPlayerStateDto(this IHiddenPlayerState state) =>
        new()
        {
            User = state.User.ToDto(),
            Grid = state.Grid.ToHiddenPlayerGridDto(state)
        };
}
