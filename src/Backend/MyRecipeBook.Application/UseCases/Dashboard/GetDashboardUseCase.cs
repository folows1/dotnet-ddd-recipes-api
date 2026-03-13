using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;

namespace MyRecipeBook.Application.UseCases.Dashboard;

public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly IRecipeReadOnlyRepo _recipeReadOnlyRepo;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;

    public GetDashboardUseCase(IRecipeReadOnlyRepo recipeReadOnlyRepo, IMapper mapper, ILoggedUser loggedUser)
    {
        _recipeReadOnlyRepo = recipeReadOnlyRepo;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseRecipesJson> Execute()
    {
        var loggedUser = await _loggedUser.User();
        var recipes = await _recipeReadOnlyRepo.GetForDashboard(loggedUser);

        return new ResponseRecipesJson
        {
            Recipes = _mapper.Map<IList<ResponseShortRecipeJson>>(recipes)
        };
    }
}