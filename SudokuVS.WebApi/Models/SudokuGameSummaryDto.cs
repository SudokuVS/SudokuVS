using SudokuVS.Game;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Summary of a game.
/// </summary>
public class SudokuGameSummaryDto
{
    /// <summary>
    ///     The unique ID of the game.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the game.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The initial grid.
    /// </summary>
    public required SudokuSimpleGridDto Grid { get; init; }

    /// <summary>
    ///     The name of the first player.
    /// </summary>
    public required UserIdentityDto? Player1 { get; init; }

    /// <summary>
    ///     The name of the second player.
    /// </summary>
    public required UserIdentityDto? Player2 { get; init; }

    /// <summary>
    ///     Is the game started yet. The game starts once the both players have joined.
    /// </summary>
    public required bool IsStarted { get; init; }

    /// <summary>
    ///     If the game is started, the date at which it started
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    ///     Is the game over. The game ends once one of the players solve the sudoku puzzle.
    /// </summary>
    public required bool IsOver { get; init; }

    /// <summary>
    ///     If the game is over, the date at which it ended
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    ///     If the game is over, the player that won the game.
    /// </summary>
    public SudokuGamePlayerSideDto? Winner { get; set; }
}

public static class SudokuGameSummaryMappingExtensions
{
    public static SudokuGameSummaryDto ToSummaryDto(this SudokuGame game) =>
        new()
        {
            Id = game.Id,
            Name = game.Name,
            Grid = game.InitialGrid.ToSimpleDto(),
            Player1 = game.Player1?.User.ToDto(),
            Player2 = game.Player2?.User.ToDto(),
            IsStarted = game.IsStarted,
            StartDate = game.StartDate,
            IsOver = game.IsOver,
            EndDate = game.EndDate,
            Winner = game.Winner?.ToDto()
        };
}
