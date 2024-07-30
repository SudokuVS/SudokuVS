using SudokuBattle.App.Services;
using SudokuBattle.Sudoku.Models;

namespace SudokuBattle.App.Models.Game;

public class PlayerState
{
    public PlayerState(SudokuGame game, SudokuGrid grid, PlayerSide side, string playerName)
    {
        Game = game;
        Grid = grid;
        Side = side;
        PlayerName = playerName;
    }

    public SudokuGame Game { get; }
    public SudokuGrid Grid { get; }
    public PlayerSide Side { get; }
    public string PlayerName { get; }
}
