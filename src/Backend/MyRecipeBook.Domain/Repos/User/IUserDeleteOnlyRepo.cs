namespace MyRecipeBook.Domain.Repos.User;

public interface IUserDeleteOnlyRepo
{
    public Task DeleteAccount(Guid userIdentifier);
}