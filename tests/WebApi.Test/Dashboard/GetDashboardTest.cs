using System.Net;
using System.Text.Json;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace WebApi.Test.Dashboard;

public class GetDashboardTest : MyRecipeBookClassFixture
{
    private const string Method = "dashboard";
    private readonly Guid _userId;

    public GetDashboardTest(CustomWebApplicationFactory factory) : base(factory)
    {
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

        responseData.RootElement.GetProperty("recipes").GetArrayLength().Should().BeGreaterThan(0);
    }
}