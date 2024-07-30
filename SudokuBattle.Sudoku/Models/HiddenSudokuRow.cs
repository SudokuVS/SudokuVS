namespace SudokuBattle.Sudoku.Models;

public class HiddenSudokuRow
{
    readonly HiddenSudokuCell[,] _grid;
    readonly int _rowIndex;

    internal HiddenSudokuRow(HiddenSudokuCell[,] grid, int rowIndex)
    {
        if (rowIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Expected row index to be between 0 and 8.");
        }

        _grid = grid;
        _rowIndex = rowIndex;
    }

    public HiddenSudokuCell this[int index] {
        get {
            if (index is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Expected index of row to be between 0 and 8.");
            }

            return _grid[_rowIndex, index];
        }
    }
}