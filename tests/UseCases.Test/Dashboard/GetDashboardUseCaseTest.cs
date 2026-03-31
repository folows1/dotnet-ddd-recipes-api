using CommonTestUtils.BlobStorage;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repos;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Dashboard;

namespace UseCases.Test.Dashboard;

public class GetDashboardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute();

        result.Should().NotBeNull();
        result.Recipes.Should().HaveCountGreaterThan(0)
            .And.OnlyHaveUniqueItems(r => r.Id)
            .And.AllSatisfy(r =>
            {
                r.Id.Should().NotBeNullOrWhiteSpace();
                r.Title.Should().NotBeNullOrWhiteSpace();
                r.AmountIngredients.Should().BeGreaterThan(0);
                r.ImageUrl.Should().NotBeNullOrWhiteSpace();
            });
    }

    private static GetDashboardUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        IList<MyRecipeBook.Domain.Entities.Recipe> recipes
    )
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repo = new RecipeReadOnlyRepoBuilder().GetForDashboard(user, recipes).Build();
        var blobStorage = new BlobStorageServiceBuilder().GetImageUrl(user, recipes).Build();

        return new GetDashboardUseCase(repo, mapper, loggedUser, blobStorage);
    }
}