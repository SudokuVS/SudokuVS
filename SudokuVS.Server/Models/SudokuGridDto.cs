using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Server.Models;

/// <summary>
///     Sparse representation of a sudoku grid.
/// </summary>
public class SudokuGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    [Required]
    public required Dictionary<int, SudokuCellDto> Cells { get; init; }
}

static class SudokuGridMappingExtensions
{
    public static SudokuGridDto ToDto(this IReadOnlySudokuGrid grid, PlayerState state) =>
        new()
        {
            Cells = grid.Enumerate()
                .Where(c => !c.IsEmpty || c.HasAnnotations || c.IsLocked || state.Hints.Contains((c.Row, c.Column)))
                .ToDictionary(c => c.GetFlatIndex(), c => c.ToDto())
        };
}
