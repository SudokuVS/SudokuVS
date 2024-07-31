using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Sudoku.Models;

public class SudokuRegion : IReadOnlySudokuRegion, IHiddenSudokuRegion
{
    readonly SudokuCell[,] _grid;

    internal SudokuRegion(SudokuCell[,] grid, int regionIndex)
    {
        if (regionIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(regionIndex), regionIndex, "Expected region index to be between 0 and 8.");
        }

        _grid = grid;
        Row = regionIndex / 3;
        Column = regionIndex % 3;

        UpdateValidationState();
    }

    public int Row { get; }
    public int Column { get; }
    public bool IsCompleted { get; private set; }
    public bool IsValid { get; private set; }

    public SudokuCell this[int rowIndex, int colIndex] {
        get {
            if (rowIndex is < 0 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Expected row index of region to be between 0 and 2.");
            }

            if (colIndex is < 0 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(colIndex), colIndex, "Expected col index of region to be between 0 and 2.");
            }

            return _grid[Row * 3 + rowIndex, Column * 3 + colIndex];
        }
    }

    IReadOnlySudokuCell IReadOnlySudokuRegion.this[int rowIndex, int colIndex] => this[rowIndex, colIndex];
    IHiddenSudokuCell IHiddenSudokuRegion.this[int rowIndex, int colIndex] => this[rowIndex, colIndex];

    public void UpdateValidationState()
    {
        IsCompleted = true;
        IsValid = true;
        HashSet<int> seen = [];
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            int? element = this[i, j].Element;
            if (element == null)
            {
                IsCompleted = false;
                continue;
            }

            if (!IsValid)
            {
                continue;
            }

            if (!seen.Add(element.Value))
            {
                IsValid = false;
            }
        }
    }
}
