using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SudokuVS.Game;
using SudokuVS.Game.Services;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Database.Models;
using SudokuVS.Server.RestApi.Controllers.Games.Requests;
using SudokuVS.Server.RestApi.Models;

namespace SudokuVS.Server.RestApi.Controllers.Games;

/// <summary>
///     Gameplay
/// </summary>
[Route("/api/games")]
[OpenApiTag("Gameplay")]
[Authorize]
[ApiController]
public class GameplayController : ControllerBase
{
    readonly UserManager<AppUser> _userManager;
    readonly GamesService _gamesService;
    readonly GameplayService _gameplayService;

    /// <summary>
    /// </summary>
    public GameplayController(UserManager<AppUser> userManager, GamesService gamesService, GameplayService gameplayService)
    {
        _userManager = userManager;
        _gameplayService = gameplayService;
        _gamesService = gamesService;
    }

    /// <summary>
    ///     Create game
    /// </summary>
    [HttpPost]
    public async Task<SudokuPlayerGameDto> StartNewGameAsync(CreateGameRequest request)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.StartNewGameAsync(request.Name, new SudokuGameOptions { MaxHints = request.Hints }, user);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player has not joined game");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Get game state
    /// </summary>
    /// <remarks>
    ///     Get the state of the game as seen by the current user.
    /// </remarks>
    [HttpGet("{gameId:guid}")]
    public async Task<SudokuPlayerGameDto> GetGameAsync(Guid gameId, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();

        SudokuGame game = await _gamesService.RequireGameAsync(gameId, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new AccessDeniedException();

        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Join game
    /// </summary>
    [HttpPost("{gameId:guid}")]
    public async Task<SudokuPlayerGameDto> JoinGameAsync(Guid gameId, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.JoinGameAsync(gameId, user, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Set cell element
    /// </summary>
    [HttpPut("{gameId:guid}/cell/{cellIndex:int}/element/{element:int}")]
    public async Task<SudokuPlayerGameDto> SetElementAsync(Guid gameId, int cellIndex, int element, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.SetElementAsync(gameId, user, cellIndex, element, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Clear cell element
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/element")]
    public async Task<SudokuPlayerGameDto> ClearElementAsync(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.ClearElementAsync(gameId, user, cellIndex, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Add cell annotation
    /// </summary>
    [HttpPut("{gameId:guid}/cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> AddAnnotationAsync(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.AddAnnotationAsync(gameId, user, annotation, cellIndex, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Remove cell annotation
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/annotations/{annotation:int}")]
    public async Task<SudokuPlayerGameDto> RemoveAnnotationAsync(Guid gameId, int cellIndex, int annotation, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.RemoveAnnotationAsync(gameId, user, annotation, cellIndex, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Clear cell annotation
    /// </summary>
    [HttpDelete("{gameId:guid}/cell/{cellIndex:int}/annotations")]
    public async Task<SudokuPlayerGameDto> ClearAnnotationsAsync(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.ClearAnnotationsAsync(gameId, user, cellIndex, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    /// <summary>
    ///     Use hint
    /// </summary>
    [HttpPost("{gameId:guid}/cell/{cellIndex:int}/hint")]
    public async Task<SudokuPlayerGameDto> UseHintAsync(Guid gameId, int cellIndex, CancellationToken cancellationToken)
    {
        string user = _userManager.GetUserName(HttpContext.User) ?? throw new AccessDeniedException();
        SudokuGame game = await _gameplayService.UseHintAsync(gameId, user, cellIndex, cancellationToken);
        PlayerState playerState = game.GetPlayerState(user) ?? throw new InternalErrorException("Player should have joined the game.");
        return await ComputePlayerGameDto(game, playerState);
    }

    async Task<SudokuPlayerGameDto> ComputePlayerGameDto(SudokuGame game, PlayerState playerState)
    {
        UserIdentityDto playerIdentity = await playerState.GetUserIdentity(_userManager);

        IHiddenPlayerState? otherPlayerState = game.GetOtherPlayerState(playerState.Username);
        UserIdentityDto? opponentIdentity = otherPlayerState == null ? null : await otherPlayerState.GetUserIdentity(_userManager);

        return game.ToPlayerGameDto(playerState, playerIdentity, opponentIdentity);
    }
}
