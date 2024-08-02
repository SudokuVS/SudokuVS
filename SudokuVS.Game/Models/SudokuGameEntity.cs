using System.ComponentModel.DataAnnotations;

namespace SudokuVS.Game.Models;

public class SudokuGameEntity
{
    public SudokuGameEntity(string name, string initialGrid, string solvedGrid, SudokuGameOptionsEntity? options = null)
    {
        Name = name;
        InitialGrid = initialGrid;
        SolvedGrid = solvedGrid;
        Options = options ?? new SudokuGameOptionsEntity();
    }

    /// <summary>
    ///     The unique ID of the game
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    ///     The name of the game.
    /// </summary>
    [MaxLength(64)]
    public string Name { get; private set; }

    /// <summary>
    ///     The grid generated for the game.
    /// </summary>
    [MaxLength(81)]
    public string InitialGrid { get; private set; }

    /// <summary>
    ///     The solution to the game.
    /// </summary>
    [MaxLength(81)]
    public string SolvedGrid { get; private set; }

    /// <summary>
    ///     The options of the game.
    /// </summary>
    public SudokuGameOptionsEntity Options { get; private set; }

    /// <summary>
    ///     The first player of the game.
    /// </summary>
    public PlayerStateEntity? Player1 { get; set; }

    /// <summary>
    ///     The second player of the game.
    /// </summary>
    public PlayerStateEntity? Player2 { get; set; }

    /// <summary>
    ///     The date at which the game started. The game starts once both players have joined the game.
    /// </summary>
    public DateTime? StartDate { get; private set; }

    /// <summary>
    ///     The date at which the game ended. The game ends once one of them finds the solution to the sudoku puzzle.
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    ///     If the game is over, the winner of the game. The winner of the game is the first player that found the solution to the sudoku puzzle.
    /// </summary>
    public PlayerSide? Winner { get; private set; }
}

public static class SudokuGameEntityExtensions
{
    public static bool IsStarted(this SudokuGameEntity game) => game.StartDate != null;
    public static bool IsOver(this SudokuGameEntity game) => game.EndDate != null;

    public static PlayerStateEntity? GetPlayerState(this SudokuGameEntity game, PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => game.Player1,
            PlayerSide.Player2 => game.Player2,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
