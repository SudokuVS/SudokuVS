using SudokuVS.Game.Abstractions;
using SudokuVS.Game.Exceptions;

namespace SudokuVS.Game.Services;

public class GameplayService
{
    readonly ISudokuGamesRepository _repository;

    public GameplayService(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    public async Task<SudokuGame> StartNewGameAsync(string? gameName, SudokuGameOptions gameOptions, string creatorUser, CancellationToken cancellationToken = default)
    {
        SudokuGame? game = SudokuGame.Create(gameName, gameOptions);
        if (game == null)
        {
            throw new DomainException("Failed to create game.");
        }

        game.Join(creatorUser, PlayerSide.Player1);
        await _repository.SaveAsync(game, cancellationToken);

        return game;
    }

    public async Task<SudokuGame> JoinGameAsync(Guid gameId, string user, CancellationToken cancellationToken = default)
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
            throw new DomainException("Game full");
        }

        return game;
    }

    public async Task<SudokuGame> SetElementAsync(Guid gameId, string user, int cellIndex, int element, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, user, cancellationToken);
        playerState.SetElement(cellIndex, element);
        return game;
    }

    public async Task<SudokuGame> ClearElementAsync(Guid gameId, string user, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, user, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game;
    }

    public async Task<SudokuGame> AddAnnotationAsync(Guid gameId, string user, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, user, cancellationToken);
        playerState.AddAnnotation(cellIndex, annotation);
        return game;
    }

    public async Task<SudokuGame> RemoveAnnotationAsync(Guid gameId, string user, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, user, cancellationToken);
        playerState.RemoveAnnotation(cellIndex, annotation);
        return game;
    }

    public async Task<SudokuGame> ClearAnnotationsAsync(Guid gameId, string user, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, user, cancellationToken);
        playerState.ClearAnnotations(cellIndex);
        return game;
    }

    public async Task<SudokuGame> UseHintAsync(Guid gameId, string user, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, user, cancellationToken);

        if (!playerState.TryUseHint(cellIndex, out string? whyNot))
        {
            throw new DomainException(whyNot);
        }

        return game;
    }

    async Task<(SudokuGame game, PlayerState playerState)> GetGameAndPlayerState(Guid gameId, string user, CancellationToken cancellationToken)
    {
        SudokuGame game = await _repository.RequireAsync(gameId, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new DomainException("Player is not part of the game");

        return (game, playerState);
    }
}
