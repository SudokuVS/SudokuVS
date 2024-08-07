using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;
using SudokuVS.Game.Abstractions;
using SudokuVS.Game.Utils;
using SudokuVS.Server;
using SudokuVS.Server.Areas.App.Components;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Authentication;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;
using SudokuVS.Server.Infrastructure.Authentication.Oidc;
using SudokuVS.Server.Infrastructure.Database;
using SudokuVS.Server.Infrastructure.Database.Models;
using SudokuVS.Server.Infrastructure.Logging;
using SudokuVS.Server.Infrastructure.Repositories;
using ILogger = Microsoft.Extensions.Logging.ILogger;

Log.Logger = Logging.CreateBootstrapLogger();
ILoggerFactory factory = new LoggerFactory().AddSerilog(Log.Logger);
ILogger bootstrapLogger = factory.CreateLogger("Bootstrap");

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

    builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer(appConnectionString));
    builder.Services.AddDbContext<AppDbContext>(
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
                options.SetTokenEndpointUris("connect/token");
                options.AllowClientCredentialsFlow();

        #if DEBUG
                options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        #endif

                options.UseAspNetCore().EnableTokenEndpointPassthrough();
            }
        );
    builder.Services.AddHostedService<OidcApplicationsWorker>();

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

    await ApplyMigrations<AppDbContext>(app);

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

            string? hostName = builder.Configuration.GetValue<string>("Host");
            if (!string.IsNullOrWhiteSpace(hostName))
            {
                configure.OAuth2Client = new OAuth2ClientSettings { AppName = "SudokuVS", ClientId = hostName };
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
            if (string.IsNullOrWhiteSpace(apiKeySecret))
            {
                Log.Information("Swagger UI authentication not configured, please set configurations Authentication:ApiKey:Secret.");
                return;
            }

            Log.Information("Swagger UI API key authentication.");

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
        }
    );
}

async Task ApplyMigrations<TContext>(WebApplication app) where TContext: DbContext
{
    bootstrapLogger.LogInformation("Applying migrations to {ctx}", typeof(TContext).Name);

    IServiceScopeFactory scopeProvider = app.Services.GetRequiredService<IServiceScopeFactory>();
    using IServiceScope scope = scopeProvider.CreateScope();
    TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
    await context.Database.MigrateAsync();
}

void PreloadGames(WebApplication app)
{
    SudokuGamesInDbContext gamesInDbContext = app.Services.GetRequiredService<SudokuGamesInDbContext>();

    bootstrapLogger.LogInformation("Loading games from DB to memory...");
    gamesInDbContext.PreloadAsync().GetAwaiter().GetResult();
    bootstrapLogger.LogInformation("Done loading games to memory.");
}
