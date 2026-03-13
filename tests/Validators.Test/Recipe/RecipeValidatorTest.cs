using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;

namespace Validators.Test.Recipe;

public class RecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_Invalid_Cooking_Time()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.CookingTime = (CookingTime?)1000;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.COOKING_TIME_INVALID));
    }

    [Fact]
    public void Error_Invalid_Difficulty()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Difficulty = (Difficulty?)1000;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.DIFFICULTY_INVALID));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("      ")]
    [InlineData("")]
    public void Error_Invalid_Title(string stringTitle)
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = stringTitle;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.TITLE_EMPTY));
    }

    [Fact]
    public void Success_DishTypes_Empty()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes.Clear();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_DishTypes_Invalid()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes.Add((DishType)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.DISH_TYPE_INVALID));
    }

    [Fact]
    public void Error_Empty_Ingredients()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients.Clear();

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.INGREDIENTS_EMPTY));
    }

    [Fact]
    public void Error_Empty_Instructions()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.Clear();

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.INSTRUCTIONS_EMPTY));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("      ")]
    [InlineData("")]
    public void Error_Empty_Value_Ingredients(string ingredient)
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients.Add(ingredient);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.INGREDIENT_EMPTY));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("      ")]
    [InlineData("")]
    public void Error_Empty_Value_Instructions(string instruction)
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Text = instruction;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.INSTRUCTION_TEXT_EMPTY));
    }
}