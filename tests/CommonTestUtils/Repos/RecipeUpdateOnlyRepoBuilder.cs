using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.Recipe;

namespace CommonTestUtils.Repos;

public class RecipeUpdateOnlyRepoBuilder
{
    private readonly Mock<IRecipeUpdateOnlyRepo> _repo;

    public RecipeUpdateOnlyRepoBuilder() => _repo = new Mock<IRecipeUpdateOnlyRepo>();

    public RecipeUpdateOnlyRepoBuilder GetById(User user, Recipe? recipe)
    {
        if (recipe is not null)
            _repo.Setup(repo => repo.GetById(user, recipe.Id)).ReturnsAsync(recipe);

        return this;
    }

    public IRecipeUpdateOnlyRepo Build() => _repo.Object;
}