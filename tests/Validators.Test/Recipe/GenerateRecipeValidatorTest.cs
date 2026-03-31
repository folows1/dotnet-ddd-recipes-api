using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Domain.ValueObjects;

namespace Validators.Test.Recipe;

public class GenerateRecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new GenerateRecipeValidator();
        var request = RequestGenerateRecipeJsonBuilder.Build();
        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_More_Max_Ingredients()
    {
        var validator = new GenerateRecipeValidator();
        var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MaximumNumberIngredients + 1);
        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Error_Ingredients_Duplicated()
    {
        var validator = new GenerateRecipeValidator();
        var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MaximumNumberIngredients - 1);
        request.Ingredients.Add(request.Ingredients.First());
        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void Error_Empty_Ingredient(string ingredient)
    {
        var validator = new GenerateRecipeValidator();
        var request = RequestGenerateRecipeJsonBuilder.Build(count: 1);
        request.Ingredients.Add(ingredient);
        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Error_Ingredient_Not_In_The_Pattern()
    {
        var validator = new GenerateRecipeValidator();
        var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MaximumNumberIngredients - 1);
        request.Ingredients.Add("This is a invalid ingredient because is too long!");
        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}