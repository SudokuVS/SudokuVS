using Microsoft.AspNetCore.Mvc;
using SudokuVS.Game.Users;
using SudokuVS.WebApi.Exceptions;

namespace SudokuVS.WebApi;

static class AuthenticationExtensions
{
    public static string? GetAuthenticatedUserId(this ControllerContext context) => context.HttpContext.User.GetId();
    public static string RequireAuthenticatedUserId(this ControllerContext context) => GetAuthenticatedUserId(context) ?? throw new AccessDeniedException();
}
