using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;

public class UpdateRecipeUseCase(
    IRecipeUpdateOnlyRepo repo,
    ILoggedUser user,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IUpdateRecipeUseCase
{
    public async Task Execute(long recipeId, RequestRecipeJson request)
    {
        Validate(request);

        var loggedUser = await user.User();

        var recipe = await repo.GetById(loggedUser, recipeId);

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesException.NAME_EMPTY);

        recipe.Ingredients.Clear();
        recipe.Instructions.Clear();
        recipe.DishTypes.Clear();

        mapper.Map(request, recipe);

        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
        for (var index = 0; index < instructions.Count; index++)
            instructions[index].Step = index + 1;

        recipe.Instructions = mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        repo.Update(recipe);

        await unitOfWork.Commit();
    }

    private static void Validate(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
    }
}