using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;
using SudokuVS.Apps.Common.Logging;
using SudokuVS.Apps.Common.Services;
using SudokuVS.Game.Persistence;
using SudokuVS.WebApi;
using SudokuVS.WebApi.Exceptions;

Log.Logger = Logging.CreateBootstrapLogger();

try
{
    Log.Logger.Information("Hello!");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.ConfigureSerilog();
    ConfigureMicrosoftEntra(builder);

    builder.Services.AddControllers()
        .AddJsonOptions(
            configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
        );
    builder.Services.AddRazorPages();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddProblemDetails();

    ConfigureOpenApiDocument(builder);

    builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesOnDisk>(
        services =>
        {
            const string relativePath = "%LOCALAPPDATA%/SudokuVS/repository/games";
            string path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(relativePath));
            Log.Logger.Information("Game repository path: {path}", path);
            return new SudokuGamesOnDisk(path, services.GetRequiredService<ILogger<SudokuGamesOnDisk>>());
        }
    );

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        IdentityModelEventSource.ShowPII = true;
        IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    }

    app.UseApiExceptionMiddleware(app.Environment.IsProduction());

    app.UseOpenApi(settings => { settings.PostProcess = (document, request) => { document.Host = request.Host.Value; }; });
    app.UseSwaggerUi(
        configure =>
        {
            configure.PersistAuthorization = true;

            string? clientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");
            string? clientSecret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
            if (!string.IsNullOrWhiteSpace(clientId) || !string.IsNullOrWhiteSpace(clientSecret))
            {
                configure.OAuth2Client = new OAuth2ClientSettings { ClientId = clientId, ClientSecret = clientSecret };
            }
        }
    );

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapDefaultControllerRoute();

    app.Run();

    Log.Information("Bye!");
}
catch (Exception exn)
{
    Log.Fatal(exn, "Uncaught exception.");
}
finally
{
    await Log.CloseAndFlushAsync();
}

return;

static void ConfigureMicrosoftEntra(WebApplicationBuilder builder)
{
    string? clientId = builder.Configuration.GetValue<string>("AzureAd:ClientId", "");
    if (string.IsNullOrWhiteSpace(clientId))
    {
        Log.Information("Microsoft Entra application not configured, please set configurations AzureAd:ClientId and AzureAd:ClientCredentials:0:ClientSecret properly.");
        return;
    }

    Log.Information("Found Microsoft Entra application {client-id}.", clientId);

    builder.Services.AddControllers().AddMicrosoftIdentityUI();

    // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
    // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
    // For instance, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles' claim.
    // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    // Sign-in users with the Microsoft identity platform
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd", "Bearer", builder.Environment.IsDevelopment());
}

void ConfigureOpenApiDocument(WebApplicationBuilder builder)
{
    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "SudokuVS - API";
            settings.Description = "Rest API for the Sudoku VS game.";
            settings.Version = Metadata.Version?.ToString();
            settings.DocumentName = "game-server";

            string? instance = builder.Configuration.GetValue<string>("AzureAd:Instance");
            string? tenantId = builder.Configuration.GetValue<string>("AzureAd:TenantId");
            string? clientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");

            if (string.IsNullOrWhiteSpace(instance) || string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(clientId))
            {
                Log.Information("Swagger UI authentication not configured, please set configurations AzureAd:Instance, AzureAd:TenantId and AzureAd:ClientId.");
                return;
            }

            string basePath = $"{instance}{tenantId}/oauth2/v2.0/";
            Log.Information("Swagger UI authentication configured: {path}.", basePath);

            const string schemeName = "Microsoft Entra";
            settings.AddSecurity(
                schemeName,
                new OpenApiSecurityScheme
                {
                    Description = "Bearer {access_token}",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    AuthorizationUrl = $"{basePath}authorize",
                    TokenUrl = $"{basePath}token",
                    Scopes = new Dictionary<string, string> { { "api://55a9fda1-7bb2-45c4-b99d-eec6b0caef11/SudokuVS.Play", "Play the game" } }
                }
            );
            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(schemeName));
        }
    );
}
