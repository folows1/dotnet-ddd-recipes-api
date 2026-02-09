using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Services;

public interface ILoggedUser
{
    public Task<User> User();
}