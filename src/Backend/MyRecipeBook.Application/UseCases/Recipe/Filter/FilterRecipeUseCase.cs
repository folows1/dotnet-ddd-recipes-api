using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeUseCase : IFilterRecipeUseCase
{
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepo _repo;

    public FilterRecipeUseCase(IMapper mapper, ILoggedUser loggedUser, IRecipeReadOnlyRepo repo)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _repo = repo;
    }

    public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.User();

        var filters = new Domain.Dtos.FilterRecipesDto
        {
            RecipeTitleOrIngredient = request.RecipeTitleOrIngredient,
            CookingTimes = request.CookingTimes.Distinct().Select(c => (Domain.Enums.CookingTime)c).ToList(),
            Difficulties = request.Difficulties.Distinct().Select(c => (Domain.Enums.Difficulty)c).ToList(),
            DishTypes = request.DishTypes.Distinct().Select(c => (Domain.Enums.DishType)c).ToList(),
        };

        var recipes = await _repo.Filter(loggedUser, filters);

        return new ResponseRecipesJson
        {
            Recipes = _mapper.Map<List<ResponseShortRecipeJson>>(recipes)
        };
    }

    private static void Validate(RequestFilterRecipeJson request)
    {
        var validator = new FilterRecipeValidator();
        var result = validator.Validate(request);

        if (result.IsValid) return;

        var errMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
        throw new ErrorOnValidationException(errMessages);
    }
}