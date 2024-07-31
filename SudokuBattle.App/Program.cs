using System.Security.Cryptography;
using Serilog;
using Serilog.Events;
using SudokuBattle.App.Components;
using SudokuBattle.App.Services;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(
        c => c.WriteTo.Console()
            .MinimumLevel.Is(builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .ReadFrom.Configuration(builder.Configuration)
    );

    // Add services to the container.
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();

    builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesOnDisk>(
        services =>
        {
            const string relativePath = "%LOCALAPPDATA%/SudokuBattle/repository/games";
            string path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(relativePath));
            Log.Logger.Information("Game repository path: {path}", path);
            return new SudokuGamesOnDisk(path, services.GetRequiredService<ILogger<SudokuGamesOnDisk>>());
        }
    );
    builder.Services.AddSingleton<GameTokenService>(_ => new GameTokenService(RandomNumberGenerator.GetBytes(64)));

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception exn)
{
    Log.Fatal(exn, "Uncaught exception.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
