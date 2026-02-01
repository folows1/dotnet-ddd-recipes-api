using MyRecipeBook.Application.Services.Crypto;

namespace CommonTestUtils.Crypto;

public class PasswordEncripterBuilder
{
    public static PasswordEncripter Build() => new PasswordEncripter("abc");
}