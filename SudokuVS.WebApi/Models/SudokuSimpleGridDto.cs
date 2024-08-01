﻿using SudokuVS.Sudoku.Models.Abstractions;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.WebApi.Models;

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
    public static SudokuSimpleGridDto ToSimpleDto(this IReadOnlySudokuGrid grid) =>
        new()
        {
            Cells = new SudokuGridEnumerableSerializer().ToEnumerable(grid).ToArray()
        };
}
