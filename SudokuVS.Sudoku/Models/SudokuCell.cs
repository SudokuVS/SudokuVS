using System.Diagnostics.CodeAnalysis;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Sudoku.Models;

public class SudokuCell : IReadOnlySudokuCell, IHiddenSudokuCell
{
    int? _element;
    readonly SudokuCellAnnotations _annotations;
    bool _isLocked;

    public SudokuCell(int row, int column, int? element = null)
    {
        if (row is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(row), row, "Expected index of row to be between 0 and 8.");
        }

        if (column is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(column), column, "Expected index of column to be between 0 and 8.");
        }

        if (element is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(element), element, "Expected element be between 1 and 9.");
        }

        Row = row;
        Column = column;
        Region = GetRegionIndex(row, column);
        Element = element == 0 ? null : element;

        _annotations = new SudokuCellAnnotations(this);
        _annotations.CollectionChanged += (_, _) => AnnotationsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int Row { get; }
    public int Column { get; }
    public int Region { get; }
    public int? Element {
        get => _element;

        set {
            if (value is < 1 or > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Expected element to be between 1 and 9.");
            }

            if (IsLocked)
            {
                throw new InvalidOperationException("Cell is locked");
            }

            if (_element == value)
            {
                return;
            }

            _element = value;
            ElementChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public ICollection<int> Annotations => _annotations;
    IReadOnlyCollection<int> IReadOnlySudokuCell.Annotations => _annotations;

    public bool IsLocked {
        get => _isLocked;

        set {
            if (_isLocked == value)
            {
                return;
            }

            _isLocked = value;
            LockChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [MemberNotNullWhen(false, nameof(Element))]
    public bool IsEmpty => Element == null;

    public bool HasAnnotations => Annotations.Count != 0;

    public event EventHandler? ElementChanged;
    public event EventHandler? AnnotationsChanged;
    public event EventHandler? LockChanged;

    public static SudokuCell Clone(SudokuCell cell)
    {
        SudokuCell result = new(cell.Row, cell.Column, cell.Element);

        foreach (int annotation in cell.Annotations)
        {
            result.Annotations.Add(annotation);
        }

        result.IsLocked = cell.IsLocked;

        return result;
    }

    static int GetRegionIndex(int row, int column) => row / 3 * 3 + column / 3;

    public override string ToString() => $"Cell[{Row}, {Column}]";
}
