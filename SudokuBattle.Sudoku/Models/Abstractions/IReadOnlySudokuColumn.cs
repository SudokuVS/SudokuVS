namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuColumn
{
    SudokuCell this[int index] { get; }
}
