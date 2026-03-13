using CommonTestUtils.Crypto;
using CommonTestUtils.Entities;
using CommonTestUtils.Repos;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Login.DoLogin;

public class DoLoginUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, pwd) = UserBuilder.Build();
        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(new RequestLoginJson
        {
            Password = pwd,
            Email = user.Email
        });

        result.Should().NotBeNull();
        result.Tokens.Should().NotBeNull();
        result.Name.Should().NotBeNullOrWhiteSpace().And.Be(user.Name);
        result.Tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Invalid_User()
    {
        var request = RequestLoginJsonBuilder.Build();
        var useCase = CreateUseCase();

        var act = async () => { await useCase.Execute(request); };

        await act.Should().ThrowAsync<InvalidLoginException>()
            .Where(e => e.Message.Equals(ResourceMessagesException.INVALID_LOGIN));
    }

    private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
    {
        var pwdEncrypter = PasswordEncripterBuilder.Build();
        var repoBuilder = new UserReadOnlyRepoBuilder();
        var accessTokenGen = JwtTokenGeneratorBuilder.Build();

        if (user is not null)
            repoBuilder.GetByEmailAndPassword(user);

        return new DoLoginUseCase(repoBuilder.Build(), pwdEncrypter, accessTokenGen);
    }
}