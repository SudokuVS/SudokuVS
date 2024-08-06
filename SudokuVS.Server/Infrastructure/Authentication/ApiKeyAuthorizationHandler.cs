using Microsoft.AspNetCore.Authorization;

namespace SudokuVS.Server.Infrastructure.Authentication;

public class ApiKeyAuthorizationRequirement : IAuthorizationRequirement
{
}

class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyAuthorizationRequirement>
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly ApiKeyService _apiKeyService;

    public ApiKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ApiKeyService apiKeyService)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiKeyService = apiKeyService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyAuthorizationRequirement requirement)
    {
        string? apiKey = _httpContextAccessor.HttpContext?.Request.Headers[ApiKeySchemeOptions.HeaderName].ToString();
        if (string.IsNullOrWhiteSpace(apiKey) || !await _apiKeyService.ValidateTokenAsync(apiKey))
        {
            context.Fail(new AuthorizationFailureReason(this, "Bad API key."));
            return;
        }

        context.Succeed(requirement);
    }
}
