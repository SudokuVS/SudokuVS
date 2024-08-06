using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SudokuVS.Game.Services;
using SudokuVS.Sudoku.Serialization;

namespace SudokuVS.Game.Utils;

public static class AspNetHostingExtensions
{
    public static void ConfigureGameServices(this WebApplicationBuilder builder, GameOptions options)
    {
        builder.Services.AddTransient<SudokuGridEnumerableSerializer>();
        builder.Services.AddTransient<SudokuGridStringSerializer>();
        builder.Services.AddTransient<GameplayService>();
        builder.Services.AddTransient<GamesService>();
    }
}

public class GameOptions
{
    public ILogger? Logger { get; set; }
}
