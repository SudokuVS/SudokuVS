using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public class HiddenPlayerState : IHiddenPlayerState
{
    readonly PlayerState _state;

    public HiddenPlayerState(PlayerState state)
    {
        _state = state;
    }

    public string PlayerName => _state.PlayerName;
    public IHiddenSudokuGrid Grid => _state.Grid;
    public PlayerSide Side => _state.Side;
    public IReadOnlyCollection<(int Row, int Column)> Hints => _state.Hints;
    public int RemainingHints => _state.RemainingHints;
}
