using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update;

public class UpdateUserUseCase(
    ILoggedUser loggedUser,
    IUserReadOnlyRepo readOnlyRepo,
    IUnitOfWork unitOfWork,
    IUserUpdateOnlyRepo updateRepo)
    : IUpdateUserUseCase
{
    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser1 = await loggedUser.User();

        await Validate(request, loggedUser1.Email);

        var user = await updateRepo.GetById(loggedUser1.Id);

        user.Name = request.Name;
        user.Email = request.Email;

        updateRepo.Update(user);
        await unitOfWork.Commit();
    }

    private async Task Validate(RequestUpdateUserJson request, string currentEmail)
    {
        var validator = new UpdateUserValidator();

        var result = await validator.ValidateAsync(request);

        if (currentEmail.Equals(request.Email).IsFalse())
        {
            var userExist = await readOnlyRepo.ExistActiveUserWithEmail(request.Email);
            if (userExist)
                result.Errors.Add(
                    new FluentValidation.Results.ValidationFailure("email", ResourceMessagesException.NAME_EMPTY));
        }

        if (result.IsValid.IsFalse())
        {
            var errorMessages = result.Errors.Select(err => err.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}