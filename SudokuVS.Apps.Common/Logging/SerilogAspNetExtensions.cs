using Microsoft.AspNetCore.Builder;
using Serilog;

namespace SudokuVS.Apps.Common.Logging;

public static class SerilogAspNetExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder) => builder.Services.AddSerilog(c => Logging.ConfigureLogger(builder, c));
}
