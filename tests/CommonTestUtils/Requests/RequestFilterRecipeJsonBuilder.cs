using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RequestFilterRecipeJsonBuilder
{
    public static RequestFilterRecipeJson Build()
    {
        return new Faker<RequestFilterRecipeJson>()
            .RuleFor(user => user.CookingTimes, faker => faker.Make(1, faker.PickRandom<CookingTime>))
            .RuleFor(user => user.Difficulties, faker => faker.Make(1, faker.PickRandom<Difficulty>))
            .RuleFor(user => user.DishTypes, faker => faker.Make(1, faker.PickRandom<DishType>))
            .RuleFor(user => user.RecipeTitleOrIngredient, faker => faker.Lorem.Word());
    }
}