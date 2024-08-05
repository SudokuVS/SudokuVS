using SudokuVS.Game.Models;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public class HiddenPlayerState : IHiddenPlayerState
{
    readonly PlayerState _state;

    public HiddenPlayerState(PlayerState state)
    {
        _state = state;
    }

    public event EventHandler? HintAdded { add => _state.HintAdded += value; remove => _state.HintAdded -= value; }

    public string Username => _state.Username;
    public IHiddenSudokuGrid Grid => _state.Grid;
    public PlayerSide Side => _state.Side;
    public IReadOnlyCollection<(int Row, int Column)> Hints => _state.Hints;
    public int RemainingHints => _state.RemainingHints;
}
