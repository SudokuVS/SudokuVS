using System.Diagnostics.CodeAnalysis;
using SudokuBattle.App.Models.Game;
using SudokuBattle.Sudoku.Models;

namespace SudokuBattle.App.Services;

public class SudokuGame
{
    OtherPlayerState? _otherPlayer1Cached;
    OtherPlayerState? _otherPlayer2Cached;

    public SudokuGame(SudokuGrid initialGrid)
    {
        InitialGrid = initialGrid;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public SudokuGrid InitialGrid { get; }
    public PlayerState? Player1 { get; private set; }
    public PlayerState? Player2 { get; private set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PlayerSide? Winner { get; set; }

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
        OtherPlayerState otherPlayerState = new(newState.Grid, side, name);

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
            PlayerSide.Player1 => _otherPlayer2Cached,
            PlayerSide.Player2 => _otherPlayer1Cached,
            _ => null
        };

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
}
