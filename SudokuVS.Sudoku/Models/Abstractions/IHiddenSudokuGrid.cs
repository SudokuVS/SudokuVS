namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IHiddenSudokuGrid
{
    IHiddenSudokuCell this[int rowIndex, int colIndex] { get; }
    IReadOnlyList<IHiddenSudokuRegion> Regions { get; }
    IReadOnlyList<IHiddenSudokuRow> Rows { get; }
    IReadOnlyList<IHiddenSudokuColumn> Columns { get; }

    event EventHandler<(int Row, int Column)>? CellElementChanged;
    event EventHandler<(int Row, int Column)>? CellAnnotationsChanged;

    IEnumerable<IHiddenSudokuCell> Enumerate();
}
