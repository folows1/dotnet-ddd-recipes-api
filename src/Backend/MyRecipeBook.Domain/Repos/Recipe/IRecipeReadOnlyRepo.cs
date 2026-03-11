using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repos.Recipe;

public interface IRecipeReadOnlyRepo
{
    Task<IList<Entities.Recipe>> Filter(Entities.User user, FilterRecipesDto filters);
    Task<Entities.Recipe?> GetById(Entities.User user, long recipeId);
}