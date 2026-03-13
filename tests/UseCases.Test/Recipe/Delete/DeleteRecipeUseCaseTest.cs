using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repos;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Exceptions;

namespace UseCases.Test.Recipe.Delete;

public class DeleteRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_RecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(recipeId: 1000);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.Message.Equals(ResourceMessagesException.NAME_EMPTY));
    }

    private static DeleteRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null
    )
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repo = new RecipeReadOnlyRepoBuilder().GetById(user, recipe).Build();
        var repoWrite = RecipeWriteOnlyRepoBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new DeleteRecipeUseCase(repo, loggedUser, repoWrite, unitOfWork);
    }
}