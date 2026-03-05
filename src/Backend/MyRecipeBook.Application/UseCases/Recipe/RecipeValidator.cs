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
    }
}