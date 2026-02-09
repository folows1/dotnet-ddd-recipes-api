using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infra.Security.Tokens.Access.Generator;

namespace CommonTestUtils.Tokens;

public static class JwtTokenGeneratorBuilder
{
    public static IAccessTokenGenerator Build() =>
        new JwtTokenGenerator(expirationTimeMinutes: 5, signingKey: "12345678901234567890123456789012");
}