using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Models.Users;

namespace SudokuVS.Game.Models;

[Owned]
public class PlayerStateEntity
{
    public PlayerStateEntity(UserIdentityEntity user, string grid)
    {
        User = user;
        Grid = grid;
    }

    /// <summary>
    ///     The user associated with the state.
    /// </summary>
    public UserIdentityEntity User { get; private set; }

    /// <summary>
    ///     The current grid of the player.
    /// </summary>
    public string Grid { get; private set; }

    /// <summary>
    ///     The cells of the grid that have been obtained using hints, as a comma-separated list of indices.
    /// </summary>
    public string Hints { get; set; } = "";
}