using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeValidator : AbstractValidator<RequestFilterRecipeJson>
{
    public FilterRecipeValidator()
    {
        RuleForEach(r => r.CookingTimes).IsInEnum().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleForEach(r => r.Difficulties).IsInEnum().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleForEach(r => r.DishTypes).IsInEnum().WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}