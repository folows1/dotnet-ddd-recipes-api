using System.Net;
using System.Text.Json;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.Recipe.Update;

public class UpdateRecipeTest : MyRecipeBookClassFixture
{
    private const string Method = "recipe";
    private readonly Guid _userId;
    private readonly string _recipeId;

    public UpdateRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userId = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);

        var response = await DoPut($"{Method}/{_recipeId}", request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}