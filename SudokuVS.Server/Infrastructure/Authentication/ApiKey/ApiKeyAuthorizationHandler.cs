using Microsoft.AspNetCore.Authorization;

namespace SudokuVS.Server.Infrastructure.Authentication.ApiKey;

public class AuthorizationHeaderRequirement : IAuthorizationRequirement
{
}

class ApiKeyAuthorizationHandler : AuthorizationHandler<AuthorizationHeaderRequirement>
{
    readonly IHttpContextAccessor _httpContextAccessor;

    public ApiKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationHeaderRequirement requirement)
    {
        string? apiKey = _httpContextAccessor.HttpContext?.Request.Headers[ApiKeySchemeOptions.HeaderName].ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            context.Fail(new AuthorizationFailureReason(this, "Bad API key."));
            return Task.CompletedTask;
        }

        // no need to check the actual key, if it was bad authentication would have discarded it.
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
