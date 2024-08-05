using Microsoft.AspNetCore.Authentication;

namespace SudokuVS.Server.Infrastructure.Authentication;

public static class AspNetAuthenticationExtensions
{
    public static void AddAuthentication(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        AuthenticationBuilder authBuilder = builder.Services.AddAuthentication();

        AddGoogleAuthentication(builder, logger, authBuilder);
        AddMicrosoftAccountAuthentication(builder, logger, authBuilder);
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
}
