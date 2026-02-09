using Bogus;
using CommonTestUtils.Crypto;
using MyRecipeBook.Domain.Entities;

namespace CommonTestUtils.Entities;

public static class UserBuilder
{
    public static (User user, string pwd) Build()
    {
        var pwdEncrypter = PasswordEncripterBuilder.Build();

        var pwd = new Faker().Internet.Password();

        var user = new Faker<User>()
            .RuleFor(user => user.Id, () => 1)
            .RuleFor(user => user.Name, f => f.Person.FirstName)
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
            .RuleFor(user => user.UserIdentifier, _ => Guid.NewGuid())
            .RuleFor(user => user.Password, f => pwdEncrypter.Encrypt(pwd));

        return (user, pwd);
    }
}