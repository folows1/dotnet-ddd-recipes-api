namespace MyRecipeBook.Domain.Repos.User;

public interface IUserUpdateOnlyRepo
{
    public Task<Entities.User> GetById(long id);
    public void Update(Entities.User user);
}