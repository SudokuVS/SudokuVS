using SudokuBattle.Sudoku.Models;

namespace SudokuBattle.App.Models.Game;

public class OtherPlayerState
{
    public OtherPlayerState(HiddenSudokuGrid grid, PlayerSide side, string playerName)
    {
        Grid = grid;
        Side = side;
        PlayerName = playerName;
    }

    public HiddenSudokuGrid Grid { get; }
    public PlayerSide Side { get; }
    public string PlayerName { get; }
}
