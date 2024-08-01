using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.RestApi.Models;

/// <summary>
///     Flat representation of a sudoku grid.
/// </summary>
public class SudokuSimpleGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    public required int[] Cells { get; init; }
}

public static class SudokuSimpleGridMappingExtensions
{
    public static SudokuSimpleGridDto ToSimpleDto(this SudokuGrid grid) =>
        new()
        {
            Cells = new SudokuGridEnumerableSerializer().ToEnumerable(grid).ToArray()
        };
}
