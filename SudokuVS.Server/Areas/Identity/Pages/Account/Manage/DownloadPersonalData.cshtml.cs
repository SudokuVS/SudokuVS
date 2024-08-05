// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SudokuVS.Server.Models;

namespace SudokuVS.Server.Areas.Identity.Pages.Account.Manage;

public class DownloadPersonalDataModel : PageModel
{
    readonly UserManager<AppUser> _userManager;
    readonly ILogger<DownloadPersonalDataModel> _logger;

    public DownloadPersonalDataModel(UserManager<AppUser> userManager, ILogger<DownloadPersonalDataModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult OnGet() => NotFound();

    public async Task<IActionResult> OnPostAsync()
    {
        AppUser user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

        // Only include personal data for download
        Dictionary<string, string> personalData = new Dictionary<string, string>();
        IEnumerable<PropertyInfo> personalDataProps = typeof(AppUser).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        foreach (PropertyInfo p in personalDataProps)
        {
            personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        }

        IList<UserLoginInfo> logins = await _userManager.GetLoginsAsync(user);
        foreach (UserLoginInfo l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add("Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

        Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}
