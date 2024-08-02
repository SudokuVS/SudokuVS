using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SudokuVS.Game.Persistence;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Game.Utils;

public static class AspNetHostingExtensions
{
    public static void ConfigureGameServices(this WebApplicationBuilder builder, ILogger logger)
    {
        string? connectionString = builder.Configuration.GetConnectionString("AppDbContext");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogInformation("No connection string provided for AppDbContext, falling back to in-memory repository");
            builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesInMemory>();
        }
        else
        {
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            logger.LogInformation("Using DbContext game repository");
            builder.Services.AddSingleton<ISudokuGamesRepository, SudokuGamesInDbContext>();
        }


        builder.Services.AddTransient<SudokuGridEnumerableSerializer>();
        builder.Services.AddTransient<SudokuGridStringSerializer>();
    }

    public static async Task UseGameServicesAsync(this WebApplication app)
    {
        IServiceScopeFactory scopeProvider = app.Services.GetRequiredService<IServiceScopeFactory>();
        using IServiceScope scope = scopeProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}
