using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Authentication;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.RestApi.Controllers.Account;

[Route("/api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    readonly UserManager<AppUser> _userManager;
    readonly ApiKeyService _apiKeyService;

    public AccountController(UserManager<AppUser> userManager, ApiKeyService apiKeyService)
    {
        _userManager = userManager;
        _apiKeyService = apiKeyService;
    }

    /// <summary>
    ///     Create API key
    /// </summary>
    /// <remarks>
    ///     Create a new API key for the current account.
    /// </remarks>
    [HttpPost]
    public async Task<string> CreateApiKey()
    {
        AppUser user = await _userManager.GetUserAsync(HttpContext.User) ?? throw new AccessDeniedException();
        return await _apiKeyService.CreateNewApiKeyAsync(user);
    }

    /// <summary>
    ///     Get API keys
    /// </summary>
    /// <remarks>
    ///     Return all the API keys of the current account.
    /// </remarks>
    [HttpGet]
    public async Task<IReadOnlyList<string>> GetApiKeys()
    {
        AppUser user = await _userManager.GetUserAsync(HttpContext.User) ?? throw new AccessDeniedException();
        return await _apiKeyService.GetApiKeysAsync(user);
    }

    /// <summary>
    ///     Revoke API key
    /// </summary>
    /// <remarks>
    ///     Revoke the given API key of the current account.
    /// </remarks>
    [HttpDelete("{apiKey}")]
    public async Task RevokeApiKey(string apiKey)
    {
        AppUser user = await _userManager.GetUserAsync(HttpContext.User) ?? throw new AccessDeniedException();
        await _apiKeyService.RevokeApiKeyAsync(user, apiKey);
    }

    /// <summary>
    ///     Revoke all API keys
    /// </summary>
    /// <remarks>
    ///     Revoke all the API keys of the current account.
    /// </remarks>
    [HttpDelete]
    public async Task RevokeAllApiKeys()
    {
        AppUser user = await _userManager.GetUserAsync(HttpContext.User) ?? throw new AccessDeniedException();
        await _apiKeyService.RevokeAllApiKeysAsync(user);
    }
}
