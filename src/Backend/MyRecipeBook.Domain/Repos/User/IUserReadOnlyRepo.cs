namespace MyRecipeBook.Domain.Repos.User;

public interface IUserReadOnlyRepo
{
    public Task<bool> ExistActiveUserWithEmail(string email);

    public Task<Entities.User> GetByEmailAndPassword(string email, string password);
}