#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SudokuVS.Server.Infrastructure.Authentication;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Areas.Identity.Pages.Account.Manage;

public class ApiKeys : PageModel
{
    UserManager<AppUser> _userManager;
    ApiKeyService _apiKeyService;

    public ApiKeys(ApiKeyService apiKeyService, UserManager<AppUser> userManager)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
    }

    public IReadOnlyList<ApiKey> Keys { get; set; } = [];

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Keys = await _apiKeyService.GetApiKeysAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateKeyAsync(string name)
    {
        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _apiKeyService.CreateNewApiKeyAsync(user, name);

        StatusMessage = "A new API key was added.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRevokeKeyAsync(string token)
    {
        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _apiKeyService.RevokeApiKeyAsync(user, token);
        return RedirectToPage();
    }
}
