using SudokuBattle.Sudoku.Models.Abstractions;

namespace SudokuBattle.Sudoku.Models;

public class SudokuRow : IReadOnlySudokuRow, IHiddenSudokuRow
{
    readonly SudokuCell[,] _grid;
    readonly int _rowIndex;

    internal SudokuRow(SudokuCell[,] grid, int rowIndex)
    {
        if (rowIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Expected row index to be between 0 and 8.");
        }

        _grid = grid;
        _rowIndex = rowIndex;

        UpdateValidationState();
    }

    public bool IsCompleted { get; private set; }
    public bool IsValid { get; private set; }

    public SudokuCell this[int index] {
        get {
            if (index is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Expected index of row to be between 0 and 8.");
            }

            return _grid[_rowIndex, index];
        }
    }

    IReadOnlySudokuCell IReadOnlySudokuRow.this[int index] => this[index];
    IHiddenSudokuCell IHiddenSudokuRow.this[int index] => this[index];

    public void UpdateValidationState()
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
