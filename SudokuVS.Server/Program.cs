using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Events;
using SudokuVS.Game.Persistence;
using SudokuVS.RestApi;
using SudokuVS.Server.Components;
using SudokuVS.Server.Services;

const LogEventLevel infrastructureLoggingLevel = LogEventLevel.Warning;
const string serilogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}";

Log.Logger = new LoggerConfiguration().WriteTo.Console(outputTemplate: serilogTemplate).Enrich.WithProperty("SourceContext", "Bootstrap").CreateBootstrapLogger();

try
{
    Log.Logger.Information("Hello!");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.IsDevelopment())
    {
        IdentityModelEventSource.ShowPII = true;
    }

    builder.Services.AddSerilog(
        c => c.WriteTo.Console(outputTemplate: serilogTemplate)
            .Enrich.WithProperty("SourceContext", "Bootstrap")
            .MinimumLevel.Is(builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information)
            .MinimumLevel.Override("System.Net.Http.HttpClient", infrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.Extensions.Http", infrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore", infrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.Identity", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.IdentityModel", LogEventLevel.Debug)
            .ReadFrom.Configuration(builder.Configuration)
    );

    // Add services to the container.
    builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddMicrosoftIdentityConsentHandler();
    builder.Services.AddControllers()
        .AddApplicationPart(typeof(PingController).Assembly)
        .AddJsonOptions(
            opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
        );
    builder.Services.AddControllersWithViews(
            options =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }
        )
        .AddMicrosoftIdentityUI();

    // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
    // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
    // For instance, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles' claim.
    // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    // Sign-in users with the Microsoft identity platform
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration)
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddDownstreamApi("GraphApi", builder.Configuration.GetSection("GraphApi"))
        .AddInMemoryTokenCaches();

    builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesOnDisk>(
        services =>
        {
            const string relativePath = "%LOCALAPPDATA%/SudokuVS/repository/games";
            string path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(relativePath));
            Log.Logger.Information("Game repository path: {path}", path);
            return new SudokuGamesOnDisk(path, services.GetRequiredService<ILogger<SudokuGamesOnDisk>>());
        }
    );
    builder.Services.AddSingleton<GameTokenService>(_ => new GameTokenService(RandomNumberGenerator.GetBytes(64)));

    builder.AddSwagger();

    WebApplication app = builder.Build();

    app.UseSwagger();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", true);
        app.UseHsts();
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
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
