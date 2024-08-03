using Microsoft.EntityFrameworkCore;

namespace SudokuVS.Game.Models;

[Owned]
public class SudokuGameOptionsEntity
{
    /// <summary>
    ///     The max number of hints that a user can use.
    /// </summary>
    public int MaxHints { get; set; } = 3;
}