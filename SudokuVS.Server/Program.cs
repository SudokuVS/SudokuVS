using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;
using SudokuVS.Game.Abstractions;
using SudokuVS.Game.Services;
using SudokuVS.Game.Utils;
using SudokuVS.Server;
using SudokuVS.Server.Areas.App.Components;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Authentication;
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

    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(appConnectionString));
    bootstrapLogger.LogInformation("Connection to AppComponent database configured.");
    builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesInDbContext>();

    builder.Services.AddDefaultIdentity<AppUser>(
            options =>
            {
                // FIXME: enable when email sender configured properly
                options.SignIn.RequireConfirmedAccount = false;
            }
        )
        .AddEntityFrameworkStores<AppDbContext>();
    builder.AddAuthentication(bootstrapLogger);
    builder.Services.AddAuthorization();

    builder.ConfigureGameServices(gameOptions);
    builder.Services.AddTransient<GameplayService>();
    builder.Services.AddTransient<GamesService>();

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
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    app.MapRazorPages();
    app.MapRazorComponents<AppComponent>().AddInteractiveServerRenderMode();
    app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

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

            string? hostName = builder.Configuration.GetValue<string>("Host");
            if (string.IsNullOrWhiteSpace(hostName))
            {
                Log.Information("Swagger UI authentication not configured, please set configurations Host.");
                return;
            }

            string discovery = "https://api.passwordless.id/.well-known/openid-configuration";
            Log.Information("Swagger UI authentication configured: {path}.", discovery);

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
                    Type = OpenApiSecuritySchemeType.OpenIdConnect,
                    Flow = OpenApiOAuth2Flow.AccessCode,
                    OpenIdConnectUrl = discovery
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
