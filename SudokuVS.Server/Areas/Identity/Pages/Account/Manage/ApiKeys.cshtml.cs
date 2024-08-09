#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Areas.Identity.Pages.Account.Manage;

public class ApiKeys : PageModel
{
    UserManager<AppUser> _userManager;
    ApiKeyService _apiKeyService;
    IOptions<ApiKeyOptions> _options;

    public ApiKeys(ApiKeyService apiKeyService, UserManager<AppUser> userManager, IOptions<ApiKeyOptions> options)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
        _options = options;
    }

    public bool Enabled => _options.Value.Enabled;
    public IReadOnlyList<ApiKey> Keys { get; set; } = [];

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!Enabled)
        {
            StatusMessage = "Api Key authentication is disabled.";
            return Page();
        }

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
        if (!Enabled)
        {
            StatusMessage = "Api Key authentication is disabled.";
            return RedirectToPage();
        }

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
        if (!Enabled)
        {
            StatusMessage = "Api Key authentication is disabled.";
            return RedirectToPage();
        }

        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _apiKeyService.RevokeApiKeyAsync(user, token);
        return RedirectToPage();
    }
}
