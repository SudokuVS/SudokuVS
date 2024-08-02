using SudokuVS.Game.Models;
using SudokuVS.Game.Users;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public class HiddenPlayerState : IHiddenPlayerState
{
    readonly PlayerState _state;

    public HiddenPlayerState(PlayerState state)
    {
        _state = state;
    }

    public UserIdentity User => _state.User;
    public IHiddenSudokuGrid Grid => _state.Grid;
    public PlayerSide Side => _state.Side;
    public IReadOnlyCollection<(int Row, int Column)> Hints => _state.Hints;
    public int RemainingHints => _state.RemainingHints;
}
