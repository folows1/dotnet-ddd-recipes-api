using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Infra.DataAccess;

namespace MyRecipeBook.Infra.Services;

public class LoggedUser(MyRecipeBookDbContext dbContext, ITokenProvider tokenProvider)
    : ILoggedUser
{
    public async Task<User> User()
    {
        var token = tokenProvider.Value();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecToken = tokenHandler.ReadJwtToken(token);

        var identifier = jwtSecToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
        var guidP = Guid.Parse(identifier);

        return await dbContext.Users.AsNoTracking()
            .FirstAsync(user => user.Active && user.UserIdentifier == guidP);
    }
}