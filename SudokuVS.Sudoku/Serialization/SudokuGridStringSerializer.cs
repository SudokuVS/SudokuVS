using SudokuVS.Sudoku.Models;

namespace SudokuVS.Sudoku.Serialization;

public class SudokuGridStringSerializer
{
    readonly SudokuGridEnumerableSerializer _enumerableSerializer = new();

    public string Serialize(SudokuGrid grid) => string.Join("", _enumerableSerializer.ToEnumerable(grid));

    public SudokuGrid Deserialize(string serialized)
    {
        if (serialized.Length != 81)
        {
            throw new ArgumentException("Expected string to have length 81.", nameof(serialized));
        }

        return _enumerableSerializer.FromEnumerable(
            Enumerable.Range(0, 81)
                .Select(i => int.TryParse([serialized[i]], out int element) ? element : throw new ArgumentException($"Bad character at position {i}: {serialized[i]}"))
        );
    }
}
