using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.RefreshToken;

namespace MyRecipeBook.Infra.DataAccess.Repos;

public class TokenRepo : ITokenRepo
{
    private readonly MyRecipeBookDbContext _dbCtx;

    public TokenRepo(MyRecipeBookDbContext dbCtx)
    {
        _dbCtx = dbCtx;
    }

    public async Task<RefreshToken?> Get(string refreshToken)
    {
        return await _dbCtx
            .RefreshTokens
            .AsNoTracking()
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.Value.Equals(refreshToken));
    }

    public async Task SaveNewRefreshToken(RefreshToken refreshToken)
    {
        var tokens = _dbCtx.RefreshTokens.Where(token => token.UserId == refreshToken.UserId);

        _dbCtx.RefreshTokens.RemoveRange(tokens);

        await _dbCtx.RefreshTokens.AddAsync(refreshToken);
    }
}