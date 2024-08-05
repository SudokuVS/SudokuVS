using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.Game.Users;
using SudokuVS.Server.Controllers.Games.Requests;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Models;
using SudokuVS.Server.Services;

namespace SudokuVS.Server.Controllers.Games;

/// <summary>
///     Gameplay
/// </summary>
[Route("/api/games")]
[OpenApiTag("Gameplay")]
[Authorize]
[ApiController]
public class GameplayController : ControllerBase
{
    readonly ISudokuGamesRepository _repository;
    readonly GameplayService _gameplayService;

    /// <summary>
    /// </summary>
    public GameplayController(ISudokuGamesRepository repository, GameplayService gameplayService)
    {
        _repository = repository;
        _gameplayService = gameplayService;
    }

    /// <summary>
    ///     Create game
    /// </summary>
    [HttpPost]
    public async Task<SudokuPlayerGameDto> CreateGameAsync(CreateGameRequest request)
    {
        UserIdentity user = ControllerContext.HttpContext.User.GetUserIdentity() ?? throw new AccessDeniedException();

        SudokuGame game = await _gameplayService.CreateGameAsync(request.Name, new SudokuGameOptions { MaxHints = request.Hints }, user);
        PlayerState? playerState = game.GetPlayerState(user.ExternalId);
        if (playerState == null)
        {
            throw new InternalErrorException("Player has not joined game");
        }

        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Get game
    /// </summary>
    [HttpGet("{gameId:guid}")]
    public async Task<SudokuPlayerGameDto> GetGameAsync(Guid gameId, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Join game
    /// </summary>
    [HttpPost("{gameId:guid}")]
    public async Task<SudokuPlayerGameDto> JoinGameAsync(Guid gameId, SudokuGamePlayerSideDto side, CancellationToken cancellationToken)
    {
        UserIdentity user = ControllerContext.HttpContext.User.GetUserIdentity() ?? throw new AccessDeniedException();
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);

        game.Join(user, side.FromDto());

        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Set cell element
    /// </summary>
    [HttpPut("{gameId:guid}/cell/{cellIndex:int}/element/{element:int}")]
    public async Task<SudokuPlayerGameDto> SetElementAsync(Guid gameId, int cellIndex, int element, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);
        playerState.SetElement(cellIndex, element);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Clear cell element
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/element")]
    public async Task<SudokuPlayerGameDto> ClearElementAsync(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Add cell annotation
    /// </summary>
    [HttpPut("{gameId:guid}/cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> AddAnnotationAsync(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Remove cell annotation
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> RemoveAnnotationAsync(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Clear cell annotation
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/annotations")]
    public async Task<SudokuPlayerGameDto> ClearAnnotationsAsync(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);
        playerState.ClearAnnotations(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    /// <summary>
    ///     Use hint
    /// </summary>
    [HttpPost("{gameId:guid}/cell/{cellIndex:int}/hint")]
    public async Task<SudokuPlayerGameDto> UseHintAsync(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerStateAsync(gameId, cancellationToken);

        if (!playerState.TryUseHint(cellIndex))
        {
            throw new BadRequestException();
        }

        return game.ToPlayerGameDto(playerState);
    }

    async Task<(SudokuGame game, PlayerState playerState)> GetGameAndPlayerStateAsync(Guid gameId, CancellationToken cancellationToken)
    {
        string user = ControllerContext.RequireAuthenticatedUserId();
        SudokuGame game = await _repository.GetAsync(gameId, cancellationToken) ?? throw new NotFoundException();
        PlayerState playerState = game.GetPlayerState(user) ?? throw new NotFoundException();
        return (game, playerState);
    }
}
