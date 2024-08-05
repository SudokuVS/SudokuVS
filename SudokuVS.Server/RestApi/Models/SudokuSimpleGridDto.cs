using System.ComponentModel.DataAnnotations;
using SudokuVS.Sudoku.Models.Abstractions;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     Flat representation of a sudoku grid.
/// </summary>
public class SudokuSimpleGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    [Required]
    public required int[] Cells { get; init; }
}

static class SudokuSimpleGridMappingExtensions
{
    public static SudokuSimpleGridDto ToSimpleDto(this IReadOnlySudokuGrid grid) =>
        new()
        {
            Cells = new SudokuGridEnumerableSerializer().ToEnumerable(grid).ToArray()
        };
}
