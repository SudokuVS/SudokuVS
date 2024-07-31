namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuColumn
{
    IReadOnlySudokuCell this[int index] { get; }
}
