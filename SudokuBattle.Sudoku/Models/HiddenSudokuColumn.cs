namespace SudokuBattle.Sudoku.Models;

public class HiddenSudokuColumn
{
    readonly HiddenSudokuCell[,] _grid;
    readonly int _colIndex;

    internal HiddenSudokuColumn(HiddenSudokuCell[,] grid, int colIndex)
    {
        if (colIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(colIndex), colIndex, "Expected col index to be between 0 and 8.");
        }

        _grid = grid;
        _colIndex = colIndex;
    }

    public HiddenSudokuCell this[int index] {
        get {
            if (index is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Expected index of column to be between 0 and 8.");
            }

            return _grid[index, _colIndex];
        }
    }
}