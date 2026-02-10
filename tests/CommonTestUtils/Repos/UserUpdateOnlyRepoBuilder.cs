using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.User;

namespace CommonTestUtils.Repos;

public class UserUpdateOnlyRepoBuilder
{
    private readonly Mock<IUserUpdateOnlyRepo> _repo;

    public UserUpdateOnlyRepoBuilder()
    {
        _repo = new Mock<IUserUpdateOnlyRepo>();
    }

    public UserUpdateOnlyRepoBuilder GetById(User user)
    {
        _repo.Setup(x => x.GetById(user.Id)).ReturnsAsync(user);
        return this;
    }

    public IUserUpdateOnlyRepo Build() => _repo.Object;
}