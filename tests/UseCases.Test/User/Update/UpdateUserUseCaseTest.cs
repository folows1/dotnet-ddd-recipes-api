using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repos;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.Update;

public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);
        var request = RequestUpdateUserJsonBuilder.Build();

        var act = async () => await useCase.Execute(request);
        await act.Should().NotThrowAsync();

        user.Name.Should().Be(request.Name);
        user.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesException.NAME_EMPTY));

        user.Name.Should().NotBe(request.Name);
        user.Email.Should().NotBe(request.Email);
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user, request.Email);

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED));

        user.Name.Should().NotBe(request.Name);
        user.Email.Should().NotBe(request.Email);
    }


    private static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userUpdateRepo = new UserUpdateOnlyRepoBuilder().GetById(user).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userReadOnlyRepoBuilder = new UserReadOnlyRepoBuilder();

        if (string.IsNullOrEmpty(email).IsFalse())
            userReadOnlyRepoBuilder.ExistsActiveUserWithEmail(email!);

        return new UpdateUserUseCase(loggedUser, userReadOnlyRepoBuilder.Build(), unitOfWork, userUpdateRepo);
    }
}