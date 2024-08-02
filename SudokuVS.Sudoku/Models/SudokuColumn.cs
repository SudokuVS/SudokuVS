using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Sudoku.Models;

public class SudokuColumn : IReadOnlySudokuColumn, IHiddenSudokuColumn
{
    readonly SudokuCell[,] _grid;
    readonly int _colIndex;

    internal SudokuColumn(SudokuCell[,] grid, int colIndex)
    {
        if (colIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(colIndex), colIndex, "Expected col index to be between 0 and 8.");
        }

        _grid = grid;
        _colIndex = colIndex;

        UpdateValidationState();
    }

    public bool IsCompleted { get; private set; }
    public bool IsValid { get; private set; }

    public SudokuCell this[int index] {
        get {
            if (index is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Expected index of column to be between 0 and 8.");
            }

            return _grid[index, _colIndex];
        }
    }

    IReadOnlySudokuCell IReadOnlySudokuColumn.this[int index] => this[index];
    IHiddenSudokuCell IHiddenSudokuColumn.this[int index] => this[index];

    /// <summary>
    ///     Update the values of <see cref="IsCompleted" /> and <see cref="IsValid" />.
    /// </summary>
    /// <remarks>
    ///     This method is called by the parent grid when cell events are triggered
    /// </remarks>
    internal void UpdateValidationState()
    {
        IsCompleted = true;
        IsValid = true;
        HashSet<int> seen = [];
        for (int i = 0; i < 9; i++)
        {
            int? element = this[i].Element;
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
