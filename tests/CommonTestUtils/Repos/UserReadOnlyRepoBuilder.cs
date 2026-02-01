using Moq;
using MyRecipeBook.Domain.Repos.User;

namespace CommonTestUtils.Repos;

public class UserReadOnlyRepoBuilder
{
    private readonly Mock<IUserReadOnlyRepo> _repo;
    
    public UserReadOnlyRepoBuilder()
    {
        _repo = new Mock<IUserReadOnlyRepo>();
    }

    public void ExistsActiveUserWithEmail(string email)
    {
        _repo.Setup(repo => repo.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
    }

    public IUserReadOnlyRepo Build()
    {
        return _repo.Object;
    }
}