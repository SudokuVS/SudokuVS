using Serilog;

namespace SudokuVS.Server.Infrastructure.Logging;

public static class SerilogAspNetExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder) => builder.Services.AddSerilog(c => Logging.ConfigureLogger(builder, c));
}
