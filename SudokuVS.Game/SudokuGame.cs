using System.Diagnostics.CodeAnalysis;
using SudokuVS.Game.Models;
using SudokuVS.Sudoku.Generators;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Solvers;

namespace SudokuVS.Game;

public class SudokuGame
{
    HiddenPlayerState? _otherPlayer1Cached;
    HiddenPlayerState? _otherPlayer2Cached;

    SudokuGame(Guid id, string name, SudokuGrid initialGrid, SudokuGrid solvedGrid, SudokuGameOptions? options = null)
    {
        Id = id;
        Name = name;
        InitialGrid = initialGrid;
        SolvedGrid = solvedGrid;
        Options = options ?? new SudokuGameOptions();
    }

    public Guid Id { get; }
    public string Name { get; }
    public SudokuGrid InitialGrid { get; }
    public SudokuGameOptions Options { get; }
    public SudokuGrid SolvedGrid { get; }
    public PlayerState? Player1 { get; private set; }
    public PlayerState? Player2 { get; private set; }

    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public PlayerSide? Winner { get; private set; }

    [MemberNotNullWhen(true, nameof(StartDate))]
    public bool IsStarted => StartDate.HasValue;

    [MemberNotNullWhen(true, nameof(Winner), nameof(StartDate), nameof(EndDate))]
    public bool IsOver => Winner != null;

    public event EventHandler<PlayerSide>? PlayerJoined;
    public event EventHandler<PlayerSide>? GameOver;

    public PlayerState Join(string username, PlayerSide side)
    {
        PlayerState? existing = GetPlayerState(side);
        if (existing != null)
        {
            throw new InvalidOperationException($"{side.Format()} has already joined");
        }

        SudokuGrid grid = SudokuGrid.Clone(InitialGrid);
        grid.CellElementChanged += (_, _) => OnCellValueChanged(side);

        PlayerState newState = new(this, grid, side, username);
        HiddenPlayerState otherPlayerState = new(newState);

        switch (side)
        {
            case PlayerSide.Player1:
                Player1 = newState;
                _otherPlayer1Cached = otherPlayerState;
                break;
            case PlayerSide.Player2:
                Player2 = newState;
                _otherPlayer2Cached = otherPlayerState;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }

        if (Player1 != null && Player2 != null)
        {
            StartDate = DateTime.Now;
        }

        PlayerJoined?.Invoke(this, side);

        return newState;
    }

    public PlayerState? GetPlayerState(PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => Player1,
            PlayerSide.Player2 => Player2,
            _ => null
        };

    public IHiddenPlayerState? GetHiddenPlayerState(PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => Player1 == null ? null : _otherPlayer1Cached ??= new HiddenPlayerState(Player1),
            PlayerSide.Player2 => Player2 == null ? null : _otherPlayer2Cached ??= new HiddenPlayerState(Player2),
            _ => null
        };

    public PlayerState? GetPlayerState(string username)
    {
        if (Player1 != null && Player1.Username == username)
        {
            return Player1;
        }

        if (Player2 != null && Player2.Username == username)
        {
            return Player2;
        }

        return null;
    }

    public IHiddenPlayerState? GetOtherPlayerState(string username)
    {
        if (Player1 != null && Player1.Username == username)
        {
            return GetHiddenPlayerState(PlayerSide.Player2);
        }

        if (Player2 != null && Player2.Username == username)
        {
            return GetHiddenPlayerState(PlayerSide.Player1);
        }

        return null;
    }

    internal void Restore(PlayerState? player1, PlayerState? player2, DateTime? startDate, DateTime? endDate, PlayerSide? winner)
    {
        Player1 = player1;
        Player2 = player2;
        StartDate = startDate;
        EndDate = endDate;
        Winner = winner;
    }

    void OnCellValueChanged(PlayerSide side)
    {
        if (IsOver)
        {
            return;
        }

        SudokuGrid? grid = GetPlayerState(side)?.Grid;
        if (grid is not { IsCompleted: true, IsValid: true })
        {
            return;
        }

        Winner = side;
        EndDate = DateTime.Now;
        GameOver?.Invoke(this, side);
    }

    public static SudokuGame? Create(SudokuGameOptions? options = null)
    {
        Guid id = Guid.NewGuid();
        return Create(id, id.ToString(), options);
    }

    public static SudokuGame? Create(string? name, SudokuGameOptions? options = null)
    {
        Guid id = Guid.NewGuid();
        return Create(id, name, options);
    }

    static SudokuGame? Create(Guid id, string? name, SudokuGameOptions? options = null)
    {
        SudokuLibGenerator generator = new();
        SudokuGrid? grid = generator.Generate();
        if (grid == null)
        {
            return null;
        }
        grid.LockNonEmptyCells();

        SudokuLibSolver solver = new();
        SudokuGrid? solvedGrid = solver.Solve(grid);
        if (solvedGrid == null)
        {
            throw new InvalidOperationException("Could not solve grid");
        }

        solvedGrid.LockNonEmptyCells();

        return new SudokuGame(id, name ?? id.ToString(), grid, solvedGrid, options);
    }

    internal static SudokuGame Load(Guid id, string name, SudokuGrid grid, SudokuGrid solvedGrid, SudokuGameOptions options) => new(id, name, grid, solvedGrid, options);
}

public static class SudokuGameExtensions
{
    public static bool InvolvesPlayer(this SudokuGame game, string username) =>
        game.Player1 != null && game.Player1.Username == username || game.Player2 != null && game.Player2.Username == username;
}
