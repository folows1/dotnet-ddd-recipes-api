using Moq;
using MyRecipeBook.Domain.Repos.Recipe;

namespace CommonTestUtils.Repos;

public class RecipeWriteOnlyRepoBuilder
{
    public static IRecipeWriteOnlyRepo Build()
    {
        var mock = new Mock<IRecipeWriteOnlyRepo>();
        return mock.Object;
    }
}