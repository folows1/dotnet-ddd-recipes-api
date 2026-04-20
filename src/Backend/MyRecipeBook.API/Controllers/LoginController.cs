using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Application.UseCases.Login.ExternalLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

public class LoginController : MyRecipeBookBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login
    (
        [FromServices] IDoLoginUseCase useCase,
        [FromBody] RequestLoginJson request
    )
    {
        var response = await useCase.Execute(request);
        return Ok(response);
    }

    [HttpGet]
    [Route("google")]
    public async Task<IActionResult> LoginGoogle(string returnUrl,
        [FromServices] IExternalLoginUseCase externalLoginUseCase
    )
    {
        var response = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (IsNotAuthenticated(response))
        {
            return Challenge(GoogleDefaults.AuthenticationScheme);
        }

        var claims = response.Principal!.Identities.First().Claims;
        var name = claims.First(c => c.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var email = claims.First(c => c.Type == System.Security.Claims.ClaimTypes.Email).Value;

        var token = externalLoginUseCase.Execute(name, email);

        return Redirect($"{returnUrl}/{token}");
    }
}