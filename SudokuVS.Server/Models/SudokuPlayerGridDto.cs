using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Server.Models;

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
                .Where(c => !c.IsEmpty || c.HasAnnotations || c.IsLocked || state.Hints.Contains((c.Row, c.Column)))
                .ToDictionary(c => c.GetFlatIndex(), c => c.ToPlayerCellDto(state))
        };
}
