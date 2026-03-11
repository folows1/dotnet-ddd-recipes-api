using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe;

public class RecipeValidator : AbstractValidator<RequestRecipeJson>
{
    public RecipeValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(r => r.CookingTime).IsInEnum().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(r => r.Difficulty).IsInEnum().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(r => r.Ingredients.Count).GreaterThan(0).WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(r => r.Instructions.Count).GreaterThan(0).WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleForEach(r => r.DishTypes).IsInEnum().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleForEach(r => r.Ingredients).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleForEach(r => r.Instructions).ChildRules(instructionRule =>
        {
            instructionRule.RuleFor(instruction => instruction.Step).GreaterThan(0)
                .WithMessage(ResourceMessagesException.NAME_EMPTY);

            instructionRule.RuleFor(instruction => instruction.Text).NotEmpty()
                .WithMessage(ResourceMessagesException.NAME_EMPTY).MaximumLength(2000)
                .WithMessage(ResourceMessagesException.NAME_EMPTY);
        });
        RuleFor(recipe => recipe.Instructions).Must(instructions =>
                instructions.Select(i => i.Step).Distinct().Count() == instructions.Count)
            .WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}