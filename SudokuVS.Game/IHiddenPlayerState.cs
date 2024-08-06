using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public interface IHiddenPlayerState
{
    string Username { get; }
    IHiddenSudokuGrid Grid { get; }
    PlayerSide Side { get; }
    IReadOnlyCollection<(int Row, int Column)> Hints { get; }
    int RemainingHints { get; }
    public event EventHandler? HintAdded;
}
