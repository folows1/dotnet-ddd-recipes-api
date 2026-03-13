using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe;

public class RecipeValidator : AbstractValidator<RequestRecipeJson>
{
    public RecipeValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage(ResourceMessagesException.TITLE_EMPTY);
        RuleFor(r => r.CookingTime).IsInEnum().WithMessage(ResourceMessagesException.COOKING_TIME_INVALID);
        RuleFor(r => r.Difficulty).IsInEnum().WithMessage(ResourceMessagesException.DIFFICULTY_INVALID);
        RuleFor(r => r.Ingredients.Count).GreaterThan(0).WithMessage(ResourceMessagesException.INGREDIENTS_EMPTY);
        RuleFor(r => r.Instructions.Count).GreaterThan(0).WithMessage(ResourceMessagesException.INSTRUCTIONS_EMPTY);
        RuleForEach(r => r.DishTypes).IsInEnum().WithMessage(ResourceMessagesException.DISH_TYPE_INVALID);
        RuleForEach(r => r.Ingredients).NotEmpty().WithMessage(ResourceMessagesException.INGREDIENT_EMPTY);
        RuleForEach(r => r.Instructions).ChildRules(instructionRule =>
        {
            instructionRule.RuleFor(instruction => instruction.Step).GreaterThan(0)
                .WithMessage(ResourceMessagesException.INSTRUCTION_STEP_INVALID);

            instructionRule.RuleFor(instruction => instruction.Text).NotEmpty()
                .WithMessage(ResourceMessagesException.INSTRUCTION_TEXT_EMPTY).MaximumLength(2000)
                .WithMessage(ResourceMessagesException.INSTRUCTION_TEXT_MAX_LENGTH_EXCEEDED);
        });
        RuleFor(recipe => recipe.Instructions).Must(instructions =>
                instructions.Select(i => i.Step).Distinct().Count() == instructions.Count)
            .WithMessage(ResourceMessagesException.INSTRUCTIONS_DUPLICATED_STEP);
    }
}