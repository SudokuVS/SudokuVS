using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SudokuVS.Game.Models;
using SudokuVS.Game.Models.Users;
using SudokuVS.Game.Users;
using SudokuVS.Game.Utils;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;
using SudokuVS.Sudoku.Utils;

namespace SudokuVS.Game.Persistence;

class SudokuGamesInDbContext : SudokuGameCachedRepository
{
    readonly IServiceScopeFactory _scopeFactory;
    readonly SudokuGridStringSerializer _serializer;

    public SudokuGamesInDbContext(IServiceScopeFactory scopeFactory, SudokuGridStringSerializer serializer, ILogger<SudokuGamesInDbContext> logger) : base(logger)
    {
        _scopeFactory = scopeFactory;
        _serializer = serializer;
    }

    protected override IAsyncEnumerable<Guid> ListIdsAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return context.Games.Select(g => g.Id).AsAsyncEnumerable();
    }

    protected override async Task<bool> ExistsInDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.Games.AnyAsync(g => g.Id == id, cancellationToken);
    }

    protected override async Task<SudokuGame?> LoadFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        SudokuGameEntity entity = await context.Games.RequireAsync(id, cancellationToken);

        SudokuGrid initialGrid = _serializer.FromString(entity.InitialGrid);
        initialGrid.LockNonEmptyCells();

        SudokuGrid solvedGrid = _serializer.FromString(entity.SolvedGrid);
        solvedGrid.CopyLocksFrom(initialGrid);

        SudokuGameOptions options = new() { MaxHints = entity.Options.MaxHints };

        SudokuGame game = SudokuGame.Load(entity.Id, entity.Name, initialGrid, solvedGrid, options);

        PlayerStateEntity? player1StateEntity = entity.GetPlayerState(PlayerSide.Player1);
        PlayerState? player1State = player1StateEntity == null ? null : LoadPlayerState(game, PlayerSide.Player1, player1StateEntity);
        PlayerStateEntity? player2StateEntity = entity.GetPlayerState(PlayerSide.Player2);
        PlayerState? player2State = player2StateEntity == null ? null : LoadPlayerState(game, PlayerSide.Player2, player2StateEntity);

        game.Restore(player1State, player2State, entity.StartDate, entity.EndDate, entity.Winner);

        return game;
    }

    protected override async Task SaveToDistributedRepositoryAsync(SudokuGame game, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        SudokuGameEntity? entity = await context.Games.FindAsync([game.Id], cancellationToken);
        if (entity == null)
        {
            entity = new SudokuGameEntity(game.Id, game.Name, _serializer.ToString(game.InitialGrid), _serializer.ToString(game.SolvedGrid));
            await context.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Name = game.Name;
            entity.InitialGrid = _serializer.ToString(game.InitialGrid);
            entity.SolvedGrid = _serializer.ToString(game.SolvedGrid);
        }

        await SavePlayerState(context, game, PlayerSide.Player1, entity, cancellationToken);
        await SavePlayerState(context, game, PlayerSide.Player2, entity, cancellationToken);

        entity.StartDate = game.StartDate;
        entity.EndDate = game.EndDate;
        entity.Winner = game.Winner;

        await context.SaveChangesAsync(cancellationToken);
    }

    protected override async Task DeleteFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        SudokuGameEntity entity = await context.Games.RequireAsync(id, cancellationToken);
        context.Games.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    PlayerState LoadPlayerState(SudokuGame game, PlayerSide side, PlayerStateEntity state)
    {
        SudokuGrid grid = _serializer.FromString(state.Grid);
        grid.CopyLocksFrom(game.InitialGrid);

        (int Row, int Column)[] hints = state.Hints.Split(",")
            .Select(s => int.TryParse(s, out int i) ? i : -1)
            .Where(i => i >= 0)
            .Select(SudokuGridCoordinates.ComputeCoordinates)
            .ToArray();
        foreach ((int row, int column) in hints)
        {
            grid[row, column].IsLocked = true;
        }

        UserIdentity user = new() { ExternalId = state.User.Id, Name = state.User.Name };

        PlayerState result = new(game, grid, side, user);
        result.Restore(hints);
        return result;
    }

    async Task SavePlayerState(AppDbContext context, SudokuGame game, PlayerSide side, SudokuGameEntity entity, CancellationToken cancellationToken)
    {
        PlayerState? state = game.GetPlayerState(side);
        if (state == null)
        {
            PlayerStateEntity? entityState = entity.GetPlayerState(side);
            if (entityState != null)
            {
                context.Remove(entityState);
            }
        }
        else
        {
            PlayerStateEntity? entityState = entity.GetPlayerState(side);

            UserIdentityEntity user = await context.Users.GetOrCreateAsync(state.User, cancellationToken);
            string grid = _serializer.ToString(state.Grid);
            string hints = string.Join(",", state.Hints.Select(x => SudokuGridCoordinates.ComputeFlatIndex(x.Row, x.Column)));

            if (entityState == null)
            {
                entityState = new PlayerStateEntity(entity, side, user, grid) { Hints = hints };
                entity.Players.Add(entityState);
            }
            else
            {
                entityState.User = user;
                entityState.Grid = grid;
                entityState.Hints = hints;
            }
        }
    }
}
