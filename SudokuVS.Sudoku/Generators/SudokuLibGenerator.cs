using Sudoku;
using SudokuVS.Sudoku.Generators.Abstractions;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Sudoku.Generators;

/// <summary>
///     Use a solver to generate a sudoku grid that has exactly one solution.
/// </summary>
public class SudokuLibGenerator : ISudokuGenerator
{
    readonly Generator _generator = new();
    readonly SudokuGridEnumerableSerializer _serializer = new();
    readonly int _clues;

    public SudokuLibGenerator(int clues = 30)
    {
        _clues = clues;
    }

    public SudokuGrid? Generate()
    {
        int[]? flattened = _generator.Generate(_clues);
        return flattened == null ? null : _serializer.FromEnumerable(flattened);
    }
}
