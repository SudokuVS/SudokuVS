namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IHiddenSudokuRow
{
    IHiddenSudokuCell this[int index] { get; }
}
