using SudokuBattle.Sudoku.Models.Abstractions;

namespace SudokuBattle.Game;

public interface IHiddenPlayerState
{
    string PlayerName { get; }
    IHiddenSudokuGrid Grid { get; }
    PlayerSide Side { get; }
    IReadOnlyCollection<(int Row, int Column)> Hints { get; }
    int RemainingHints { get; }
}
