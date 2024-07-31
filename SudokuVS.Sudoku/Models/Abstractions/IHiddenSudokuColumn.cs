namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IHiddenSudokuColumn
{
    IHiddenSudokuCell this[int index] { get; }
}
