using SudokuVS.Game.Models;
using SudokuVS.Game.Users;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public interface IHiddenPlayerState
{
    UserIdentity User { get; }
    IHiddenSudokuGrid Grid { get; }
    PlayerSide Side { get; }
    IReadOnlyCollection<(int Row, int Column)> Hints { get; }
    int RemainingHints { get; }
}
