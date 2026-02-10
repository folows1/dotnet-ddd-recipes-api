using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infra.Security.Crypto;

namespace CommonTestUtils.Crypto;

public class PasswordEncripterBuilder
{
    public static IPasswordEncripter Build() => new Sha512Encripter("abc");
}