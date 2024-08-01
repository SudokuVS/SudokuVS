using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Grid as seen by a player.
/// </summary>
public class SudokuHiddenPlayerGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    public required Dictionary<int, SudokuHiddenPlayerCellDto> Cells { get; init; }
}

public static class SudokuHiddenPlayerGridMappingExtensions
{
    public static SudokuHiddenPlayerGridDto ToHiddenPlayerGridDto(this IHiddenSudokuGrid grid, IHiddenPlayerState state) =>
        new()
        {
            Cells = grid.Enumerate()
                .Where(c => !c.Cell.IsEmpty || c.Cell.HasAnnotations || c.Cell.IsLocked || state.Hints.Contains((c.Row, c.Column)))
                .ToDictionary(c => c.Cell.GetFlatIndex(), c => c.Cell.ToHiddenPlayerCellDto(state))
        };
}
