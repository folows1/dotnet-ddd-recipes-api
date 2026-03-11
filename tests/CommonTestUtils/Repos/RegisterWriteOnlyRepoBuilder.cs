using Moq;
using MyRecipeBook.Domain.Repos.Recipe;

namespace CommonTestUtils.Repos;

public class RegisterWriteOnlyRepoBuilder
{
    public static IRecipeWriteOnlyRepo Build()
    {
        var mock = new Mock<IRecipeWriteOnlyRepo>();
        return mock.Object;
    }
}