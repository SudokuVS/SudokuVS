using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;

namespace SudokuVS.Server.Infrastructure.Authorization;

public static class AspNetAuthorizationExtensions
{
    public static void AddAuthorization(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(
                AuthorizationConstants.ApiAuthorizationPolicy,
                policy =>
                {
                    // combine both open id dict and api key auth for APIs

                    policy.AddAuthenticationSchemes(ApiKeyConstants.AuthenticationScheme);
                    policy.AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                    policy.Requirements.Add(new ApiAuthorizationRequirement());
                }
            );

        builder.Services.AddScoped<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
    }
}
