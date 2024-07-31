using System.Diagnostics.CodeAnalysis;
using SudokuBattle.Sudoku.Models;
using SudokuBattle.Sudoku.Solvers;

namespace SudokuBattle.Game;

public class SudokuGame
{
    OtherPlayerState? _otherPlayer1Cached;
    OtherPlayerState? _otherPlayer2Cached;

    SudokuGame(Guid id, SudokuGrid initialGrid, SudokuGrid solvedGrid, SudokuGameOptions? options = null)
    {
        Id = id;
        InitialGrid = initialGrid;
        SolvedGrid = solvedGrid;
        Options = options ?? new SudokuGameOptions();
    }

    public Guid Id { get; }
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

    public PlayerState Join(string name, PlayerSide side)
    {
        PlayerState? existing = GetPlayerState(side);
        if (existing != null)
        {
            throw new InvalidOperationException($"{side.Format()} has already joined");
        }

        SudokuGrid grid = SudokuGrid.Clone(InitialGrid);
        grid.CellValueChanged += (_, _) => OnCellValueChanged(side);

        PlayerState newState = new(this, grid, side, name);
        OtherPlayerState otherPlayerState = new(newState);

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

    public OtherPlayerState? GetOtherPlayerState(PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => Player2 == null ? null : _otherPlayer2Cached ??= new OtherPlayerState(Player2),
            PlayerSide.Player2 => Player1 == null ? null : _otherPlayer1Cached ??= new OtherPlayerState(Player1),
            _ => null
        };

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

    public static SudokuGame Create(SudokuGrid grid)
    {
        SudokuLibSolver solver = new();
        SudokuGrid? solvedGrid = solver.Solve(grid);
        if (solvedGrid == null)
        {
            throw new InvalidOperationException("Could not solve grid");
        }

        solvedGrid.LockNonEmptyCells();

        return new SudokuGame(Guid.NewGuid(), grid, solvedGrid);
    }

    internal static SudokuGame Load(Guid id, SudokuGrid grid, SudokuGrid solvedGrid, SudokuGameOptions options) => new(id, grid, solvedGrid, options);
}
