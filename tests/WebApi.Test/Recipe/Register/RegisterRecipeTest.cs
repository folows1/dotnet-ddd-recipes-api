using System.Net;
using System.Text.Json;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.Recipe.Register;

public class RegisterRecipeTest : MyRecipeBookClassFixture
{
    private const string Method = "recipe";
    private readonly Guid _userId;

    public RegisterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userId = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);

        var response = await DoPost(Method, request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
        responseData.RootElement.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();
    }
}