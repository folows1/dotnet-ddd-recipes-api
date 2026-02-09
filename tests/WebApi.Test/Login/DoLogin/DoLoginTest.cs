using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login.DoLogin;

public class DoLoginTest(CustomWebApplicationFactory factory) : MyRecipeBookClassFixture(factory)
{
    private const string Method = "login";

    private readonly string _email = factory.GetEmail();
    private readonly string _pwd = factory.GetPassword();
    private readonly string _name = factory.GetName();

    [Fact]
    public async Task Success()
    {
        var request = new RequestLoginJson()
        {
            Email = _email,
            Password = _pwd
        };

        var response = await DoPost(Method, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace().And.Be(_name);
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().Should()
            .NotBeNullOrEmpty();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Login(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();

        var response = await DoPost(Method, request, culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage =
            ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(expectedMessage));
    }
}