namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuRow
{
    IReadOnlySudokuCell this[int index] { get; }
}
