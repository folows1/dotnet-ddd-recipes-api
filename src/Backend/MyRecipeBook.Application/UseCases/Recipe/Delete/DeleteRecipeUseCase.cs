using AutoMapper;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase : IDeleteRecipeUseCase
{
    private readonly IRecipeReadOnlyRepo _readOnlyRepo;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeWriteOnlyRepo _repoWrite;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRecipeUseCase(IRecipeReadOnlyRepo readOnlyRepo, ILoggedUser loggedUser, IRecipeWriteOnlyRepo repo,
        IUnitOfWork unitOfWork)
    {
        _readOnlyRepo = readOnlyRepo;
        _loggedUser = loggedUser;
        _repoWrite = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();
        var recipe = await _readOnlyRepo.GetById(loggedUser, recipeId);

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesException.NAME_EMPTY);

        await _repoWrite.Delete(recipeId);
        await _unitOfWork.Commit();
    }
}