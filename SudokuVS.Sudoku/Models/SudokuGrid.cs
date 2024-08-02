using SudokuVS.Sudoku.Models.Abstractions;
using SudokuVS.Sudoku.Utils;

namespace SudokuVS.Sudoku.Models;

public class SudokuGrid : IReadOnlySudokuGrid, IHiddenSudokuGrid
{
    readonly SudokuCell[,] _grid;

    public SudokuGrid(int[,] grid) : this(BuildCellGridFromValues(grid))
    {
    }

    public SudokuGrid(SudokuCell[,] grid)
    {
        _grid = grid;
        foreach (SudokuCell cell in grid)
        {
            cell.ElementChanged += (_, _) => OnCellValueChanged(cell);
            cell.AnnotationsChanged += (_, _) => CellAnnotationsChanged?.Invoke(this, (cell.Row, cell.Column));
            cell.LockChanged += (_, _) => CellLockChanged?.Invoke(this, (cell.Row, cell.Column));
        }

        Regions = Enumerable.Range(0, 9).Select(i => new SudokuRegion(_grid, i)).ToList();
        Rows = Enumerable.Range(0, 9).Select(i => new SudokuRow(_grid, i)).ToList();
        Columns = Enumerable.Range(0, 9).Select(i => new SudokuColumn(_grid, i)).ToList();
    }

    public IReadOnlyList<SudokuRegion> Regions { get; set; }
    IReadOnlyList<IReadOnlySudokuRegion> IReadOnlySudokuGrid.Regions => Regions;
    IReadOnlyList<IHiddenSudokuRegion> IHiddenSudokuGrid.Regions => Regions;

    public IReadOnlyList<SudokuRow> Rows { get; set; }
    IReadOnlyList<IReadOnlySudokuRow> IReadOnlySudokuGrid.Rows => Rows;
    IReadOnlyList<IHiddenSudokuRow> IHiddenSudokuGrid.Rows => Rows;

    public IReadOnlyList<SudokuColumn> Columns { get; set; }
    IReadOnlyList<IReadOnlySudokuColumn> IReadOnlySudokuGrid.Columns => Columns;
    IReadOnlyList<IHiddenSudokuColumn> IHiddenSudokuGrid.Columns => Columns;

    public bool IsValid => Regions.All(r => r is { IsValid: true }) && Rows.All(r => r is { IsValid: true }) && Columns.All(c => c is { IsValid: true });
    public bool IsCompleted => Regions.All(r => r is { IsCompleted: true });

    public event EventHandler<(int Row, int Column)>? CellElementChanged;
    public event EventHandler<(int Row, int Column)>? CellAnnotationsChanged;
    public event EventHandler<(int Row, int Column)>? CellLockChanged;

    public SudokuCell this[int rowIndex, int colIndex] {
        get {
            if (rowIndex is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Expected row index to be between 0 and 8.");
            }

            if (colIndex is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(colIndex), colIndex, "Expected col index to be between 0 and 8.");
            }

            return _grid[rowIndex, colIndex];
        }
    }

    public SudokuCell this[int index] {
        get {
            if (index is < 0 or > 80)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Expected index to be between 0 and 80.");
            }

            (int rowIndex, int colIndex) = SudokuGridCoordinates.ComputeCoordinates(index);

            return _grid[rowIndex, colIndex];
        }
    }

    IReadOnlySudokuCell IReadOnlySudokuGrid.this[int rowIndex, int colIndex] => this[rowIndex, colIndex];
    IHiddenSudokuCell IHiddenSudokuGrid.this[int rowIndex, int colIndex] => this[rowIndex, colIndex];

    public void LockNonEmptyCells()
    {
        foreach (SudokuCell cell in _grid)
        {
            if (!cell.IsEmpty)
            {
                cell.IsLocked = true;
            }
        }
    }

    public void CopyLocksFrom(SudokuGrid grid)
    {
        foreach (SudokuCell cell in grid.Enumerate())
        {
            if (!cell.IsEmpty)
            {
                cell.IsLocked = true;
            }
        }
    }

    public IEnumerable<SudokuCell> Enumerate()
    {
        for (int i = 0; i < Rows.Count; i++)
        for (int j = 0; j < Columns.Count; j++)
        {
            yield return this[i, j];
        }
    }

    IEnumerable<IReadOnlySudokuCell> IReadOnlySudokuGrid.Enumerate() => Enumerate();
    IEnumerable<IHiddenSudokuCell> IHiddenSudokuGrid.Enumerate() => Enumerate();

    void OnCellValueChanged(SudokuCell cell)
    {
        Rows[cell.Row].UpdateValidationState();
        Columns[cell.Column].UpdateValidationState();
        Regions[cell.Region].UpdateValidationState();

        CellElementChanged?.Invoke(this, (cell.Row, cell.Column));
    }

    public static SudokuGrid CreateEmpty() => new(new int[9, 9]);

    public static SudokuGrid Clone(SudokuGrid grid)
    {
        SudokuCell[,] newGrid = new SudokuCell[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            newGrid[i, j] = SudokuCell.Clone(grid[i, j]);
        }

        return new SudokuGrid(newGrid);
    }

    static SudokuCell[,] BuildCellGridFromValues(int[,] grid)
    {
        SudokuCell[,] result = new SudokuCell[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            int element = grid[i, j];
            SudokuCell cell = new(i, j, element == 0 ? null : element);
            result[i, j] = cell;
        }
        return result;
    }
}
