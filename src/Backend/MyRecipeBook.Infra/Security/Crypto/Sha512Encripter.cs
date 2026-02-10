using System.Security.Cryptography;
using System.Text;
using MyRecipeBook.Domain.Security.Cryptography;

namespace MyRecipeBook.Infra.Security.Crypto;

public class Sha512Encripter : IPasswordEncripter
{
    private readonly string _hashKey;

    public Sha512Encripter(string hashkey)
    {
        _hashKey = hashkey;
    }

    public string Encrypt(string pwd)
    {
        var newPwd = $"{pwd}{_hashKey}";

        var bytes = Encoding.UTF8.GetBytes(newPwd);

        var hashBytes = SHA512.HashData(bytes);

        return StringBytes(hashBytes);
    }

    private static string StringBytes(byte[] bytes)
    {
        var sb = new StringBuilder();

        foreach (var b in bytes)
        {
            var hex = b.ToString("x2");
            sb.Append(hex);
        }

        return sb.ToString();
    }
}