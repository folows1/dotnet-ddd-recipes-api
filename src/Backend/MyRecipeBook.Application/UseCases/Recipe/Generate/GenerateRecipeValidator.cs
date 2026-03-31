using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate;

public class GenerateRecipeValidator : AbstractValidator<RequestGenerateRecipeJson>
{
    public GenerateRecipeValidator()
    {
        var maxNumberIngredients = MyRecipeBookRuleConstants.MaximumNumberIngredients;

        RuleFor(request => request.Ingredients.Count).InclusiveBetween(1, maxNumberIngredients)
            .WithMessage(ResourceMessagesException.INGREDIENTS_EMPTY);

        RuleFor(request => request.Ingredients).Must(i => i.Count == i.Distinct().Count())
            .WithMessage(ResourceMessagesException.INGREDIENTS_EMPTY);

        RuleFor(request => request.Ingredients).ForEach(rule =>
        {
            rule.Custom((value, ctx) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ctx.AddFailure("Ingredient", ResourceMessagesException.INGREDIENT_EMPTY);
                    return;
                }

                if (value.Count(c => c == ' ') > 3 || value.Count(c => c == '/') > 1)
                {
                    ctx.AddFailure("Ingredient", ResourceMessagesException.DISH_TYPE_INVALID);
                    return;
                }
            });
        });
    }
}