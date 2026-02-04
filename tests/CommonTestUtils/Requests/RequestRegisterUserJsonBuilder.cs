using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

// https://github.com/bchavez/Bogus

public static class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build(int pwdLength = 10)
    {
        return new Faker<RequestRegisterUserJson>()
            .RuleFor(user => user.Name, f => f.Person.FirstName)
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
            .RuleFor(user => user.Password, f => f.Internet.Password(pwdLength));
    }
}