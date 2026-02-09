using System.Net;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.User.Profile;

public class GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : MyRecipeBookClassFixture(factory)
{
    private const string Method = "user";

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await DoGet(Method, token: "invalid-token");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await DoGet(Method, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoGet(Method, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}