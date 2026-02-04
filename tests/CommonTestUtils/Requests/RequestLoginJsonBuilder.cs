using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

// https://github.com/bchavez/Bogus

public static class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build(int pwdLength = 10)
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(user => user.Email, f => f.Internet.Email())
            .RuleFor(user => user.Password, f => f.Internet.Password(pwdLength));
    }
}