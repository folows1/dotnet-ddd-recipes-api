namespace MyRecipeBook.Application.UseCases.Login.ExternalLogin;

public interface IExternalLoginUseCase
{
    public Task<string> Execute(string name, string email);
}