using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SudokuVS.Game;
using SudokuVS.Server.RestApi.Models;

namespace SudokuVS.Server.Infrastructure.Database.Models;

[Index(nameof(UserName), IsUnique = true)]
public class AppUser : IdentityUser
{
    [MaxLength(64)]
    public string? DisplayName { get; set; }
}

static class UserManagerExtensions
{
    public static UserIdentityDto ToUserIdentity(this AppUser user) => new() { Username = GetUserName(user), PublicName = GetPublicName(user) };

    public static async Task<string?> GetPublicName(this UserManager<AppUser> manager, ClaimsPrincipal claims)
    {
        AppUser? user = await manager.GetUserAsync(claims);
        return user == null ? null : GetPublicName(user);
    }

    public static string GetPublicName(this AppUser user)
    {
        string? displayName = user.DisplayName;
        return !string.IsNullOrWhiteSpace(displayName) ? displayName : GetUserName(user);
    }

    public static string GetUserName(this AppUser user)
    {
        string? username = user.UserName;
        return !string.IsNullOrWhiteSpace(username) ? username : $"user_{user.Id}";
    }

    public static Task<string?> GetDisplayNameAsync(this UserManager<AppUser> manager, AppUser user, CancellationToken cancellationToken = default) =>
        Task.FromResult(user.DisplayName);

    public static Task<IdentityResult> SetDisplayNameAsync(this UserManager<AppUser> manager, AppUser user, string? displayName, CancellationToken cancellationToken = default)
    {
        user.DisplayName = displayName;
        return manager.UpdateAsync(user);
    }

    public static async Task<AppUser?> GetUserByIdAsync(this UserManager<AppUser> manager, string username) =>
        await manager.Users.SingleOrDefaultAsync(u => u.UserName == username);
}

static class AppUserExtensions
{
    public static Task<AppUser?> GetByUserNameAsync(this DbSet<AppUser> users, string username) => users.SingleOrDefaultAsync(u => u.UserName == username);
}

static class PlayerStateUserExtensions
{
    public static async Task<UserIdentityDto> GetUserIdentity(this IHiddenPlayerState state, UserManager<AppUser> userManager)
    {
        AppUser? opponent = await userManager.GetUserByIdAsync(state.Username);
        return opponent?.ToUserIdentity() ?? new UserIdentityDto { Username = state.Username, PublicName = state.Username };
    }
}
