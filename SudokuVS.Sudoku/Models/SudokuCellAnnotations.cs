using System.Collections;

namespace SudokuVS.Sudoku.Models;

public class SudokuCellAnnotations : ICollection<int>
{
    readonly HashSet<int> _collectionImplementation = [];

    public SudokuCellAnnotations(SudokuCell cell)
    {
        Cell = cell;
    }

    public SudokuCell Cell { get; }
    public int Count => _collectionImplementation.Count;
    public bool IsReadOnly => false;

    public event EventHandler? CollectionChanged;

    public void Add(int item)
    {
        AssertUnlocked();
        _collectionImplementation.Add(item);

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool Remove(int item)
    {
        AssertUnlocked();
        bool result = _collectionImplementation.Remove(item);

        CollectionChanged?.Invoke(this, EventArgs.Empty);

        return result;
    }

    public void Clear()
    {
        AssertUnlocked();
        _collectionImplementation.Clear();

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool Contains(int item) => _collectionImplementation.Contains(item);
    public void CopyTo(int[] array, int arrayIndex) => _collectionImplementation.CopyTo(array, arrayIndex);

    public IEnumerator<int> GetEnumerator() => _collectionImplementation.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collectionImplementation).GetEnumerator();

    void AssertUnlocked()
    {
        if (Cell.Locked)
        {
            throw new InvalidOperationException("Cell is locked");
        }
    }
}
