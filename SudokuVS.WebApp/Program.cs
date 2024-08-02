using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Serilog;
using SudokuVS.Apps.Common.Logging;
using SudokuVS.Game.Utils;
using SudokuVS.WebApp.Components;
using ILogger = Microsoft.Extensions.Logging.ILogger;

Log.Logger = Logging.CreateBootstrapLogger();
ILoggerFactory factory = new LoggerFactory().AddSerilog(Log.Logger);
ILogger bootstrapLogger = factory.CreateLogger("Bootstrap");

try
{
    bootstrapLogger.LogInformation("Hello!");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.ConfigureSerilog();
    ConfigureMicrosoftEntra(builder);
    builder.ConfigureGameServices(bootstrapLogger);

    builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddMicrosoftIdentityConsentHandler();

    WebApplication app = builder.Build();

    await app.UseGameServicesAsync();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", true);
        app.UseHsts();
    }
    else
    {
        IdentityModelEventSource.ShowPII = true;
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    app.MapDefaultControllerRoute();

    app.Run();

    bootstrapLogger.LogInformation("Bye!");
}
catch (Exception exn)
{
    bootstrapLogger.LogCritical(exn, "Uncaught exception.");
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
}
