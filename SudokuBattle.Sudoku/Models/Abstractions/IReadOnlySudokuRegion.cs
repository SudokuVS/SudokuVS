namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuRegion
{
    SudokuCell this[int rowIndex, int colIndex] { get; }
}
