namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuGrid
{
    SudokuCell this[int rowIndex, int colIndex] { get; }
    IReadOnlyList<IReadOnlySudokuRegion> Regions { get; }
    IReadOnlyList<IReadOnlySudokuRow> Rows { get; }
    IReadOnlyList<IReadOnlySudokuColumn> Columns { get; }
}
