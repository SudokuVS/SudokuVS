using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.Game.Users;
using SudokuVS.WebApi.Exceptions;
using SudokuVS.WebApi.Models;

namespace SudokuVS.WebApi.Controllers.Games;

[Route("/api/games")]
[OpenApiTag("Gameplay")]
[Authorize]
[ApiController]
public class GameplayController : ControllerBase
{
    readonly ISudokuGamesRepository _repository;

    public GameplayController(ISudokuGamesRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Create game
    /// </summary>
    [HttpPost]
    public async Task<SudokuPlayerGameDto> CreateGame(CreateGameRequest request)
    {
        UserIdentity user = ControllerContext.HttpContext.User.GetUserIdentity() ?? throw new AccessDeniedException();

        SudokuGame? game = SudokuGame.Create(
            request.Name,
            new SudokuGameOptions
            {
                MaxHints = request.Hints
            }
        );

        if (game == null)
        {
            throw new InternalErrorException("Failed to create game.");
        }

        await _repository.Save(game);
        PlayerState playerState = game.Join(user, PlayerSide.Player1);

        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Get game
    /// </summary>
    [HttpGet("{gameId:guid}")]
    public async Task<SudokuPlayerGameDto> GetGame(Guid gameId, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Join game
    /// </summary>
    [HttpPost("{gameId:guid}")]
    public async Task<SudokuPlayerGameDto> JoinGame(Guid gameId, SudokuGamePlayerSideDto side, CancellationToken cancellationToken)
    {
        UserIdentity user = ControllerContext.HttpContext.User.GetUserIdentity() ?? throw new AccessDeniedException();
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);

        game.Join(user, side.FromDto());

        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Set cell element
    /// </summary>
    [HttpPut("{gameId:guid}/cell/{cellIndex:int}/element/{element:int}")]
    public async Task<SudokuPlayerGameDto> SetElement(Guid gameId, int cellIndex, int element, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.SetElement(cellIndex, element);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Clear cell element
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/element")]
    public async Task<SudokuPlayerGameDto> ClearElement(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Add cell annotation
    /// </summary>
    [HttpPut("{gameId:guid}/cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> AddAnnotation(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Remove cell annotation
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> RemoveAnnotation(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Clear cell annotation
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/annotations")]
    public async Task<SudokuPlayerGameDto> ClearAnnotations(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearAnnotations(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Use hint
    /// </summary>
    [HttpPost("{gameId:guid}/cell/{cellIndex:int}/hint")]
    public async Task<SudokuPlayerGameDto> UseHint(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);

        if (!playerState.TryUseHint(cellIndex))
        {
            throw new BadRequestException();
        }

        return game.ToPlayerGameDto(playerState);
    }

    async Task<(SudokuGame game, PlayerState playerState)> GetGameAndPlayerState(Guid gameId, CancellationToken cancellationToken)
    {
        Guid user = ControllerContext.RequireAuthenticatedUserId();
        SudokuGame game = await _repository.Get(gameId, cancellationToken) ?? throw new NotFoundException();
        PlayerState playerState = game.GetPlayerState(user) ?? throw new NotFoundException();
        return (game, playerState);
    }
}
