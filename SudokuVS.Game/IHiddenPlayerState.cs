using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public interface IHiddenPlayerState
{
    string PlayerName { get; }
    IHiddenSudokuGrid Grid { get; }
    PlayerSide Side { get; }
    IReadOnlyCollection<(int Row, int Column)> Hints { get; }
    int RemainingHints { get; }
}
