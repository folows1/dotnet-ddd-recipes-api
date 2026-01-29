namespace MyRecipeBook.Domain.Repos.User;

public interface IUserWriteOnlyRepo
{
  public Task Add(Entities.User user);

}
