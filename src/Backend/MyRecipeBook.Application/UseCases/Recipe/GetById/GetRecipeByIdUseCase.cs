using System.Globalization;
using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
{
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepo _repo;

    public GetRecipeByIdUseCase(IMapper mapper, ILoggedUser loggedUser, IRecipeReadOnlyRepo repo)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _repo = repo;
    }

    public async Task<ResponseRecipeJson> Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _repo.GetById(loggedUser, recipeId);

        return recipe is null
            ? throw new CultureNotFoundException(ResourceMessagesException.NAME_EMPTY)
            : _mapper.Map<ResponseRecipeJson>(recipe);
    }
}