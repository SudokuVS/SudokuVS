using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Game;

public static class SudokuCellExtensions
{
    public static int GetFlatIndex(this IHiddenSudokuCell cell) => cell.Row * 9 + cell.Column;
}
