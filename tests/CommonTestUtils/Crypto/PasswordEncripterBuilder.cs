using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infra.Security.Crypto;

namespace CommonTestUtils.Crypto;

public static class PasswordEncripterBuilder
{
    public static IPasswordEncripter Build() => new BCryptNet();
}