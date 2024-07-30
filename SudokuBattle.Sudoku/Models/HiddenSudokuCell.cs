namespace SudokuBattle.Sudoku.Models;

public class HiddenSudokuCell
{
    readonly SudokuGrid _grid;

    public HiddenSudokuCell(SudokuGrid grid, int row, int column)
    {
        Row = row;
        Column = column;
        _grid = grid;
    }

    public int Row { get; }
    public int Column { get; }
    public int Region => _grid[Row, Column].Region;
    public bool Locked => _grid[Row, Column].Locked;
    public bool Empty => _grid[Row, Column].Empty;
    public bool HasAnnotations => _grid[Row, Column].Annotations.Count > 0;
}
