using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase : IDeleteRecipeUseCase
{
    private readonly IRecipeReadOnlyRepo _readOnlyRepo;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeWriteOnlyRepo _repoWrite;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteRecipeUseCase(IRecipeReadOnlyRepo readOnlyRepo, ILoggedUser loggedUser, IRecipeWriteOnlyRepo repo,
        IUnitOfWork unitOfWork, IBlobStorageService blobStorageService)
    {
        _readOnlyRepo = readOnlyRepo;
        _loggedUser = loggedUser;
        _repoWrite = repo;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();
        var recipe = await _readOnlyRepo.GetById(loggedUser, recipeId);

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        if (recipe.ImageIdentifier.NotEmpty())
            await _blobStorageService.Delete(loggedUser, recipe.ImageIdentifier);

        await _repoWrite.Delete(recipeId);
        await _unitOfWork.Commit();
    }
}