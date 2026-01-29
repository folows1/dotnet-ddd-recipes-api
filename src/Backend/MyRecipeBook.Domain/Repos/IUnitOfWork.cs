namespace MyRecipeBook.Domain.Repos;

public interface IUnitOfWork
{
  public Task Commit();
}
