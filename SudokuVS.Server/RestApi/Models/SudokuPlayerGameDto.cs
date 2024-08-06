using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     State of the game as seen by one of the players
/// </summary>
public class SudokuPlayerGameDto
{
    /// <summary>
    ///     The unique ID of the game.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the game.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The game state of the current player.
    /// </summary>
    [Required]
    public required SudokuGamePlayerStateDto Player { get; init; }

    /// <summary>
    ///     The game state of the opponent.
    /// </summary>
    public SudokuGameHiddenPlayerStateDto? Opponent { get; init; }

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

static class SudokuGameMappingExtensions
{
    public static SudokuPlayerGameDto ToPlayerGameDto(this SudokuGame game, PlayerState playerState, UserIdentityDto player, UserIdentityDto? opponent) =>
        new()
        {
            Id = game.Id,
            Name = game.Name,
            Player = playerState.ToPlayerStateDto(player),
            Opponent = opponent == null ? null : game.GetOtherPlayerState(playerState.Username)?.ToHiddenPlayerStateDto(opponent),
            IsStarted = game.IsStarted,
            StartDate = game.StartDate,
            IsOver = game.IsOver,
            EndDate = game.EndDate,
            Winner = game.Winner?.ToDto()
        };
}
