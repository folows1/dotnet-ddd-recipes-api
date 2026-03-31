using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RequestGenerateRecipeJsonBuilder
{
    public static RequestGenerateRecipeJson Build(int count = 5)
    {
        return new Faker<RequestGenerateRecipeJson>()
            .RuleFor(user => user.Ingredients, f => f.Make(count, () => f.Commerce.ProductName()));
    }
}