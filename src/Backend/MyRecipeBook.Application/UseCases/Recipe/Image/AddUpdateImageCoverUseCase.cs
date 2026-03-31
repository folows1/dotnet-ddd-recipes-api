using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Image;

public class AddUpdateImageCoverUseCase : IAddUpdateImageCoverUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepo _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public AddUpdateImageCoverUseCase(ILoggedUser loggedUser, IRecipeUpdateOnlyRepo repo, IUnitOfWork unitOfWork,
        IBlobStorageService blob)
    {
        _loggedUser = loggedUser;
        _repo = repo;
        _unitOfWork = unitOfWork;
        _blobStorageService = blob;
    }

    public async Task Execute(long recipeId, IFormFile file)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _repo.GetById(loggedUser, recipeId);

        if (recipe is null) throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        var fileStream = file.OpenReadStream();

        var (isValidImage, extension) = fileStream.ValidateAndGetImageExtension();

        if (isValidImage.IsFalse())
            throw new ErrorOnValidationException([ResourceMessagesException.TOKEN_INVALID]);

        if (string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";
            _repo.Update(recipe);
            await _unitOfWork.Commit();
        }

        await _blobStorageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
    }
}