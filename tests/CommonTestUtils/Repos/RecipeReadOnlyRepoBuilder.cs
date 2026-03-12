using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.Recipe;

namespace CommonTestUtils.Repos;

public class RecipeReadOnlyRepoBuilder
{
    private readonly Mock<IRecipeReadOnlyRepo> _repo;

    public RecipeReadOnlyRepoBuilder() => _repo = new Mock<IRecipeReadOnlyRepo>();

    public RecipeReadOnlyRepoBuilder Filter(User user, IList<Recipe> recipes)
    {
        _repo.Setup(repo => repo.Filter(user, It.IsAny<FilterRecipesDto>())).ReturnsAsync(recipes);

        return this;
    }

    public RecipeReadOnlyRepoBuilder GetById(User user, Recipe? recipe)
    {
        if (recipe is not null)
            _repo.Setup(repo => repo.GetById(user, recipe.Id)).ReturnsAsync(recipe);

        return this;
    }

    public IRecipeReadOnlyRepo Build() => _repo.Object;
}