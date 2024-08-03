using System.Security.Claims;
using Microsoft.Identity.Web;

namespace SudokuVS.Game.Users;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetId(this ClaimsPrincipal claimsPrincipal)
    {
        string? idString = claimsPrincipal.FindFirst(ClaimConstants.Oid)?.Value;
        return idString != null && Guid.TryParse(idString, out Guid id) ? id : null;
    }

    public static string? GetName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirst(ClaimConstants.Name)?.Value ?? claimsPrincipal.Identity?.Name;

    public static UserIdentity? GetUserIdentity(this ClaimsPrincipal claimsPrincipal)
    {
        Guid? id = GetId(claimsPrincipal);
        return id.HasValue ? new UserIdentity { ExternalId = id.Value, Name = GetName(claimsPrincipal) ?? "Player" } : null;
    }
}
