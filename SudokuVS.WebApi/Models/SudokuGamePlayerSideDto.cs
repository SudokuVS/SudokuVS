using SudokuVS.Game;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Side of a player.
/// </summary>
public enum SudokuGamePlayerSideDto
{
    /// <summary>
    ///     Player 1 side.
    /// </summary>
    Player1,

    /// <summary>
    ///     Player 2 side.
    /// </summary>
    Player2
}

static class SudokuGamePlayerSideMappingExtensions
{
    public static SudokuGamePlayerSideDto ToDto(this PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => SudokuGamePlayerSideDto.Player1,
            PlayerSide.Player2 => SudokuGamePlayerSideDto.Player2,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

    public static PlayerSide FromDto(this SudokuGamePlayerSideDto side) =>
        side switch
        {
            SudokuGamePlayerSideDto.Player1 => PlayerSide.Player1,
            SudokuGamePlayerSideDto.Player2 => PlayerSide.Player2,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
