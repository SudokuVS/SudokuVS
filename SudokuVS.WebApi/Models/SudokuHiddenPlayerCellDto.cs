using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Cell of the opponent as seen by a player.
/// </summary>
public class SudokuHiddenPlayerCellDto
{
    /// <summary>
    ///     Does the cell have annotations.
    /// </summary>
    public bool? HasAnnotations { get; init; }

    /// <summary>
    ///     Is the cell locked.
    /// </summary>
    public bool? IsLocked { get; init; }

    /// <summary>
    ///     Is the cell a hint requested by the player.
    /// </summary>
    public bool? IsHint { get; init; }
}

static class SudokuHiddenPlayerCellMappingExtensions
{
    public static SudokuHiddenPlayerCellDto ToHiddenPlayerCellDto(this IHiddenSudokuCell cell, IHiddenPlayerState state) =>
        new()
        {
            HasAnnotations = cell.HasAnnotations ? null : cell.HasAnnotations,
            IsLocked = cell.IsLocked ? true : null,
            IsHint = state.Hints.Contains((cell.Row, cell.Column)) ? true : null
        };
}
