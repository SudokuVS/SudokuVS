using SudokuBattle.Sudoku.Models.Abstractions;

namespace SudokuBattle.App.Models.Game;

public class OtherPlayerState
{
    public OtherPlayerState(IHiddenSudokuGrid grid, PlayerSide side, string playerName)
    {
        Grid = grid;
        Side = side;
        PlayerName = playerName;
    }

    public IHiddenSudokuGrid Grid { get; }
    public PlayerSide Side { get; }
    public string PlayerName { get; }
}
