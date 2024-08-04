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

    public async Task<SudokuGame> CreateGameAsync(string? gameName, SudokuGameOptions gameOptions, UserIdentity creatorUser)
    {
        SudokuGame? game = SudokuGame.Create(gameName, gameOptions);
        if (game == null)
        {
            throw new InternalErrorException("Failed to create game.");
        }

        PlayerState playerState = game.Join(creatorUser, PlayerSide.Player1);
        await _repository.SaveAsync(game);

        return game;
    }
}
