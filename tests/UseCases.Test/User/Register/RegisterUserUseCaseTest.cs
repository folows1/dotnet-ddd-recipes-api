using CommonTestUtils.Crypto;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repos;
using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

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
    }

    [Fact]
    public async Task Error_Email_Already_Exists()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase(request.Email);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesException.NAME_EMPTY));
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesException.NAME_EMPTY));
    }

    private static RegisterUserUseCase CreateUseCase(string? email = null)
    {
        var mapper = MapperBuilder.Build();
        var crypto = PasswordEncripterBuilder.Build();
        var writeOnlyRepo = UserWriteOnlyRepoBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var readOnlyRepoBuilder = new UserReadOnlyRepoBuilder();

        if (!string.IsNullOrEmpty(email))
            readOnlyRepoBuilder.ExistsActiveUserWithEmail(email);

        return new RegisterUserUseCase(writeOnlyRepo, readOnlyRepoBuilder.Build(), unitOfWork, mapper, crypto);
    }
}