using System.Net;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.Recipe.Generate;

public class GenerateRecipeInvalidTokenTest(CustomWebApplicationFactory fact) : MyRecipeBookClassFixture(fact)
{
    private const string Method = "recipe/generate";

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();
        var response = await DoPost(Method, request, "invalid");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Empty_Token()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();
        var response = await DoPost(Method, request, string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_NoUser()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPost(method: Method, request: request, token: token);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}