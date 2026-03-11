namespace MyRecipeBook.Domain.Repos.Recipe;

public interface IRecipeWriteOnlyRepo
{
    public Task Add(Entities.Recipe recipe);
}