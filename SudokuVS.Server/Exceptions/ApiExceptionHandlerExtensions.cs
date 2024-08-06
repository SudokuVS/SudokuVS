using Microsoft.AspNetCore.Mvc.Infrastructure;
using SudokuVS.Game.Exceptions;

namespace SudokuVS.Server.Exceptions;

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
                            if (!await TryWriteApiException(problemDetailsService, problemDetailsFactory, context, apiException, hideDetails))
                            {
                                throw;
                            }
                            break;
                        case DomainException domainException:
                            if (!await TryWriteDomainException(problemDetailsService, problemDetailsFactory, context, domainException, hideDetails))
                            {
                                throw;
                            }
                            break;
                        default: throw;
                    }
                }
            }
        );

    static async Task<bool> TryWriteApiException(
        IProblemDetailsService problemDetailsService,
        ProblemDetailsFactory problemDetailsFactory,
        HttpContext context,
        ApiException apiException,
        bool hideDetails
    ) =>
        await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                Exception = hideDetails ? null : apiException,
                ProblemDetails = problemDetailsFactory.CreateProblemDetails(context, apiException.StatusCode, apiException.Title, detail: hideDetails ? null : apiException.Detail)
            }
        );

    static async Task<bool> TryWriteDomainException(
        IProblemDetailsService problemDetailsService,
        ProblemDetailsFactory problemDetailsFactory,
        HttpContext context,
        DomainException domainException,
        bool hideDetails
    )
    {
        int statusCode = domainException switch
        {
            InvalidArgumentException => StatusCodes.Status400BadRequest,
            InvalidKeyException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                Exception = hideDetails ? null : domainException,
                ProblemDetails = problemDetailsFactory.CreateProblemDetails(context, statusCode, detail: hideDetails ? null : domainException.Message)
            }
        );
    }
}
