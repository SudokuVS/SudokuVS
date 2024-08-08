using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;

namespace SudokuVS.Server.Infrastructure.Logging;

public static class Logging
{
    const LogEventLevel InfrastructureLoggingLevel = LogEventLevel.Information;
    public const string OutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}";

    public static ReloadableLogger CreateBootstrapLogger() =>
        new LoggerConfiguration().WriteTo.Console(outputTemplate: OutputTemplate).Enrich.WithProperty("SourceContext", "Bootstrap").CreateBootstrapLogger();

    public static LoggerConfiguration ConfigureLogger(WebApplicationBuilder builder, LoggerConfiguration configuration) =>
        configuration.WriteTo.Console(outputTemplate: OutputTemplate)
            .Enrich.WithProperty("SourceContext", "Bootstrap")
            .MinimumLevel.Is(LogEventLevel.Debug)
            .MinimumLevel.Override("System.Net.Http.HttpClient", InfrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.Extensions.Http", InfrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore", InfrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.Identity", InfrastructureLoggingLevel)
            .MinimumLevel.Override("Microsoft.IdentityModel", InfrastructureLoggingLevel)
            .ReadFrom.Configuration(builder.Configuration);
}
