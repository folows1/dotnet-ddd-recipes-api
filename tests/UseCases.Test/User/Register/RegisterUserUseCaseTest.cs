using CommonTestUtils.Crypto;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repos;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using Moq;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Domain.Repos.RefreshToken;
using MyRecipeBook.Infra.Security.Tokens.Refresh;

namespace UseCases.Test.User.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var useCase = CreateUseCase();
        var request = RequestRegisterUserJsonBuilder.Build();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Tokens.Should().NotBeNull();
        result.Tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Email_Already_Exists()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase(request.Email);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesException
                .EMAIL_ALREADY_REGISTERED));
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.NAME_EMPTY));
    }

    private static RegisterUserUseCase CreateUseCase(string? email = null)
    {
        var mapper = MapperBuilder.Build();
        var crypto = PasswordEncripterBuilder.Build();
        var writeOnlyRepo = UserWriteOnlyRepoBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var readOnlyRepoBuilder = new UserReadOnlyRepoBuilder();
        var accessTokenGen = JwtTokenGeneratorBuilder.Build();
        var refreshTokenGen = new RefreshTokenGenerator();
        var tokenRepo = new Mock<ITokenRepo>();
        tokenRepo.Setup(repo => repo.SaveNewRefreshToken(It.IsAny<MyRecipeBook.Domain.Entities.RefreshToken>()))
            .Returns(Task.CompletedTask);


        if (!string.IsNullOrEmpty(email))
            readOnlyRepoBuilder.ExistsActiveUserWithEmail(email);

        return new RegisterUserUseCase(
            writeOnlyRepo,
            readOnlyRepoBuilder.Build(),
            unitOfWork,
            mapper,
            accessTokenGen,
            refreshTokenGen,
            tokenRepo.Object,
            crypto);
    }
}