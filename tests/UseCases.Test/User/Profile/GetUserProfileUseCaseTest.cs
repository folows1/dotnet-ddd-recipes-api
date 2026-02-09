using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Profile;

namespace UseCases.Test.User.Profile;

public class GetUserProfileUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute();

        result.Should().NotBeNull();
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    private static GetUserProfileUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new GetUserProfileUseCase(loggedUser, mapper);
    }
}