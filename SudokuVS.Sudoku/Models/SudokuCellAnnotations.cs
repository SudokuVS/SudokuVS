using System.Collections;

namespace SudokuVS.Sudoku.Models;

/// <summary>
///     ICollection&lt;int&gt; implementation that is
///     <list type="number">
///         <item>observable (see <see cref="CollectionChanged" />)</item>
///         <item>locked when the parent <see cref="SudokuCell" /> is locked</item>
///     </list>
/// </summary>
class SudokuCellAnnotations : ICollection<int>, IReadOnlyCollection<int>
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

        if (!_collectionImplementation.Add(item))
        {
            return;
        }

        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool Remove(int item)
    {
        AssertUnlocked();

        if (!_collectionImplementation.Remove(item))
        {
            return false;
        }

        CollectionChanged?.Invoke(this, EventArgs.Empty);
        return true;

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
        if (Cell.IsLocked)
        {
            throw new InvalidOperationException("Cell is locked");
        }
    }
}
