using SudokuVS.Game;
using SudokuVS.Game.Models;
using SudokuVS.Server.Exceptions;

namespace SudokuVS.Server.Services;

public class GameplayService
{
    readonly ISudokuGamesRepository _repository;

    public GameplayService(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    public async Task<SudokuGame> CreateGameAsync(string? gameName, SudokuGameOptions gameOptions, string creatorUser, CancellationToken cancellationToken = default)
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

    public async Task JoinGameAsync(Guid gameId, string user, CancellationToken cancellationToken = default)
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
