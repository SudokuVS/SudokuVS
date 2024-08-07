using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;

namespace SudokuVS.Server.Infrastructure.Authorization;

public static class AuthorizationConstants
{
    public const string ApiAuthorizationPolicy = "ApiAuthorizationPolicy";
}

public static class AspNetAuthorizationExtensions
{
    public static void AddAuthorization(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        builder.Services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    AuthorizationConstants.ApiAuthorizationPolicy,
                    policy =>
                    {
                        policy.AddAuthenticationSchemes(ApiKeyConstants.AuthenticationScheme);
                        policy.AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                        policy.Requirements.Add(new ApiAuthorizationRequirement());
                    }
                );
            }
        );

        builder.Services.AddScoped<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
    }
}
