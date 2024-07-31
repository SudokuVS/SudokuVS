using System.Security.Cryptography;
using Serilog;
using Serilog.Events;
using SudokuVS.Game.Persistence;
using SudokuVS.RestApi;
using SudokuVS.Server.Components;
using SudokuVS.Server.Services;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Logger.Information("Hello!");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(
        c => c.WriteTo.Console()
            .MinimumLevel.Is(builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .ReadFrom.Configuration(builder.Configuration)
    );

    // Add services to the container.
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    builder.Services.AddRestApi();

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

    WebApplication app = builder.Build();

    app.UseRestApi();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", true);
        app.UseHsts();
    }

    app.UseStaticFiles();
    app.UseRouting();

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
