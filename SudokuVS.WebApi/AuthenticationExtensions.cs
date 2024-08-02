using Microsoft.AspNetCore.Mvc;
using SudokuVS.Game.Users;
using SudokuVS.WebApi.Exceptions;

namespace SudokuVS.WebApi;

static class AuthenticationExtensions
{
    public static Guid? GetAuthenticatedUserId(this ControllerContext context) => context.HttpContext.User.GetId();

    public static Guid RequireAuthenticatedUserId(this ControllerContext context) => GetAuthenticatedUserId(context) ?? throw new AccessDeniedException();
}
