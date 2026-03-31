using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Application.UseCases.User.Delete.Request;

public class RequestDeleteUserUseCase : IRequestDeleteUserUseCase
{
    private readonly IDeleteUserQueue _queue;
    private readonly IUserUpdateOnlyRepo _userRepo;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public RequestDeleteUserUseCase(IDeleteUserQueue queue, IUserUpdateOnlyRepo userRepo, ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _queue = queue;
        _userRepo = userRepo;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute()
    {
        var loggedUser = await _loggedUser.User();
        var user = await _userRepo.GetById(loggedUser.Id);

        user.Active = false;
        _userRepo.Update(user);

        await _unitOfWork.Commit();

        await _queue.SendMessage(loggedUser);
    }
}