using CommonTestUtils.Dtos;
using CommonTestUtils.OpenAI;
using CommonTestUtils.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Recipe.Generate;

public class GenerateRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var dto = GeneratedRecipeDtoBuilder.Build();
        var request = RequestGenerateRecipeJsonBuilder.Build();
        var useCase = CreateUseCase(dto);
        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Difficulty.Should().Be(Difficulty.Low);
    }

    [Fact]
    public async Task Error_Dup_Ingredients()
    {
        var dto = GeneratedRecipeDtoBuilder.Build();
        var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MaximumNumberIngredients - 1);
        request.Ingredients.Add(request.Ingredients[0]);
        var useCase = CreateUseCase(dto);

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1);
    }

    private static GenerateRecipeUseCase CreateUseCase(GeneratedRecipeDto dto)
    {
        var generateRecipeAI = GenerateRecipeAIBuilder.Build(dto);
        return new GenerateRecipeUseCase(generateRecipeAI);
    }
}