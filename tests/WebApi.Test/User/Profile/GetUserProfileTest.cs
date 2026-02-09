using System.Net;
using System.Text.Json;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.User.Profile;

public class GetUserProfileTest : MyRecipeBookClassFixture
{
    private const string Method = "user";

    private readonly string _email;
    private readonly string _name;
    private readonly Guid _userId;

    public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.GetEmail();
        _name = factory.GetName();
        _userId = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);

        var response = await DoGet(Method, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace().And.Be(_name);
        responseData.RootElement.GetProperty("email").GetString().Should().NotBeNullOrWhiteSpace().And.Be(_email);
    }
}