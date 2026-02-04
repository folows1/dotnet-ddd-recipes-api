using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repos.User;

namespace MyRecipeBook.Infra.DataAccess.Repos;

public class UserRepo(MyRecipeBookDbContext dbCtx) : IUserReadOnlyRepo, IUserWriteOnlyRepo
{
    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        return await dbCtx.Users.AnyAsync(user => user.Email.Equals(email) && user.Active);
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
}