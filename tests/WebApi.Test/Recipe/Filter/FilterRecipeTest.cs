using System.Net;
using System.Text.Json;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Enums;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Filter;

public class FilterRecipeTest : MyRecipeBookClassFixture
{
    private const string Method = "recipe/filter";
    private readonly Guid _userId;

    private readonly string _recipeTitle;
    private readonly Difficulty _recipeDifficulty;
    private readonly CookingTime _recipeCookingTime;
    private readonly IList<DishType> _recipeDishTypes;

    public FilterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userId = factory.GetUserIdentifier();

        _recipeTitle = factory.GetRecipeTitle();
        _recipeDifficulty = factory.GetRecipeDifficulty();
        _recipeCookingTime = factory.GetCookingTime();
        _recipeDishTypes = factory.GetDishTypes();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestFilterRecipeJson
        {
            CookingTimes = [(MyRecipeBook.Communication.Enums.CookingTime)_recipeCookingTime],
            Difficulties = [(MyRecipeBook.Communication.Enums.Difficulty)_recipeDifficulty],
            DishTypes = _recipeDishTypes.Select(dishType => (MyRecipeBook.Communication.Enums.DishType)dishType)
                .ToList(),
            RecipeTitleOrIngredient = _recipeTitle,
        };

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);

        var response = await DoPost(Method, request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("recipes").EnumerateArray().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Success_NoContent()
    {
        var request = RequestFilterRecipeJsonBuilder.Build();
        request.RecipeTitleOrIngredient = "recipe not found";

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);

        var response = await DoPost(Method, request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_CookingTimeInvalid(string culture)
    {
        var request = RequestFilterRecipeJsonBuilder.Build();
        request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userId);

        var response = await DoPost(Method, request, token: token, culture: culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}