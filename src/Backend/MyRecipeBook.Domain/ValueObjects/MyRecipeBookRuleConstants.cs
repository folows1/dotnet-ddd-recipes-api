namespace MyRecipeBook.Domain.ValueObjects;

public abstract class MyRecipeBookRuleConstants
{
    public const int MaximumNumberIngredients = 5;
    public const string ChatModel = "gpt-4o";
    public const int MaximumImageUrlLifetimeInMinutes = 10;
    public const int RefreshTokenExpirationDays = 7;
}