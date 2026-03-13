using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtils.Crypto;
using CommonTestUtils.Tokens;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Delete;

public class DeleteRecipeTest : MyRecipeBookClassFixture
{
    private const string Method = "recipe";
    private readonly Guid _userId;
    private readonly string _recipeId;

    public DeleteRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userId = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);
        var response = await DoDelete($"{Method}/{_recipeId}", token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response = await DoGet($"{Method}/{_recipeId}", token);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Recipe_NotFound(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);
        var id = IdEncripterBuilder.Build().Encode(1000);

        var response = await DoDelete($"{Method}/{id}", token, culture);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage =
            ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));
    }
}