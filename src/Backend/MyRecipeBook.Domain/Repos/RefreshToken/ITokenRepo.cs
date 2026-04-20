namespace MyRecipeBook.Domain.Repos.RefreshToken;

public interface ITokenRepo
{
    public Task<Entities.RefreshToken?> Get(string refreshToken);
    public Task SaveNewRefreshToken(Entities.RefreshToken refreshToken);
}