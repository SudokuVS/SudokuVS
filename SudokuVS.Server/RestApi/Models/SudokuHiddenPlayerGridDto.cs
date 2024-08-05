using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     Grid as seen by a player.
/// </summary>
public class SudokuHiddenPlayerGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    [Required]
    public required Dictionary<int, SudokuHiddenPlayerCellDto> Cells { get; init; }
}

static class SudokuHiddenPlayerGridMappingExtensions
{
    public static SudokuHiddenPlayerGridDto ToHiddenPlayerGridDto(this IHiddenSudokuGrid grid, IHiddenPlayerState state) =>
        new()
        {
            Cells = grid.Enumerate()
                .Where(c => !c.IsEmpty || c.HasAnnotations || c.IsLocked || state.Hints.Contains((c.Row, c.Column)))
                .ToDictionary(c => c.GetFlatIndex(), c => c.ToHiddenPlayerCellDto(state))
        };
}
