namespace SudokuBattle.Sudoku.Models;

public class HiddenSudokuRegion
{
    readonly HiddenSudokuCell[,] _grid;
    readonly int _rowOffset;
    readonly int _colOffset;

    internal HiddenSudokuRegion(HiddenSudokuCell[,] grid, int regionIndex)
    {
        if (regionIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(regionIndex), regionIndex, "Expected region index to be between 0 and 8.");
        }

        _grid = grid;
        _rowOffset = regionIndex / 3;
        _colOffset = regionIndex % 3;
    }

    public HiddenSudokuCell this[int rowIndex, int colIndex] {
        get {
            if (rowIndex is < 0 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Expected row index of region to be between 0 and 2.");
            }

            if (colIndex is < 0 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(colIndex), colIndex, "Expected col index of region to be between 0 and 2.");
            }

            return _grid[_rowOffset + rowIndex, _colOffset + colIndex];
        }
    }
}