using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.User.Update;

public interface IUpdateUserUseCase
{
    public Task Execute(RequestUpdateUserJson request);
}