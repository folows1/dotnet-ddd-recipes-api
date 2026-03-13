using CommonTestUtils.Crypto;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repos;
using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = password;

        var useCase = CreateUseCase(user);

        var action = async () => await useCase.Execute(request);

        await action.Should().NotThrowAsync();

        var passwordEncripter = PasswordEncripterBuilder.Build();

        user.Password.Should().Be(passwordEncripter.Encrypt(request.NewPassword));
    }

    [Fact]
    public async Task Error_NewPwd_Empty()
    {
        var (user, password) = UserBuilder.Build();

        var request = new RequestChangePasswordJson
        {
            Password = password,
            NewPassword = string.Empty
        };

        var useCase = CreateUseCase(user);

        var action = async () => await useCase.Execute(request);

        (await action.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesException.PASSWORD_EMPTY));

        var passwordEncripter = PasswordEncripterBuilder.Build();

        user.Password.Should().Be(passwordEncripter.Encrypt(password));
    }

    [Fact]
    public async Task Error_Password_Diff()
    {
        var (user, password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var action = async () => await useCase.Execute(request);

        (await action.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesException.CURRENT_PASSWORD_INVALID));

        var passwordEncripter = PasswordEncripterBuilder.Build();

        user.Password.Should().Be(passwordEncripter.Encrypt(password));
    }

    private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userUpdateRepo = new UserUpdateOnlyRepoBuilder().GetById(user).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var pwdEncripter = PasswordEncripterBuilder.Build();

        return new ChangePasswordUseCase(loggedUser, userUpdateRepo, unitOfWork, pwdEncripter);
    }
}