using System.ComponentModel.DataAnnotations;
using SudokuVS.Game.Models.Users;

namespace SudokuVS.Game.Models;

public class PlayerStateEntity
{
    public PlayerStateEntity() { }

    public PlayerStateEntity(SudokuGameEntity game, PlayerSide side, UserIdentityEntity user, string grid)
    {
        Game = game;
        Side = side;
        User = user;
        Grid = grid;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    ///     The game associated with this state.
    /// </summary>
    public SudokuGameEntity Game { get; private set; }

    /// <summary>
    ///     The side of the player.
    /// </summary>
    public PlayerSide Side { get; set; }

    /// <summary>
    ///     The user associated with the state.
    /// </summary>
    public UserIdentityEntity User { get; set; }

    /// <summary>
    ///     The current grid of the player.
    /// </summary>
    [MaxLength(81)]
    public string Grid { get; set; }

    /// <summary>
    ///     The cells of the grid that have been obtained using hints, as a comma-separated list of indices.
    /// </summary>
    [MaxLength(256)]
    public string Hints { get; set; } = "";
}
