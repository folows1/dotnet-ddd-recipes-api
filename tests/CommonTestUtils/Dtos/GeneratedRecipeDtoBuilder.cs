using Bogus;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtils.Dtos;

public class GeneratedRecipeDtoBuilder
{
    public static GeneratedRecipeDto Build()
    {
        return new Faker<GeneratedRecipeDto>()
            .RuleFor(r => r.Title, f => f.Lorem.Word())
            .RuleFor(r => r.CookingTime, f => f.PickRandom<CookingTime>())
            .RuleFor(r => r.Ingredients, f => f.Make(1, () => f.Commerce.ProductName()))
            .RuleFor(r => r.Instructions, faker => faker.Make(1, () => new GeneratedInstructionsDto
            {
                Step = 1,
                Text = faker.Lorem.Paragraph()
            }));
    }
}