using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.User;

namespace MyRecipeBook.Infra.DataAccess.Repos;

public class UserRepo : IUserReadOnlyRepo, IUserWriteOnlyRepo
{
  private readonly MyRecipeBookDbContext _dbContext;

  public UserRepo(MyRecipeBookDbContext dbCtx)
  {
    _dbContext = dbCtx;
  }

  public async Task Add(User user) => await _dbContext.Users.AddAsync(user);

  public async Task<bool> ExistActiveUserWithEmail(string email)
  {
    return await _dbContext.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);
  }
}
