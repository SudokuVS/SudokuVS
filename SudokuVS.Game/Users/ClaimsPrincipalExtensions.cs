using System.Security.Claims;
using Microsoft.Identity.Web;

namespace SudokuVS.Game.Users;

public static class ClaimsPrincipalExtensions
{
    public static string? GetId(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.GetNameIdentifierId();
    public static string? GetName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.GetDisplayName() ?? claimsPrincipal.GetNameIdentifierId();

    public static UserIdentity? GetUserIdentity(this ClaimsPrincipal claimsPrincipal)
    {
        string? id = GetId(claimsPrincipal);
        return id != null ? new UserIdentity { ExternalId = id, Name = GetName(claimsPrincipal) ?? "Player" } : null;
    }
}
