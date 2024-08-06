using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SudokuVS.Game.Infrastructure.Database;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Game.Utils;

public static class AspNetHostingExtensions
{
    public static void ConfigureGameServices(this WebApplicationBuilder builder, GameOptions options)
    {
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
}
