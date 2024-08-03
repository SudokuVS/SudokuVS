namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuGrid
{
    IReadOnlySudokuCell this[int rowIndex, int colIndex] { get; }
    IReadOnlyList<IReadOnlySudokuRegion> Regions { get; }
    IReadOnlyList<IReadOnlySudokuRow> Rows { get; }
    IReadOnlyList<IReadOnlySudokuColumn> Columns { get; }

    event EventHandler<(int Row, int Column)>? CellElementChanged;
    event EventHandler<(int Row, int Column)>? CellAnnotationsChanged;

    IEnumerable<IReadOnlySudokuCell> Enumerate();
}
