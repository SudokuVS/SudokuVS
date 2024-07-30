namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuRow
{
    SudokuCell this[int index] { get; }
}
