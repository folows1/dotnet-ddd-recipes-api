using Sqids;

namespace CommonTestUtils.Crypto;

public class IdEncripterBuilder
{
    public static SqidsEncoder<long> Build()
    {
        return new SqidsEncoder<long>(new SqidsOptions
            {
                MinLength = 3,
                Alphabet = "aUudAokpKBbCgOfGDPJjF"
            }
        );
    }
}