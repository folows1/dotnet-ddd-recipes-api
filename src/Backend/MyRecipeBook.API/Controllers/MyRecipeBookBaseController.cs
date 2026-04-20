using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Domain.Extensions;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MyRecipeBookBaseController : ControllerBase
{
    protected static bool IsNotAuthenticated(AuthenticateResult result)
    {
        return result.Succeeded.IsFalse()
               || result.Principal is null
               || result.Principal.Identities.Any(i => i.IsAuthenticated).IsFalse();
    }
}