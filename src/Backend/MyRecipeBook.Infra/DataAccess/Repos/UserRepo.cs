using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.User;

namespace MyRecipeBook.Infra.DataAccess.Repos;

public class UserRepo(MyRecipeBookDbContext dbCtx) : IUserReadOnlyRepo, IUserWriteOnlyRepo, IUserUpdateOnlyRepo
{
    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        return await dbCtx.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);
    }

    public async Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier)
    {
        return await dbCtx.Users.AnyAsync(user => user.Active && user.UserIdentifier.Equals(userIdentifier));
    }

    public async Task<User?> GetByEmailAndPassword(string email, string password)
    {
        return await dbCtx.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user =>
                user.Active && user.Email.Equals(email) && user.Password.Equals(password)
            );
    }

    public async Task Add(User user)
    {
        await dbCtx.Users.AddAsync(user);
    }

    public async Task<User> GetById(long id)
    {
        return await dbCtx.Users.FirstAsync(user => user.Id == id);
    }

    public void Update(User user)
    {
        dbCtx.Users.Update(user);
    }
}