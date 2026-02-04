using MyRecipeBook.Application.Services.Crypto;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;

public class DoLoginUseCase(IUserReadOnlyRepo repo, PasswordEncripter passwordEncripter)
    : IDoLoginUseCase
{
    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var pwd = passwordEncripter.Encrypt(request.Password);

        var user = await repo.GetByEmailAndPassword(request.Email, pwd);

        if (user is null)
            throw new InvalidLoginException();

        return new ResponseRegisteredUserJson
        {
            Name = user.Name
        };
    }
}