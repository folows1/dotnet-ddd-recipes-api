using System.Net;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.User.Update;

public class UpdateUserInvalidTokenTest(CustomWebApplicationFactory factory) : MyRecipeBookClassFixture(factory)
{
    private const string Method = "user";

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await DoPut(Method, request, "token");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_Empty()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await DoPut(Method, request, string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_Invalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPut(Method, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}