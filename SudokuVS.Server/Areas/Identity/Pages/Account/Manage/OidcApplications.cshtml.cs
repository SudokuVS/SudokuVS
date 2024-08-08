#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SudokuVS.Server.Infrastructure.Authentication.OpenIdConnect.Services;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Areas.Identity.Pages.Account.Manage;

public class OidcApplications : PageModel
{
    UserManager<AppUser> _userManager;
    OidcApplicationsService _oidcApplicationsService;

    public OidcApplications(OidcApplicationsService oidcApplicationsService, UserManager<AppUser> userManager)
    {
        _oidcApplicationsService = oidcApplicationsService;
        _userManager = userManager;
    }

    public IReadOnlyList<UserOpenIdApplicationEntity> Applications { get; set; } = [];

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    public string ReturnUrl { get; set; }

    public CreateNewApplicationModel CreateNewApplication;

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        CreateNewApplication ??= new CreateNewApplicationModel();

        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Applications = await _oidcApplicationsService.GetApplicationsAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateApplicationAsync(CreateNewApplicationModel createNewApplication, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            StatusMessage = "Bad input";
            return RedirectToPage();
        }

        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        UserOpenIdApplicationEntity application = await _oidcApplicationsService.CreateApplicationForAuthorizationCodeFlowAsync(
            user,
            createNewApplication.Name,
            new UserOpenIdApplicationOptions
            {
                ApplicationType = createNewApplication.ApplicationType,
                ConsentType = createNewApplication.ConsentType,
                RedirectUris = [createNewApplication.RedirectUri]
            }
        );

        StatusMessage = $"The OIDC application {application.Name} has been created.";

        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveApplicationAsync(string clientId)
    {
        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await _oidcApplicationsService.RemoveApplicationAsync(user, clientId);
        return RedirectToPage();
    }

    public class CreateNewApplicationModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Application type")]
        public OpenIdApplicationType ApplicationType { get; set; }

        [Display(Name = "Consent type")]
        public OpenIdConsentType ConsentType { get; set; }

        [Required]
        [Url]
        [Display(Name = "Redirect URI")]
        public string RedirectUri { get; set; }
    }
}
