namespace MyRecipeBook.Domain.Repos.Recipe;

public interface IRecipeUpdateOnlyRepo
{
    Task<Entities.Recipe?> GetById(Entities.User user, long recipeId);
    void Update(Entities.Recipe recipe);
}