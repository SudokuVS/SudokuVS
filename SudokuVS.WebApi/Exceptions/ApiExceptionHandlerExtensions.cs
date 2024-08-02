using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SudokuVS.WebApi.Exceptions;

static class ApiExceptionHandlerExtensions
{
    public static void UseApiExceptionMiddleware(this WebApplication app, bool hideDetails) =>
        app.Use(
            async (context, next) =>
            {
                IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
                ProblemDetailsFactory problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                try
                {
                    await next(context);
                }
                catch (Exception exn)
                {
                    switch (exn)
                    {
                        case ApiException apiException:
                            bool written = await problemDetailsService.TryWriteAsync(
                                new ProblemDetailsContext
                                {
                                    HttpContext = context,
                                    Exception = hideDetails ? null : exn,
                                    ProblemDetails = problemDetailsFactory.CreateProblemDetails(
                                        context,
                                        apiException.StatusCode,
                                        apiException.Title,
                                        detail: hideDetails ? null : apiException.Detail
                                    )
                                }
                            );

                            if (!written)
                            {
                                throw;
                            }
                            break;
                        default: throw;
                    }
                }
            }
        );
}
