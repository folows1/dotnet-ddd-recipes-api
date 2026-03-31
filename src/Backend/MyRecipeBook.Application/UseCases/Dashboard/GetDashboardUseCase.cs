using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.Dashboard;

public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly IRecipeReadOnlyRepo _recipeReadOnlyRepo;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IBlobStorageService _blobStorageService;

    public GetDashboardUseCase(IRecipeReadOnlyRepo recipeReadOnlyRepo, IMapper mapper, ILoggedUser loggedUser,
        IBlobStorageService blobStorageService)
    {
        _recipeReadOnlyRepo = recipeReadOnlyRepo;
        _mapper = mapper;
        _loggedUser = loggedUser;
        _blobStorageService = blobStorageService;
    }

    public async Task<ResponseRecipesJson> Execute()
    {
        var loggedUser = await _loggedUser.User();
        var recipes = await _recipeReadOnlyRepo.GetForDashboard(loggedUser);

        return new ResponseRecipesJson
        {
            Recipes = await recipes.MapToShortRecipeJson(loggedUser, _blobStorageService, _mapper)
        };
    }
}