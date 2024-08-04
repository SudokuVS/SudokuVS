using SudokuVS.Game;
using SudokuVS.Game.Models;
using SudokuVS.Game.Persistence;
using SudokuVS.Game.Users;
using SudokuVS.WebApi.Exceptions;

namespace SudokuVS.WebApi.Services;

public class GameplayService
{
    readonly ISudokuGamesRepository _repository;

    public GameplayService(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    public async Task<SudokuGame> CreateGameAsync(string? gameName, SudokuGameOptions gameOptions, UserIdentity creatorUser, CancellationToken cancellationToken = default)
    {
        SudokuGame? game = SudokuGame.Create(gameName, gameOptions);
        if (game == null)
        {
            throw new InternalErrorException("Failed to create game.");
        }

        game.Join(creatorUser, PlayerSide.Player1);
        await _repository.SaveAsync(game, cancellationToken);

        return game;
    }

    public async Task JoinGameAsync(Guid gameId, UserIdentity user, CancellationToken cancellationToken = default)
    {
        SudokuGame game = await _repository.RequireAsync(gameId, cancellationToken);

        if (game.Player1 == null)
        {
            game.Join(user, PlayerSide.Player1);
        }
        else if (game.Player2 == null)
        {
            game.Join(user, PlayerSide.Player2);
        }
        else
        {
            throw new InvalidOperationException("Game full");
        }
    }
}
