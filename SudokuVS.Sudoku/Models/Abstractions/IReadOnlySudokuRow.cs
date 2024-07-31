namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuRow
{
    IReadOnlySudokuCell this[int index] { get; }
}
