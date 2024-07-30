namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IHiddenSudokuGrid
{
    IHiddenSudokuCell this[int rowIndex, int colIndex] { get; }
    IReadOnlyList<IHiddenSudokuRegion> Regions { get; }
    IReadOnlyList<IHiddenSudokuRow> Rows { get; }
    IReadOnlyList<IHiddenSudokuColumn> Columns { get; }

    event EventHandler<(int Row, int Column)>? CellValueChanged;
    event EventHandler<(int Row, int Column)>? CellAnnotationChanged;

    IEnumerable<(int Row, int Column, IHiddenSudokuCell Cell)> Enumerate();
}
