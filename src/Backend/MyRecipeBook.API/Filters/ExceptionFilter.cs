using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is MyRecipeBookException)
            HandleProjectException(context);
        else
            ThrowUnknowException(context);
    }

    private static void HandleProjectException(ExceptionContext ctx)
    {
        switch (ctx.Exception)
        {
            case InvalidLoginException:
                ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ctx.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ctx.Exception.Message));
                return;
            case NotFoundException:
                ctx.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                ctx.Result = new NotFoundObjectResult(new ResponseErrorJson(ctx.Exception.Message));
                return;
        }

        if (ctx.Exception is not ErrorOnValidationException exception) return;

        ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        ctx.Result = new BadRequestObjectResult(new ResponseErrorJson(exception.ErrorMessages));
    }

    private void ThrowUnknowException(ExceptionContext ctx)
    {
        logger.LogError(ctx.Exception, "Unknown exception occurred: {Message}", ctx.Exception.Message);

        ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        ctx.Result = new ObjectResult(new ResponseErrorJson("An unexpected error occurred."));
    }
}