using SudokuBattle.Sudoku.Models;

namespace SudokuBattle.Game;

public class PlayerState
{
    readonly List<(int Row, int Column)> _hints = [];

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
    public IReadOnlyCollection<(int Row, int Column)> Hints => _hints;
    public int RemainingHints => Math.Max(0, Game.Options.MaxHints - Hints.Count);

    public event EventHandler? HintAdded;

    public void SetElement(int row, int column, int element) => Grid[row, column].Element = element;
    public void ClearElement(int row, int column) => Grid[row, column].Element = null;

    public void ToggleAnnotation(int row, int column, int element)
    {
        SudokuCell cell = Grid[row, column];
        if (!cell.Annotations.Remove(element))
        {
            cell.Annotations.Add(element);
        }
    }

    public void ClearAnnotation(int row, int column) => Grid[row, column].Annotations.Clear();

    public void UseHint(int row, int column)
    {
        Grid[row, column].Element = Game.SolvedGrid[row, column].Element;
        Grid[row, column].Locked = true;
        _hints.Add((row, column));

        HintAdded?.Invoke(this, EventArgs.Empty);
    }
}
