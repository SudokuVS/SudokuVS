using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;
using SudokuVS.Apps.Common.Authentication;
using SudokuVS.Apps.Common.Logging;
using SudokuVS.Game.Utils;
using SudokuVS.WebApi;
using SudokuVS.WebApi.Exceptions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

Log.Logger = Logging.CreateBootstrapLogger();
ILoggerFactory factory = new LoggerFactory().AddSerilog(Log.Logger);
ILogger bootstrapLogger = factory.CreateLogger("Bootstrap");

try
{
    Log.Logger.Information("Hello!");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.ConfigureSerilog();
    builder.ConfigureGameServices(bootstrapLogger);

    builder.AddPasswordIdAuthentication();
    builder.Services.AddAuthorization();

    builder.Services.AddControllers()
        .AddJsonOptions(
            configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
        );
    builder.Services.AddRazorPages();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddProblemDetails();

    ConfigureOpenApiDocument(builder);

    WebApplication app = builder.Build();

    await app.UseGameServicesAsync();

    if (app.Environment.IsDevelopment())
    {
        IdentityModelEventSource.ShowPII = true;
        IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    }

    app.UseApiExceptionMiddleware(app.Environment.IsProduction());

    app.UseOpenApi(settings => { settings.PostProcess = (document, request) => { document.Host = request.Host.Value; }; });
    app.UseSwaggerUi(
        configure =>
        {
            configure.PersistAuthorization = true;

            string? hostName = builder.Configuration.GetValue<string>("Host");
            if (!string.IsNullOrWhiteSpace(hostName))
            {
                configure.OAuth2Client = new OAuth2ClientSettings { AppName = "SudokuVS", ClientId = hostName };
            }
        }
    );

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
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

return;

void ConfigureOpenApiDocument(WebApplicationBuilder builder)
{
    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "SudokuVS - API";
            settings.Description = "Rest API for the Sudoku VS game.";
            settings.Version = Metadata.Version?.ToString();
            settings.DocumentName = "game-server";

            string? hostName = builder.Configuration.GetValue<string>("Host");
            if (string.IsNullOrWhiteSpace(hostName))
            {
                Log.Information("Swagger UI authentication not configured, please set configurations Host.");
                return;
            }

            string discovery = "https://api.passwordless.id/.well-known/openid-configuration";
            Log.Information("Swagger UI authentication configured: {path}.", discovery);

            const string schemeName = "Microsoft Entra";
            settings.AddSecurity(
                schemeName,
                new OpenApiSecurityScheme
                {
                    Description = "Bearer {access_token}",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Type = OpenApiSecuritySchemeType.OpenIdConnect,
                    Flow = OpenApiOAuth2Flow.AccessCode,
                    OpenIdConnectUrl = discovery
                }
            );
            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(schemeName));
        }
    );
}
