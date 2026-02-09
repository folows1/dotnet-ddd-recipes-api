using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Infra.Security.Tokens.Access.Validator;

public class JwtTokenValidator(string signingKey) : JwtTokenHandler, IAccessTokenValidator
{
    public Guid ValidateAndGetUserIdentifier(string token)
    {
        var validationParameter = new TokenValidationParameters
        {
            ClockSkew = new TimeSpan(0),
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = SecurityKey(signingKey)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, validationParameter, out _);
        var userIdentifier = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        return Guid.Parse(userIdentifier);
    }
}