using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SudokuVS.Game.Infrastructure.Database;
using SudokuVS.Game.Persistence;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Game.Utils;

public static class AspNetHostingExtensions
{
    public static void ConfigureGameServices(this WebApplicationBuilder builder, GameOptions options)
    {
        switch (options.PersistenceMode)
        {
            case PersistenceMode.InMemory:
                options.Logger?.LogInformation("Using InMemory game repository");
                builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesInMemory>();
                break;
            case PersistenceMode.Database:
                options.Logger?.LogInformation("Using DbContext game repository");
                builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesInDbContext>();
                break;
        }


        builder.Services.AddTransient<SudokuGridEnumerableSerializer>();
        builder.Services.AddTransient<SudokuGridStringSerializer>();
    }

    public static async Task UseGameServicesAsync(this WebApplication app)
    {
        IServiceScopeFactory scopeProvider = app.Services.GetRequiredService<IServiceScopeFactory>();
        using IServiceScope scope = scopeProvider.CreateScope();
        GameDbContext context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await context.Database.MigrateAsync();
    }
}

public class GameOptions
{
    public ILogger? Logger { get; set; }
    public PersistenceMode PersistenceMode { get; set; } = PersistenceMode.Database;
}

public enum PersistenceMode
{
    InMemory,
    Database
}
