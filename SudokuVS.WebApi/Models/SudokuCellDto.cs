using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Cell in a Sudoku grid.
/// </summary>
public class SudokuCellDto
{
    /// <summary>
    ///     The current value of the cell.
    /// </summary>
    public int? Element { get; init; }

    /// <summary>
    ///     The annotations of the cell.
    /// </summary>
    public IReadOnlyList<int>? Annotations { get; init; }

    /// <summary>
    ///     Is the cell locked.
    /// </summary>
    public bool? IsLocked { get; init; }
}

static class SudokuCellMappingExtensions
{
    public static SudokuCellDto ToDto(this IReadOnlySudokuCell cell) =>
        new()
        {
            Element = cell.Element,
            Annotations = cell.Annotations.Count == 0 ? null : cell.Annotations.ToArray(),
            IsLocked = cell.IsLocked ? true : null
        };
}
