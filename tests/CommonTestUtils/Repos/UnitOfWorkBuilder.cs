using Moq;
using MyRecipeBook.Domain.Repos;

namespace CommonTestUtils.Repos;

public class UnitOfWorkBuilder
{
    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();

        return mock.Object;
    }
}