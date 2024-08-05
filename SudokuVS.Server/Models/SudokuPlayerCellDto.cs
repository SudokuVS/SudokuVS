using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Server.Models;

/// <summary>
///     Cell as seen by a player.
/// </summary>
public class SudokuPlayerCellDto
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

    /// <summary>
    ///     Is the cell a hint requested by the player.
    /// </summary>
    public bool? IsHint { get; init; }
}

static class SudokuPlayerCellMappingExtensions
{
    public static SudokuPlayerCellDto ToPlayerCellDto(this IReadOnlySudokuCell cell, PlayerState state) =>
        new()
        {
            Element = cell.Element,
            Annotations = cell.Annotations.Count == 0 ? null : cell.Annotations.ToArray(),
            IsLocked = cell.IsLocked ? true : null,
            IsHint = state.Hints.Contains((cell.Row, cell.Column)) ? true : null
        };
}
