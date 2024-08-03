using System.Security.Claims;
using Microsoft.Identity.Web;

namespace SudokuVS.Game.Users;

public static class ClaimsPrincipalExtensions
{
    public static string? GetId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst(ClaimConstants.Sub)?.Value;
    }

    public static string? GetName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirst(ClaimConstants.Name)?.Value ?? claimsPrincipal.Identity?.Name;

    public static UserIdentity? GetUserIdentity(this ClaimsPrincipal claimsPrincipal)
    {
        var id = GetId(claimsPrincipal);
        return id != null ? new UserIdentity { ExternalId = id, Name = GetName(claimsPrincipal) ?? "Player" } : null;
    }
}
