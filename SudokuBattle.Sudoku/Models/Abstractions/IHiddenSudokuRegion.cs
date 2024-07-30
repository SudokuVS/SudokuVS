namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IHiddenSudokuRegion
{
    IHiddenSudokuCell this[int rowIndex, int colIndex] { get; }
}
