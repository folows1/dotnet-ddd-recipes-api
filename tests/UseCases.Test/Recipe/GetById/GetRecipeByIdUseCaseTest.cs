using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repos;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exceptions;

namespace UseCases.Test.Recipe.GetById;

public class GetRecipeByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var result = await useCase.Execute(recipe.Id);

        result.Should().NotBeNull();
        result.Title.Should().Be(recipe.Title);
        result.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Recipe_NotFound()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(recipeId: 1000);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.Message.Equals(ResourceMessagesException.NAME_EMPTY));
    }

    private static GetRecipeByIdUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null
    )
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repo = new RecipeReadOnlyRepoBuilder().GetById(user, recipe).Build();

        return new GetRecipeByIdUseCase(mapper, loggedUser, repo);
    }
}