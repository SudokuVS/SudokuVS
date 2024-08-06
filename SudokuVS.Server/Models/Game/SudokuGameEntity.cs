using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Server.Models.Game;

public class SudokuGameEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // EF ctor
    public SudokuGameEntity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public SudokuGameEntity(Guid id, string name, string initialGrid, string solvedGrid, SudokuGameOptionsEntity? options = null) : this()
    {
        Id = id;
        Name = name;
        InitialGrid = initialGrid;
        SolvedGrid = solvedGrid;
        Options = options ?? new SudokuGameOptionsEntity();
    }

    /// <summary>
    ///     The unique ID of the game
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary>
    ///     The name of the game.
    /// </summary>
    [MaxLength(64)]
    public string Name { get; set; }

    /// <summary>
    ///     The grid generated for the game.
    /// </summary>
    [MaxLength(SudokuGridStringSerializer.SerializedStringMaxLength)]
    public string InitialGrid { get; set; }

    /// <summary>
    ///     The solution to the game.
    /// </summary>
    [MaxLength(SudokuGridStringSerializer.SerializedStringMaxLength)]
    public string SolvedGrid { get; set; }

    /// <summary>
    ///     The options of the game.
    /// </summary>
    public SudokuGameOptionsEntity Options { get; private set; }

    /// <summary>
    ///     The players of the game.
    /// </summary>
    public ICollection<PlayerStateEntity> Players { get; private set; } = new HashSet<PlayerStateEntity>();

    /// <summary>
    ///     The date at which the game started. The game starts once both players have joined the game.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    ///     The date at which the game ended. The game ends once one of them finds the solution to the sudoku puzzle.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    ///     If the game is over, the winner of the game. The winner of the game is the first player that found the solution to the sudoku puzzle.
    /// </summary>
    public PlayerSide? Winner { get; set; }
}

public static class SudokuGameEntityExtensions
{
    public static bool IsStarted(this SudokuGameEntity game) => game.StartDate != null;
    public static bool IsOver(this SudokuGameEntity game) => game.EndDate != null;

    public static PlayerStateEntity? GetPlayerState(this SudokuGameEntity game, PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => game.Players.SingleOrDefault(p => p.Side == PlayerSide.Player1),
            PlayerSide.Player2 => game.Players.SingleOrDefault(p => p.Side == PlayerSide.Player2),
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
