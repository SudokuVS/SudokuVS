using SudokuVS.Game;
using SudokuVS.Sudoku.Models;

namespace SudokuVS.RestApi.Models;

/// <summary>
///     Sparse representation of a sudoku grid.
/// </summary>
public class SudokuGridDto
{
    /// <summary>
    ///     The cells of the grid.
    /// </summary>
    public required Dictionary<int, SudokuCellDto> Cells { get; init; }
}

public static class SudokuGridMappingExtensions
{
    public static SudokuGridDto ToDto(this SudokuGrid grid, PlayerState state) =>
        new()
        {
            Cells = grid.Enumerate()
                .Where(c => !c.Cell.IsEmpty || c.Cell.HasAnnotations || c.Cell.IsLocked || state.Hints.Contains((c.Row, c.Column)))
                .ToDictionary(c => c.Cell.GetFlatIndex(), c => c.Cell.ToDto())
        };
}
