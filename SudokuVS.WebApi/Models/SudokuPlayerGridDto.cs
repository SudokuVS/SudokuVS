using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Grid as seen by a player.
/// </summary>
public class SudokuPlayerGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    [Required]
    public required Dictionary<int, SudokuPlayerCellDto> Cells { get; init; }
}

static class SudokuPlayerGridMappingExtensions
{
    public static SudokuPlayerGridDto ToPlayerGridDto(this IReadOnlySudokuGrid grid, PlayerState state) =>
        new()
        {
            Cells = grid.Enumerate()
                .Where(c => !c.Cell.IsEmpty || c.Cell.HasAnnotations || c.Cell.IsLocked || state.Hints.Contains((c.Row, c.Column)))
                .ToDictionary(c => c.Cell.GetFlatIndex(), c => c.Cell.ToPlayerCellDto(state))
        };
}
