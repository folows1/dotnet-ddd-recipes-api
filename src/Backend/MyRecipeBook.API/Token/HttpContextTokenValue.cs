using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.API.Token;

public class HttpContextTokenValue(IHttpContextAccessor accessor) : ITokenProvider
{
    public string Value()
    {
        var token = accessor.HttpContext!.Request.Headers.Authorization.ToString();

        return token["Bearer ".Length..].Trim();
    }
}