using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SudokuVS.RestApi;

public static class AspNetHostingExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
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

    public static WebApplication UseSwagger(this WebApplication app)
    {
        app.UseOpenApi(settings => { settings.PostProcess = (document, request) => { document.Host = request.Host.Value; }; });

        app.UseSwaggerUi(configure => { configure.WithCredentials = false; });

        return app;
    }
}
