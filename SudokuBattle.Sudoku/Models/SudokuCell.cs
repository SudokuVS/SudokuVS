using System.Diagnostics.CodeAnalysis;
using SudokuBattle.Sudoku.Models.Abstractions;

namespace SudokuBattle.Sudoku.Models;

public class SudokuCell : IReadOnlySudokuCell, IHiddenSudokuCell
{
    int? _element;
    readonly SudokuCellAnnotations _annotations;
    bool _locked;

    public SudokuCell(int row, int column, int? element = null)
    {
        if (Row is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(Row), Row, "Expected index of row to be between 0 and 8.");
        }

        if (Column is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(Column), Column, "Expected index of column to be between 0 and 8.");
        }

        if (Region is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(Region), Region, "Expected index of region to be between 0 and 8.");
        }

        if (Element is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(Column), Column, "Expected element be between 1 and 9.");
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

            if (Locked)
            {
                throw new InvalidOperationException("Cell is locked");
            }

            _element = value;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public ICollection<int> Annotations => _annotations;
    public bool Locked {
        get => _locked;

        set {
            _locked = value;
            LockChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [MemberNotNullWhen(false, nameof(Element))]
    public bool Empty => Element == null;

    public bool HasAnnotations => Annotations.Count != 0;

    public event EventHandler? ValueChanged;
    public event EventHandler? AnnotationsChanged;
    public event EventHandler? LockChanged;

    public static SudokuCell Clone(SudokuCell cell) =>
        new(cell.Row, cell.Column, cell.Element)
        {
            Locked = cell.Locked
        };

    static int GetRegionIndex(int row, int column) => row / 3 * 3 + column / 3;
}
