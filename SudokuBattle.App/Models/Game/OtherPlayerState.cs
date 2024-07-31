using SudokuBattle.Sudoku.Models.Abstractions;

namespace SudokuBattle.App.Models.Game;

public class OtherPlayerState
{
    readonly PlayerState _state;

    public OtherPlayerState(PlayerState state)
    {
        _state = state;
    }

    public string PlayerName => _state.PlayerName;
    public IHiddenSudokuGrid Grid => _state.Grid;
    public PlayerSide Side => _state.Side;
    public IReadOnlyCollection<(int Row, int Column)> Hints => _state.Hints;
}
