using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SudokuVS.WebApi;

public static class AspNetHostingExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument(
            settings =>
            {
                settings.Title = "SudokuVS - API";
                settings.Description = "Rest API for the Sudoku VS game.";
                settings.Version = Metadata.Version?.ToString();
            }
        );

        return builder;
    }

    public static WebApplication UseSwagger(this WebApplication app)
    {
        app.UseOpenApi(settings => { settings.PostProcess = (document, request) => { document.Host = request.Host.Value; }; });

        app.UseSwaggerUi(configure => { configure.WithCredentials = false; });

        return app;
    }
}
