namespace MyRecipeBook.Domain.Repos.User;

public interface IUserReadOnlyRepo
{
    public Task<bool> ExistActiveUserWithEmail(string email);
    public Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier);

    public Task<Entities.User?> GetByEmailAndPassword(string email, string password);
    public Task<Entities.User?> GetByEmail(string email);
}