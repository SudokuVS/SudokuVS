namespace SudokuBattle.Sudoku.Models;

public class HiddenSudokuGrid : IDisposable
{
    readonly SudokuGrid _grid;
    readonly HiddenSudokuCell[,] _cells;

    public HiddenSudokuGrid(SudokuGrid grid)
    {
        _grid = grid;

        _cells = new HiddenSudokuCell[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            _cells[i, j] = new HiddenSudokuCell(_grid, i, j);
            _grid[i, j].ValueChanged += OnValueChanged;
            _grid[i, j].AnnotationsChanged += OnAnnotationChanged;
        }

        Regions = Enumerable.Range(0, 9).Select(i => new HiddenSudokuRegion(_cells, i)).ToList();
        Rows = Enumerable.Range(0, 9).Select(i => new HiddenSudokuRow(_cells, i)).ToList();
        Columns = Enumerable.Range(0, 9).Select(i => new HiddenSudokuColumn(_cells, i)).ToList();
    }

    public IReadOnlyList<HiddenSudokuRegion> Regions { get; set; }
    public IReadOnlyList<HiddenSudokuRow> Rows { get; set; }
    public IReadOnlyList<HiddenSudokuColumn> Columns { get; set; }

    public event EventHandler<HiddenSudokuCell>? CellValueChanged;
    public event EventHandler<HiddenSudokuCell>? CellAnnotationChanged;

    public HiddenSudokuCell this[int rowIndex, int colIndex] {
        get {
            if (rowIndex is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, "Expected row index to be between 0 and 8.");
            }

            if (colIndex is < 0 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(colIndex), colIndex, "Expected col index to be between 0 and 8.");
            }

            return _cells[rowIndex, colIndex];
        }
    }

    public IEnumerable<(int Row, int Column, HiddenSudokuCell Cell)> Enumerate()
    {
        for (int i = 0; i < Rows.Count; i++)
        for (int j = 0; j < Columns.Count; j++)
        {
            yield return (i, j, this[i, j]);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            _grid[i, j].ValueChanged -= OnValueChanged;
        }
    }

    void OnValueChanged(object? source, EventArgs _)
    {
        if (source is not SudokuCell cell)
        {
            return;
        }

        CellValueChanged?.Invoke(this, _cells[cell.Row, cell.Column]);
    }

    void OnAnnotationChanged(object? source, EventArgs _)
    {
        if (source is not SudokuCell cell)
        {
            return;
        }

        CellAnnotationChanged?.Invoke(this, _cells[cell.Row, cell.Column]);
    }
}
