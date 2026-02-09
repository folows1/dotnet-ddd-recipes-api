using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
{
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IUserReadOnlyRepo _repo;

    public AuthenticatedUserFilter(IAccessTokenValidator accessTokenValidator, IUserReadOnlyRepo repo)
    {
        _accessTokenValidator = accessTokenValidator;
        _repo = repo;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);
            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

            var exist = await _repo.ExistActiveUserWithIdentifier(userIdentifier);

            if (exist.IsFalse())
            {
                throw new MyRecipeBookException(ResourceMessagesException.NAME_EMPTY);
            }
        }
        catch (MyRecipeBookException e)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(e.Message));
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("TokenIsExpired")
            {
                TokenIsExpired = true
            });
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.NAME_EMPTY));
        }
    }

    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        var auth = context.HttpContext.Request.Headers.Authorization.ToString();

        return string.IsNullOrWhiteSpace(auth)
            ? throw new MyRecipeBookException(ResourceMessagesException.NAME_EMPTY)
            : auth["Bearer ".Length..].Trim();
    }
}