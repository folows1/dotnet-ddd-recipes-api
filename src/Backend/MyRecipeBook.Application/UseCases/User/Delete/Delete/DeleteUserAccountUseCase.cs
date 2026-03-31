using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.User.Delete.Delete;

public class DeleteUserAccountUseCase : IDeleteUserAccountUseCase
{
    private readonly IUserDeleteOnlyRepo _userDeleteOnlyRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteUserAccountUseCase(IUserDeleteOnlyRepo userDeleteOnlyRepo, IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _userDeleteOnlyRepo = userDeleteOnlyRepo;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task Execute(Guid userIdentifier)
    {
        await _blobStorageService.DeleteContainer(userIdentifier);
        await _userDeleteOnlyRepo.DeleteAccount(userIdentifier);
        await _unitOfWork.Commit();
    }
}