using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SudokuVS.Game;
using SudokuVS.Game.Persistence;
using SudokuVS.WebApi.Exceptions;
using SudokuVS.WebApi.Models;

namespace SudokuVS.WebApi.Controllers.Games;

[Route("/api/games/{gameId:guid}")]
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

    [HttpGet]
    public async Task<SudokuPlayerGameDto> GetGame(Guid gameId, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        return game.ToPlayerGameDto(playerState);
    }

    [HttpPut("cell/{cellIndex:int}/element/{element:int}")]
    public async Task<SudokuPlayerGameDto> SetElement(Guid gameId, int cellIndex, int element, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.SetElement(cellIndex, element);
        return game.ToPlayerGameDto(playerState);
    }

    [HttpDelete("cell/{cellIndex:int}/element")]
    public async Task<SudokuPlayerGameDto> ClearElement(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    [HttpPut("cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> AddAnnotation(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    [HttpDelete("cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> RemoveAnnotation(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearElement(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    [HttpDelete("cell/{cellIndex:int}/annotations")]
    public async Task<SudokuPlayerGameDto> ClearAnnotations(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);
        playerState.ClearAnnotations(cellIndex);
        return game.ToPlayerGameDto(playerState);
    }

    [HttpPost("cell/{cellIndex:int}/hint")]
    public async Task<SudokuPlayerGameDto> UseHint(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        (SudokuGame game, PlayerState playerState) = await GetGameAndPlayerState(gameId, cancellationToken);

        if (!playerState.TryUseHint(cellIndex))
        {
            throw new BadRequest();
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
