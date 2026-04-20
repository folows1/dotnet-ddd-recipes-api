using CommonTestUtils.BlobStorage;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repos;
using CommonTestUtils.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using UseCases.Test.InlineDatas;

namespace UseCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRegisterRecipeFormDataBuilder.Build();

        var useCase = CreateUseCase(user);
        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success_With_Image(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRegisterRecipeFormDataBuilder.Build(file);

        var useCase = CreateUseCase(user);
        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task Error_Title_Empty()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRegisterRecipeFormDataBuilder.Build();
        request.Title = string.Empty;

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.TITLE_EMPTY)
            );
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repo = RecipeWriteOnlyRepoBuilder.Build();
        var blobStorage = new BlobStorageServiceBuilder().Build();

        return new RegisterRecipeUseCase(repo, loggedUser, unitOfWork, mapper, blobStorage);
    }
}