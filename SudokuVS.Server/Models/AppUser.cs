using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace SudokuVS.Server.Models;

public class AppUser : IdentityUser
{
    [MaxLength(64)]
    public string? DisplayName { get; set; }
}

static class UserManagerExtensions
{
    public static async Task<string?> GetPublicName(this UserManager<AppUser> manager, ClaimsPrincipal claims)
    {
        AppUser? user = await manager.GetUserAsync(claims);
        return user == null ? null : GetPublicName(user);
    }

    static string GetPublicName(AppUser user)
    {
        string? username = user.UserName;
        string? displayName = user.DisplayName;

        return !string.IsNullOrWhiteSpace(displayName)
            ? displayName
            : !string.IsNullOrWhiteSpace(username)
                ? username
                : $"user_{user.Id}";
    }

    public static Task<string?> GetDisplayNameAsync(this UserManager<AppUser> manager, AppUser user) => Task.FromResult(user.DisplayName);

    public static Task<IdentityResult> SetDisplayNameAsync(this UserManager<AppUser> manager, AppUser user, string? displayName)
    {
        user.DisplayName = displayName;
        return manager.UpdateAsync(user);
    }
}
