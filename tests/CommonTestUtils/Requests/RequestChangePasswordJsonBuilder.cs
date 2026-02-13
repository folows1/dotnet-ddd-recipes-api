using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int pwdLength = 10)
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(u => u.Password, (f) => f.Internet.Password())
            .RuleFor(u => u.NewPassword, (f) => f.Internet.Password(pwdLength));
    }
}