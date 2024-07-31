using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SudokuVS.RestApi;

public static class AspNetHostingExtensions
{
    public static IServiceCollection AddRestApi(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(PingController).Assembly)
            .AddJsonOptions(
                opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }
            );

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument(
            settings =>
            {
                settings.Title = "SudokuVS - API";
                settings.Description = "Rest API for the Sudoku VS game.";
                settings.Version = Metadata.Version?.ToString();
            }
        );

        return services;
    }

    public static WebApplication UseRestApi(this WebApplication app)
    {
        app.UseOpenApi(
            settings =>
            {
                settings.PostProcess = (document, request) => { document.Host = request.Host.Value; };
            }
        );
        
        app.UseSwaggerUi(configure => { configure.WithCredentials = false; });

        return app;
    }
}
