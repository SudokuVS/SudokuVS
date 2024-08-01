using SudokuVS.Game;

namespace SudokuVS.RestApi.Models;

public enum SudokuGamePlayerSideDto
{
    Player1,
    Player2
}

public static class SudokuGamePlayerSideMappingExtensions
{
    public static SudokuGamePlayerSideDto ToDto(this PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => SudokuGamePlayerSideDto.Player1,
            PlayerSide.Player2 => SudokuGamePlayerSideDto.Player2,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
