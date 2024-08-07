using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using OpenIddict.Abstractions;
using Serilog;
using SudokuVS.Game.Abstractions;
using SudokuVS.Game.Utils;
using SudokuVS.Server;
using SudokuVS.Server.Areas.App.Components;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Authentication;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;
using SudokuVS.Server.Infrastructure.Authorization;
using SudokuVS.Server.Infrastructure.Database;
using SudokuVS.Server.Infrastructure.Database.Models;
using SudokuVS.Server.Infrastructure.Logging;
using SudokuVS.Server.Infrastructure.Repositories;
using ILogger = Microsoft.Extensions.Logging.ILogger;

Log.Logger = Logging.CreateBootstrapLogger();
ILoggerFactory factory = new LoggerFactory().AddSerilog(Log.Logger);
ILogger bootstrapLogger = factory.CreateLogger("Bootstrap");

const string swaggerApplicationClientId = "swagger-oidc-app";

try
{
    Log.Logger.Information("Hello!");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    GameOptions gameOptions = new() { Logger = bootstrapLogger };

    builder.ConfigureSerilog();

    string? appConnectionString = builder.Configuration.GetConnectionString("AppDbContext");
    if (string.IsNullOrWhiteSpace(appConnectionString))
    {
        throw new InvalidOperationException("No connection string provided for AppDbContext.");
    }

    builder.Services.AddDbContextFactory<AppDbContext>(
        options =>
        {
            options.UseSqlServer(appConnectionString);
            options.UseOpenIddict();
        }
    );
    bootstrapLogger.LogInformation("Connection to AppComponent database configured.");

    builder.Services.AddSingleton<SudokuGamesInDbContext>();
    builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesInDbContext>(services => services.GetRequiredService<SudokuGamesInDbContext>());

    builder.Services.AddDefaultIdentity<AppUser>(
            options =>
            {
                // FIXME: enable when email sender configured properly
                options.SignIn.RequireConfirmedAccount = false;
            }
        )
        .AddEntityFrameworkStores<AppDbContext>();

    builder.AddAuthentication(bootstrapLogger);
    builder.AddAuthorization(bootstrapLogger);

    builder.Services.AddOpenIddict()
        .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<AppDbContext>())
        .AddServer(
            options =>
            {
                options.SetAuthorizationEndpointUris("connect/authorize")
                    .SetLogoutEndpointUris("connect/logout")
                    .SetTokenEndpointUris("connect/token")
                    .SetUserinfoEndpointUris("connect/userinfo");

                options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.Roles);

                options.AllowAuthorizationCodeFlow().AllowRefreshTokenFlow();

        #if DEBUG
                options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        #endif

                options.UseAspNetCore().EnableAuthorizationEndpointPassthrough().EnableLogoutEndpointPassthrough().EnableTokenEndpointPassthrough();
            }
        )
        .AddValidation(
            options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            }
        );

    builder.ConfigureGameServices(gameOptions);

    builder.Services.AddControllers()
        .AddJsonOptions(
            configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
        );
    builder.Services.AddRazorPages(options => { options.Conventions.AuthorizeAreaFolder("App", "/"); });
    builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddMicrosoftIdentityConsentHandler();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddProblemDetails();

    ConfigureOpenApiDocument(builder);

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

            if (app.Environment.IsDevelopment())
            {
                configure.OAuth2Client = new OAuth2ClientSettings
                    { AppName = "SudokuVS", ClientId = swaggerApplicationClientId, ClientSecret = "", UsePkceWithAuthorizationCodeGrant = true };
            }
        }
    );

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    app.MapRazorPages();
    app.MapRazorComponents<AppComponent>().AddInteractiveServerRenderMode();
    app.MapControllers();
    app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

    if (app.Environment.IsDevelopment())
    {
        await RegisterSwaggerOidcApplication(app);
    }

    PreloadGames(app);

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

void ConfigureOpenApiDocument(WebApplicationBuilder builder)
{
    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "SudokuVS - API";
            settings.Description = "Rest API for the Sudoku VS game.";
            settings.Version = Metadata.Version?.ToString();
            settings.DocumentName = "game-server";

            string? apiKeySecret = builder.Configuration.GetValue<string>("Authentication:ApiKey:Secret");
            if (!string.IsNullOrWhiteSpace(apiKeySecret))
            {
                const string schemeName = "API key";
                settings.AddSecurity(
                    schemeName,
                    new OpenApiSecurityScheme
                    {
                        Description = "Please enter your API key.",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Name = ApiKeySchemeOptions.HeaderName,
                        Type = OpenApiSecuritySchemeType.ApiKey
                    }
                );
                settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(schemeName));
                Log.Information("Swagger UI API key authentication configured.");
            }
            else
            {
                Log.Information("Swagger UI API key authentication not configured, please set configurations Authentication:ApiKey:Secret.");
            }

            if (builder.Environment.IsDevelopment())
            {
                const string devOidcAppSchemaName = "OIDC Application (dev)";
                settings.AddSecurity(
                    devOidcAppSchemaName,
                    new OpenApiSecurityScheme
                    {
                        Description = "Use the OIDC DEV application to authenticate.",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Name = HeaderNames.Authorization,
                        Type = OpenApiSecuritySchemeType.OpenIdConnect,
                        OpenIdConnectUrl = "/.well-known/openid-configuration"
                    }
                );
                settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(devOidcAppSchemaName));
                Log.Information("Swagger UI OIDC authentication configured (dev).");
            }
        }
    );
}

async Task RegisterSwaggerOidcApplication(WebApplication app)
{
    await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
    IOpenIddictApplicationManager manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    object? application = await manager.FindByClientIdAsync(swaggerApplicationClientId);
    if (application is not null)
    {
        await manager.DeleteAsync(application);
    }

    await manager.CreateAsync(
        new OpenIddictApplicationDescriptor
        {
            DisplayName = "Swagger UI application",
            ClientId = swaggerApplicationClientId,
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            RedirectUris = { new Uri("https://localhost:8001/swagger/oauth2-redirect.html") },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Logout,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        }
    );
    bootstrapLogger.LogInformation("Registered application {clientId}.", swaggerApplicationClientId);
}

void PreloadGames(WebApplication app)
{
    SudokuGamesInDbContext gamesInDbContext = app.Services.GetRequiredService<SudokuGamesInDbContext>();

    bootstrapLogger.LogInformation("Loading games from DB to memory...");
    gamesInDbContext.PreloadAsync().GetAwaiter().GetResult();
    bootstrapLogger.LogInformation("Done loading games to memory.");
}
