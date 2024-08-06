using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Server.Models.Game;

public class PlayerStateEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // EF ctor
    public PlayerStateEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public PlayerStateEntity(SudokuGameEntity game, PlayerSide side, string username, string grid)
    {
        GameId = game.Id;
        Game = game;
        Side = side;
        Username = username;
        Grid = grid;
    }

    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    ///     The unique ID of the game associated with this state.
    /// </summary>
    public Guid GameId { get; private set; }

    /// <summary>
    ///     The game associated with this state.
    /// </summary>
    public SudokuGameEntity Game { get; private set; }

    /// <summary>
    ///     The side of the player.
    /// </summary>
    public PlayerSide Side { get; private set; }

    /// <summary>
    ///     The user associated with the state.
    /// </summary>
    [MaxLength(64)]
    public string Username { get; set; }

    /// <summary>
    ///     The current grid of the player.
    /// </summary>
    [MaxLength(SudokuGridStringSerializer.SerializedStringMaxLength)]
    public string Grid { get; set; }

    /// <summary>
    ///     The cells of the grid that have been obtained using hints, as a comma-separated list of indices.
    /// </summary>
    [MaxLength(256)]
    public string Hints { get; set; } = "";
}
