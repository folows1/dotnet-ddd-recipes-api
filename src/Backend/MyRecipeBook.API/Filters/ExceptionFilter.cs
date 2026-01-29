using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
  public void OnException(ExceptionContext context)
  {
    if (context.Exception is MyRecipeBookException)
      HandleProjectException(context);
    else
    {
      ThrowUnknowException(context);
    }
  }

  private void HandleProjectException(ExceptionContext ctx)
  {
    if (ctx.Exception is ErrorOnValidationException)
    {
      var exception = ctx.Exception as ErrorOnValidationException;
      ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      ctx.Result = new BadRequestObjectResult(new ResponseErrorJson(exception.ErrorMessages));
    }
  }

  private void ThrowUnknowException(ExceptionContext ctx)
  {
    ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    ctx.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.NAME_EMPTY));
  }
}
