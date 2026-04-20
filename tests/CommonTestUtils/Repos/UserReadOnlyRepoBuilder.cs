using Moq;
using MyRecipeBook.Domain.Entities;
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

    public void GetByEmail(User user)
    {
        _repo.Setup(repo => repo.GetByEmail(user.Email)).ReturnsAsync(user);
    }

    public IUserReadOnlyRepo Build()
    {
        return _repo.Object;
    }
}