using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;

namespace SudokuVS.Server.Infrastructure.Authentication;

public static class AspNetAuthenticationExtensions
{
    public static void AddAuthentication(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        AuthenticationBuilder authBuilder = builder.Services.AddAuthentication();

        AddGoogleAuthentication(builder, logger, authBuilder);
        AddMicrosoftAccountAuthentication(builder, logger, authBuilder);
        AddApiKeyAuthentication(builder, logger, authBuilder);
    }

    public static void AddAuthorization(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        builder.Services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    ApiKeyConstants.AuthenticationScheme,
                    policy =>
                    {
                        policy.AddAuthenticationSchemes(ApiKeyConstants.AuthenticationScheme);
                        policy.Requirements.Add(new AuthorizationHeaderRequirement());
                    }
                );
            }
        );

        builder.Services.AddScoped<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
    }

    static void AddGoogleAuthentication(WebApplicationBuilder builder, ILogger? logger, AuthenticationBuilder authenticationBuilder)
    {
        string? googleClientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId");
        string? googleClientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret");
        if (string.IsNullOrWhiteSpace(googleClientId) || string.IsNullOrWhiteSpace(googleClientSecret))
        {
            logger?.LogInformation("Google Auth not configured.");
            return;
        }

        authenticationBuilder.AddGoogle(
            options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
                options.UsePkce = true;
            }
        );

        logger?.LogInformation("Google auth configured.");
    }

    static void AddMicrosoftAccountAuthentication(WebApplicationBuilder builder, ILogger? logger, AuthenticationBuilder authenticationBuilder)
    {
        string? microsoftClientId = builder.Configuration.GetValue<string>("Authentication:Microsoft:ClientId");
        string? microsoftClientSecret = builder.Configuration.GetValue<string>("Authentication:Microsoft:ClientSecret");
        if (string.IsNullOrWhiteSpace(microsoftClientId) || string.IsNullOrWhiteSpace(microsoftClientSecret))
        {
            logger?.LogInformation("Microsoft Auth not configured.");
            return;
        }

        authenticationBuilder.AddMicrosoftAccount(
            options =>
            {
                options.ClientId = microsoftClientId;
                options.ClientSecret = microsoftClientSecret;
                options.UsePkce = true;
            }
        );

        logger?.LogInformation("Microsoft Account auth configured.");
    }

    static void AddApiKeyAuthentication(WebApplicationBuilder builder, ILogger? logger, AuthenticationBuilder authenticationBuilder)
    {
        string? secret = builder.Configuration.GetValue<string>("Authentication:ApiKey:Secret");
        if (string.IsNullOrWhiteSpace(secret))
        {
            logger?.LogInformation("API Key not configured.");
            return;
        }

        builder.Services.Configure<ApiKeyOptions>(opt => opt.Secret = secret);
        builder.Services.AddScoped<ApiKeyService>();
        authenticationBuilder.AddScheme<ApiKeySchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyConstants.AuthenticationScheme, _ => { });

        logger?.LogInformation("API Key auth configured.");
    }
}
