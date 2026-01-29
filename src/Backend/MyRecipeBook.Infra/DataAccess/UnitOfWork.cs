using System;
using MyRecipeBook.Domain.Repos;

namespace MyRecipeBook.Infra.DataAccess;

public class UnitOfWork : IUnitOfWork
{
  private readonly MyRecipeBookDbContext _dbContext;

  public UnitOfWork(MyRecipeBookDbContext dbCtx)
  {
    _dbContext = dbCtx;
  }

  public async Task Commit() => await _dbContext.SaveChangesAsync();
}
