using Moq;
using MyRecipeBook.Domain.Repos.User;

namespace CommonTestUtils.Repos;

public class UserWriteOnlyRepoBuilder
{
    public static IUserWriteOnlyRepo Build()
    {
        var mock = new Mock<IUserWriteOnlyRepo>();

        return mock.Object;
    }
}