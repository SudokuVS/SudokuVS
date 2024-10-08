﻿using Microsoft.EntityFrameworkCore;
using SudokuVS.Game;
using SudokuVS.Game.Utils;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Database;
using SudokuVS.Server.Infrastructure.Database.Models;
using SudokuVS.Server.Infrastructure.Database.Models.Game;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;
using SudokuVS.Sudoku.Utils;

namespace SudokuVS.Server.Infrastructure.Repositories;

class SudokuGamesInDbContext : SudokuGameCachedRepository
{
    readonly SudokuGridStringSerializer _serializer;
    readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SudokuGamesInDbContext(IDbContextFactory<AppDbContext> contextFactory, SudokuGridStringSerializer serializer, ILogger<SudokuGamesInDbContext> logger) : base(logger)
    {
        _contextFactory = contextFactory;
        _serializer = serializer;
    }

    protected override async IAsyncEnumerable<SudokuGame> LoadAllAsync()
    {
        await using AppDbContext context = await _contextFactory.CreateDbContextAsync();

        await foreach (SudokuGameEntity entity in context.Games.Include(g => g.Players).ThenInclude(p => p.User).AsAsyncEnumerable())
        {
            SudokuGrid initialGrid = _serializer.FromString(entity.InitialGrid);
            SudokuGrid solvedGrid = _serializer.FromString(entity.SolvedGrid);

            SudokuGameOptions options = new() { MaxHints = entity.Options.MaxHints };

            SudokuGame game = SudokuGame.Load(entity.Id, entity.Name, initialGrid, solvedGrid, options);

            PlayerStateEntity? player1StateEntity = entity.GetPlayerState(PlayerSide.Player1);
            PlayerState? player1State = player1StateEntity == null ? null : LoadPlayerState(game, PlayerSide.Player1, player1StateEntity);
            PlayerStateEntity? player2StateEntity = entity.GetPlayerState(PlayerSide.Player2);
            PlayerState? player2State = player2StateEntity == null ? null : LoadPlayerState(game, PlayerSide.Player2, player2StateEntity);

            game.Restore(player1State, player2State, entity.StartDate, entity.EndDate, entity.Winner);

            yield return game;
        }
    }

    protected override async Task SaveToDistributedRepositoryAsync(SudokuGame game, CancellationToken cancellationToken)
    {
        await using AppDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        SudokuGameEntity? entity = await FindGameAsync(context, game.Id, cancellationToken);
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

        await SavePlayerStateAsync(context, game, PlayerSide.Player1, entity, cancellationToken);
        await SavePlayerStateAsync(context, game, PlayerSide.Player2, entity, cancellationToken);

        entity.StartDate = game.StartDate;
        entity.EndDate = game.EndDate;
        entity.Winner = game.Winner;

        await context.SaveChangesAsync(cancellationToken);
    }

    protected override async Task DeleteFromDistributedRepositoryAsync(Guid id, CancellationToken cancellationToken)
    {
        await using AppDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        SudokuGameEntity entity = await context.Games.RequireAsync(id, cancellationToken);
        context.Games.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    static async Task<SudokuGameEntity?> FindGameAsync(AppDbContext context, Guid id, CancellationToken cancellationToken) =>
        await context.Games.Include(g => g.Players).SingleOrDefaultAsync(g => g.Id == id, cancellationToken);

    PlayerState LoadPlayerState(SudokuGame game, PlayerSide side, PlayerStateEntity state)
    {
        SudokuGrid grid = _serializer.FromString(state.Grid);

        (int Row, int Column)[] hints = state.Hints.Split(",")
            .Select(s => int.TryParse(s, out int i) ? i : -1)
            .Where(i => i >= 0)
            .Select(SudokuGridCoordinates.ComputeCoordinates)
            .ToArray();
        foreach ((int row, int column) in hints)
        {
            grid[row, column].IsLocked = true;
        }

        string username = state.User.GetUserName();

        PlayerState result = new(game, grid, side, username);
        result.Restore(hints);
        return result;
    }

    async Task SavePlayerStateAsync(AppDbContext context, SudokuGame game, PlayerSide side, SudokuGameEntity entity, CancellationToken _ = default)
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

            AppUser? user = await context.Users.GetByUserNameAsync(state.Username);
            if (user == null)
            {
                throw new InternalErrorException($"Could not find user with name {state.Username}");
            }

            string grid = _serializer.ToString(state.Grid);
            string hints = string.Join(",", state.Hints.Select(x => SudokuGridCoordinates.ComputeFlatIndex(x.Row, x.Column)));

            if (entityState == null)
            {
                entityState = new PlayerStateEntity(entity, side, user, grid) { Hints = hints };
                context.PlayerStates.Add(entityState);
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
