using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Serilog;
using SudokuVS.Apps.Common.Authentication;
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
    builder.ConfigureGameServices(bootstrapLogger);

    builder.AddPasswordIdAuthentication();
    builder.Services.AddAuthorization();

    builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddMicrosoftIdentityConsentHandler();
    builder.Services.AddControllers();

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

    app.MapGet(
            "/login",
            context =>
            {
                context.Response.Redirect("/");
                return Task.CompletedTask;
            }
        )
        .RequireAuthorization();

    app.MapGet(
            "/logout",
            async context =>
            {
                await context.SignOutAsync(IdentityConstants.ExternalScheme);
                context.Response.Redirect("/");
            }
        )
        .RequireAuthorization();

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
