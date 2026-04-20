using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repos;
using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Recipe.Update;

public class UpdateRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id, request);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_RecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipeId: 1000, request);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.Message == ResourceMessagesException.RECIPE_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Title_Empty()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id, request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.TITLE_EMPTY)
            );
    }

    private static UpdateRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null
    )
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var repo = new RecipeUpdateOnlyRepoBuilder().GetById(user, recipe).Build();

        return new UpdateRecipeUseCase(repo, loggedUser, unitOfWork, mapper);
    }
}